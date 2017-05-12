using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Emgu.CV;
using System.Threading;
using Emgu.CV.UI;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Recognition.NeuralNet;
using Lab_sp.Core;
using Lab_sp.View.Forms;
using Lab_sp.Entities;
using System.Data.Entity;
using System.Collections;

namespace Lab_sp
{
    public partial class Main : Form
    {
        private bool isPlay;
        private bool stopFrame = true;
        private Queue<List<string>> buffer;

        /// <summary>
        /// Пусть будет - это конструктор
        /// </summary>
        public Main()
        {
            InitializeComponent();
            SelectFormAccordingToLevelAccess();
            buffer = new Queue<List<string>>();
        }

        /// <summary>
        /// Изменить визуальное представление формы соответственно уровню доступа пользователя
        /// </summary>
        private void SelectFormAccordingToLevelAccess()
        {
            switch (Settings.Instance.Role)
            {
                case Settings.UserRole.Admin:
                    Text += " (Администратор)";
                    break;
                default:
                    //Сокрытие управления
                    managmentMenuItem.Visible = false;
                    manualMenuItem.Visible = false;
                    Text += " (Пользователь)";
                    break;
            }
        }

        /// <summary>
        /// Позволяет работать с видео покадрово
        /// </summary>
        private Capture capture = null;
        private Thread thread;

        /// <summary>
        /// Получает массив байтов (каждый пиксель занимает 3 элемента, так как color = RGB)
        /// пикселей входящих в облать заданного прямоугольника изображения
        /// </summary>
        /// <param name="image">изображение для получения данных</param>
        /// <param name="rectangle">рассматриваемая область изображения</param>
        /// <param name="size">размер для ресайза</param>
        /// <returns></returns>
        private byte[] GetData(Mat image, Rectangle rectangle, Size size)
        {
            Mat objectFromFrame = new Mat(image, rectangle);
            Mat resizeObject = new Mat();
            CvInvoke.Resize(objectFromFrame, resizeObject, size);
            return resizeObject.GetData();
        }

        /// <summary>
        /// Запускает видео
        /// </summary>
        private void picturePlay_Click(object sender, EventArgs e)
        {
            if (openFileDialog.FileName == "default")
            {
                MessageBox.Show("Выберите видеозапись для начала работы");
                return;
            }

            stopFrame = !stopFrame;
            if (stopFrame)
            {
                fileMenuItem.Enabled = true;
                managmentMenuItem.Enabled = true;
                picturePlay.Image = Properties.Resources.play1;
            }
            else
            {
                fileMenuItem.Enabled = false;
                managmentMenuItem.Enabled = false;
                picturePlay.Image = Properties.Resources.pause1;
            }

            if (!isPlay)
            {
                Clear();
                isPlay = true;
                if (thread == null)
                    thread = new Thread(PlayVideo);
                thread.Start();
            }
        }
        
