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

namespace Lab_sp
{
    public partial class Main : Form
    {
        private Network net;   

        private bool isPlay;
        private bool stopFrame = true;
        private Mat lastImage;

        /// <summary>
        /// Пусть будет - это конструктор
        /// </summary>
        public Main()
        {
            InitializeComponent();
            SelectFormAccordingToLevelAccess();
            //net = new Network(size, size, 4);
            net = Settings.Instance.Network;
            //Settings.Instance.Network = net;
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
                    managmentMenuItem.Enabled = false;
                    managmentMenuItem.Visible = false;
                    signsMenuItem.Enabled = false;
                    signsMenuItem.Visible = false;
                    usersMenuItem.Enabled = false;
                    usersMenuItem.Visible = false;
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
        private byte[] GetData(Mat image, Rectangle rectangle, int size)
        {
            Mat objectFromFrame = new Mat(image, rectangle);
            Mat resizeObject = new Mat();
            CvInvoke.Resize(objectFromFrame, resizeObject, new Size(size, size));
            return resizeObject.GetData();
        }

        /// <summary>
        /// Запускает видео
        /// </summary>
        private void buttonGo_Click(object sender, EventArgs e)
        {
            Clear();
            if (openFileDialog.FileName == "default")
            {
                MessageBox.Show("Выберите видеозапись для начала работы");
                return;
            }
            stopFrame = !stopFrame;
            if (stopFrame) openVideoMenuItem.Enabled = true;
            else openVideoMenuItem.Enabled = false;

            if (!isPlay)
            {
                isPlay = true;
                if (thread == null)
                    thread = new Thread(PlayVideo);
                thread.Start();
            }
        }

        /// <summary>
        /// Покадрово обновляет PictureBox, 
        /// пока в видео не закончатся кадры
        /// </summary>
        private void PlayVideo()
        {
            while (true)
            {
                while (stopFrame)
                    Thread.Sleep(10);

                Mat image = capture.QueryFrame();

                if (image == null)
                    break;

                //var archor = new Point(-1, -1);

                //Эррозия
                //Mat kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Cross, new
                //    Size(5, 5), new Point(-1, -1));
                //image = image.ToImage<Bgr, Byte>().MorphologyEx(Emgu.CV.CvEnum.MorphOp.Dilate, kernel, archor, 3,
                //BorderType.Constant, new MCvScalar(0, 0, 0)).Mat;

                //Четкость
                //Matrix<float> kernel1 = new Matrix<float>(new float[3, 3] { { -0.1f, -0.1f, -0.1f },
                //                                              {- 0.1f, 2f, -0.1f }, { -0.1f, -0.1f, -0.1f } });
                //Увеличение яркости
                //Matrix<float> kernel2 = new Matrix<float>(new float[3, 3] { { -0.1f, 0.2f, -0.1f }, {
                //                                                  0.2f, 3f, 0.2f }, { -0.1f, 0.2f, -0.1f } });
                //CvInvoke.Filter2D(image.ToImage<Bgr, Byte>().Copy(), image, kernel1, archor);

                VectorOfVectorOfPoint contours = image.FindContours();
                Image<Bgr, Byte> colorImage = image.ToImage<Bgr, Byte>();
                Image<Gray, Byte> grayImageCopy = colorImage.GetMonohromImage();
                Image<Bgr, Byte> colorImageCopy = colorImage.Copy();
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
                                var data = GetData(grayImageCopy.Mat, rectangle, Settings.Instance.Size.Width);
                                var classImage = Calculate(ImageExtention.ConvertToDoubles(data));
                                if (classImage[1] > 0.5)
                                {
                                    CvInvoke.DrawContours(colorImage, contours, i, new MCvScalar(255, 0, 0));
                                    CvInvoke.Rectangle(colorImage, rectangle, new MCvScalar(0, 255, 0), 2);
                                    CvInvoke.PutText(colorImage, classImage[0] + "", new Point(rectangle.X, rectangle.Y + rectangle.Height / 2), FontFace.HersheyPlain, 2, new MCvScalar(0, 0, 255), 2);
                                }
                            }
                            catch (CvException) { }
                    }
                }
                lock (pictureBox) { pictureBox.Image = colorImage.Bitmap; }
                lock (pictureBox) { pictureBox1.Image = colorImageCopy.And(grayImage.Convert<Bgr, Byte>()).Bitmap; }
            }
            menuStrip.Invoke(new EventHandler(delegate { openVideoMenuItem.Enabled = true; }));
            MessageBox.Show("Видео закончилось");
            stopFrame = !stopFrame;
            isPlay = !isPlay;
        }


        private void buttonLearning_Click(object sender, EventArgs e)
        {
           
        }

        /// <summary>
        /// Определяет возможность принадлежность к классу изображений
        /// </summary>
        /// <param name="image">Изображения для проверки</param>
        /// <returns>Класс с максимальной возможностью и его вес</returns>
        private double[] Calculate(double[] image)
        {
            double[] output = net.Calculate(image);

            /*for (int i = 0; i < output.Length; i++)
                Print("probability that x is class " + i + ": " + output[i]);
            Print("==================================================: ");*/

            double max = output[0];
            int max_i = 0;
            for (int i = 1; i < output.Length; i++)
                if (max < output[i])
                {
                    max = output[i];
                    max_i = i;
                }
            return new double[] { max_i, max };
        }

        private void Print(string text)
        {
            infoBox.Text = text + Environment.NewLine + infoBox.Text;
        }

        private void Clear()
        {
            infoBox.Text = "";
        }

        private void buttonOpenImage_Click(object sender, EventArgs e)
        {
        }

        private void buttonClear_Click(object sender, EventArgs e)
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

        private void openVideoMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Capture cap = new Capture(openFileDialog.FileName);
                Mat img = cap.QueryFrame();
                if (img.Width <= 1920 && img.Width >= 640 && img.Height <= 1080 && img.Height >= 480)
                {
                    capture = cap;
                    pictureBox.Image = img.Bitmap;
                }
                else
                {
                    openFileDialog.FileName = "default";
                    MessageBox.Show("Видеозапись имеет недопустимое разрешение!");
                }
            }
        }
    }
}