        delegate void AddControl(Control control);
        /// <summary>
        /// Покадрово обновляет PictureBox, 
        /// пока в видео не закончатся кадры
        /// </summary>
        private void PlayVideo()
        {
            buffer.Clear();
            AddControl addControl = new AddControl(logPanel.Controls.Add);

            while (true)
            {
                while (stopFrame)
                    Thread.Sleep(10);

                Mat image = capture.QueryFrame();

                if (image == null)
                    break;

                List<string> foundGosts = new List<string>();
                VectorOfVectorOfPoint contours = image.FindContours();
                Image<Bgr, Byte> colorImage = image.ToImage<Bgr, Byte>();
                Image<Gray, Byte> grayImageCopy = colorImage.GetMonohromImage();
                //Image<Bgr, Byte> colorImageCopy = colorImage.Copy();
                Image<Gray, Byte> grayImage = colorImage.GetMonohromImage();
                //Эррозия
                //Уберем шумы на ч/б изображении
                CvInvoke.GaussianBlur(grayImage, grayImage, new Size(13, 13), 6);
                CvInvoke.Threshold(grayImage, grayImage, 127, 255, ThresholdType.Binary);

                for (int i = 0; i < contours.Size; i++)
                {
                    if (CvInvoke.ContourArea(contours[i]) > 200)
                    {
                        Rectangle rectangle = CvInvoke.MinAreaRect(contours[i]).MinAreaRect();
                        Image<Gray, Byte> binaryImage = null;
                        try
                        {
                            CvInvoke.cvSetImageROI(grayImage, rectangle);
                            binaryImage = grayImage.Copy();
                            CvInvoke.cvResetImageROI(grayImage);//.ROI = new Rectangle(0, 0, grayImage.Width, grayImage.Height);
                        }
                        catch (CvException) { }
                        CvInvoke.DrawContours(colorImage, contours, i, new MCvScalar(255, 0, 0), 1);
                        //binaryImage - бинарное изображение, белый цвет - соответствует красному, синему и желтому цветам на цветном кадре
                        //Проверка на процентное содержание этих 3-х цветов в распознаваемом фрагменте (от 0 до 1)
                        if (binaryImage != null && ImageExtention.GetPropBGR(binaryImage) >= 0.15)
                            try
                            {
                                double[] data = null;
                                if (!Settings.Instance.IsColor)
                                {
                                    var dataBW = GetData(grayImageCopy.Mat, rectangle, Settings.Instance.Size);
                                    data = ImageExtention.ConvertToDoubles(dataBW);
                                }
                                else
                                {
                                    var dataColor = GetData(colorImage.Mat, rectangle, Settings.Instance.Size);
                                    data = ImageExtention.GetRGB(dataColor);
                                }
                                
                                var result = Calculate(data);
                                if (result[1] > 0.5)
                                {
                                    Sign sign = Settings.Instance.DataLayer.GetSign((int)result[0]);
                                    CvInvoke.DrawContours(colorImage, contours, i, new MCvScalar(255, 0, 0));
                                    CvInvoke.Rectangle(colorImage, rectangle, new MCvScalar(0, 255, 0), 2);
                                    CvInvoke.PutText(colorImage, sign.Gost, new Point(rectangle.X, rectangle.Y + rectangle.Height / 2), FontFace.HersheyPlain, 2, new MCvScalar(0, 0, 255), 2);

                                    if (!BufferIsContains(sign.Gost) && !foundGosts.Contains(sign.Gost))
                                    {
                                        //добавляем в базу найденный на видео знак если он не появлялся в течении N кадров
                                        Bitmap bmp = ImageExtention.ResizedRectangle(image, rectangle, Settings.Instance.Size);
                                        double ms = capture.GetCaptureProperty(CapProp.PosMsec);
                                        string time = TimeSpan.FromMilliseconds(ms).ToString().Substring(0, 8);

                                        //раскомментировать для добавлении в базу
                                        Settings.Instance.DataLayer.AddVideoSign(new VideoSign(sign.Id, time, bmp));

                                        Label infoLabel = new Label();
                                        infoLabel.Text = sign.Name + Environment.NewLine +
                                                         sign.Gost + Environment.NewLine +
                                                         time + Environment.NewLine +
                                                         sign.Type + Environment.NewLine;
                                        infoLabel.Size = new Size(200, 70);

                                        logPanel.Invoke(addControl, CreatePictureBox(bmp));
                                        logPanel.Invoke(addControl, CreatePictureBox(sign.Image));
                                        logPanel.Invoke(addControl, infoLabel);
                                        logPanel.Invoke(new EventHandler(delegate { logPanel.VerticalScroll.Value = logPanel.VerticalScroll.Maximum; }));
                                    }
                                    foundGosts.Add(sign.Gost);
                                }
                            }
                            catch (CvException) { }
                    }
                }
                UpdateBuffer(foundGosts);

                lock (pictureBox) { pictureBox.Image = colorImage.Bitmap; }
                //lock (pictureBox) { pictureBox1.Image = colorImageCopy.And(grayImage.Convert<Bgr, Byte>()).Bitmap; }
            }
            menuStrip.Invoke(new EventHandler(delegate 
            {
                fileMenuItem.Enabled = true;
                managmentMenuItem.Enabled = true;
            }));
            picturePlay.Invoke(new EventHandler(delegate { picturePlay.Image = Properties.Resources.play1; }));
            MessageBox.Show("Видео закончилось");
            stopFrame = !stopFrame;
            isPlay = !isPlay;
            thread = null;
            capture = new Capture(openFileDialog.FileName);
        }

        private bool BufferIsContains(string gost)
        {
            foreach (var list in buffer)
                if (list.Contains(gost))
                    return true;
            return false;
        }

        private void UpdateBuffer(List<string> list)
        {
            buffer.Enqueue(list);
            if (buffer.Count > 10) //10 - длинна буффера
                buffer.Dequeue();
        }

        private PictureBox CreatePictureBox(Bitmap image)
        {
            PictureBox pic = new PictureBox();
            pic.Size = Settings.Instance.Size;
            pic.Image = new Bitmap(image);
            pic.Location = new Point(0, 0);
            return pic;
        }

        /// <summary>
        /// Определяет возможность принадлежность к классу изображений
        /// </summary>
        /// <param name="image">Изображения для проверки</param>
        /// <returns>Класс с максимальной возможностью и его вес</returns>
        private double[] Calculate(double[] image)
        {
            double[] output = Settings.Instance.Network.Calculate(image);
            double max = output[0];
            int max_i = 0;
            for (int i = 1; i < output.Length; i++)
                if (max < output[i])
                {
                    max = output[i];
                    max_i = i;
                }
            return new double[] { max_i + 1, max };
        }
        

        private void Clear()
        {
            logPanel.Controls.Clear();
        }

        private void clearLogMenuItem_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void learningMenuItem_Click(object sender, EventArgs e)
        {
            new TrainingForm().ShowDialog();
        }

        private void infoMenuItem_Click(object sender, EventArgs e)
        {
            new InfoForm().ShowDialog();
        }

        private void signsMenuItem_Click(object sender, EventArgs e)
        {
            new SignManualForm().ShowDialog();
        }

        private void usersMenuItem_Click(object sender, EventArgs e)
        {
            new UsersManualForm().ShowDialog();
        }

        private void videoSignsMenuItem_Click(object sender, EventArgs e)
        {
            new VideoSignManualForm().ShowDialog();
        }

        private void openVideoMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = openFileDialog.FileName;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Capture cap = new Capture(openFileDialog.FileName);
                Mat img = cap.QueryFrame();
                if (img.Width <= 1920 && img.Width >= 640 && img.Height <= 1080 && img.Height >= 480)
                {
                    capture = cap;
                    picturePlay.Visible = true;
                    pictureBox.Image = img.Bitmap;
                }
                else
                {
                    openFileDialog.FileName = fileName;
                    MessageBox.Show("Видеозапись имеет недопустимое разрешение!");
                }
            }
        }

        private void picturePlay_MouseEnter(object sender, EventArgs e)
        {
            if (!stopFrame) picturePlay.Image = Properties.Resources.pause2;
            else picturePlay.Image = Properties.Resources.play2;
        }

        private void picturePlay_MouseLeave(object sender, EventArgs e)
        {
            if (!stopFrame) picturePlay.Image = Properties.Resources.pause1;
            else picturePlay.Image = Properties.Resources.play1;
        }

        private Size minimumSize = new Size(590, 460);
        private Size maximumSize = new Size(940, 460);
        private void buttonHideLog_Click(object sender, EventArgs e)
        {
            if (buttonHideLog.Text == "<")
            {
                buttonHideLog.Text = ">";
                MinimumSize = minimumSize;
                MaximumSize = minimumSize;
            }
            else
            {
                buttonHideLog.Text = "<";
                MinimumSize = maximumSize;
                MaximumSize = maximumSize;
            }
        }

        private void clearMenuItem_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите закрыть программу?", "Подтверждение",
                MessageBoxButtons.YesNo) == DialogResult.Yes)
                CNNFileManager.SaveNetwork(Settings.Instance.Network, Properties.Resources.data);
            else e.Cancel = true;
        }

        private void taskMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Разработка автоматизированной системы " + 
                "по распознаванию дорожных знаков на видеозаписи.", "Задание");
        }
    }
}
