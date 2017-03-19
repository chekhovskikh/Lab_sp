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
using ConvNetSharp;
using ConvNetSharp.Layers;
using ConvNetSharp.Training;
using Recognition.NeuralNet;

namespace Lab_sp
{
    public partial class Main : Form
    {
        private Network net;
        private List<TrainingImage> trainingExamples;

        private int size = 48;

        /// <summary>
        /// Пусть будет - это конструктор
        /// </summary>
        public Main()
        {
            InitializeComponent();
            net = new Network(size, size, 4);
        }

        /// <summary>
        /// Позволяет работать с видео покадрово
        /// </summary>
        private Capture capture = null;

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
        /// Перевод массива byte[] -> double[] с преобразованием 
        /// представления цвета одним числом
        /// </summary>
        /// <param name="bytes">Массив для конвектирования</param>
        /// <returns>Конвектированный массив</returns>
        static double[] ConvectToDoubles(byte[] bytes)
        {
            double[] doubles = new double[bytes.Length / 3];
            for (int i = 0; i < doubles.Length; i++)
            {
                byte red = bytes[3 * i + 1];
                byte green = bytes[3 * i + 2];
                byte blue = bytes[3 * i];
                doubles[i] = ImageExtention.ToRGB(red, green, blue);
            }
            return doubles;
        }

        /// <summary>
        /// Запускает видео
        /// </summary>
        private void buttonGo_Click(object sender, EventArgs e)
        {
            Clear();
            capture = new Capture(Properties.Resources.video_path);
            stopFrame = !stopFrame;

            if (!isPlay)
            {
                isPlay = true;
                new Thread(PlayVideo).Start();
            }
        }

        private bool isPlay;
        private bool stopFrame = true;
        private Mat lastImage;
        /// <summary>
        /// Покадрово обновляет PictureBox, 
        /// пока в видео не закончатся кадры
        /// </summary>
        private void PlayVideo()
        {
            while (true)
            {
                Mat image = null;
                if (!stopFrame)
                {
                    image = capture.QueryFrame();
                    lastImage = image;
                }
                else image = lastImage;

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
                                var data = GetData(image, rectangle, size);
                                var classImage = Calculate(ConvectToDoubles(data));
                                if (classImage[1] >= 0d)
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
            MessageBox.Show("Видео закончилось");
        }


        private void buttonLearning_Click(object sender, EventArgs e)
        {
            // species a 2-layer neural network with one hidden layer of 20 neurons
            //if (net == null)
            //{

            //    var bmps1 = new Bitmap[] { Properties.Resources.a1_48x48, Properties.Resources.a2_48x48, Properties.Resources.a3_48x48 };
            //    var bmps2 = new Bitmap[] { Properties.Resources.b1_48x48, Properties.Resources.b2_48x48, Properties.Resources.b3_48x48 };
            //    var bmps3 = new Bitmap[] { Properties.Resources.c1_48x48, Properties.Resources.c2_48x48, Properties.Resources.c3_48x48 };
            //    var bmps4 = new Bitmap[] { Properties.Resources.d1_48x48, Properties.Resources.d2_48x48, Properties.Resources.d3_48x48 };
            //    exemples = new Bitmap[][] { bmps1, bmps2, bmps3, bmps4 };
            //}

            //Learning(exemples, new int[] { 0, 1, 2, 3 }, trainer, 10);


            //Не работает нормально - в разработке
            //var res = folderBrowserDialog1.ShowDialog();
            //int classImage = 0;
            //try
            //{
            //    classImage = int.Parse(textBox2.Text);
            //    if (res == DialogResult.OK)
            //        Learning(folderBrowserDialog1.SelectedPath, 48, classImage, trainer, 10);
            //}
            //catch
            //{
            //    MessageBox.Show("Возможно номер класса задан неверно!");
            //}

            //Указать пути к папкам с выборками
            //i - число классов
            //j - число итераций обучения
            //Временная мера - тестирование

            int countTypes = 4;
            if (trainingExamples == null)
                LoadTrainingExamples(size, countTypes);

            Learning(10);
        }

        /// <summary>
        /// Обучение нейронной сети
        /// </summary>
        /// <param name="count">Количество циклов обучения</param>
        public void Learning(int count)
        {
            for (int k = 0; k < count; k++)
                for (int i = 0; i < trainingExamples.Count; i++)
                    net.Train(trainingExamples[i].Input, trainingExamples[i].DesiredOutput());
        }

        /// <summary>
        /// Заполняет тренировочную базу
        /// </summary>
        /// <param name="size">Размер тренировочных изображений</param>
        /// <param name="countType">Количество классов</param>
        private void LoadTrainingExamples(int size, int countType)
        {
            trainingExamples = new List<TrainingImage>();
            for (int folderIndex = 0; folderIndex < countType; folderIndex++)
            {
                string[] pathToFiles = Directory.GetFiles(@"Resources\Class" + (folderIndex + 1));

                for (int indexFile = 0; indexFile < pathToFiles.Length; indexFile++)
                {
                    Bitmap bmp = ImageExtention.CreateBitmap(pathToFiles[indexFile], size);
                    trainingExamples.Add(new TrainingImage(ConvectToDoubles(bmp.GetData()), folderIndex, countType));
                }
            }
            Shuffle(trainingExamples);
        }

        /// <summary>
        /// Перемешивает элементы массива случайным образом.
        /// Алгоритм перемешивания Fisher–Yates shuffle:
        /// http://en.wikipedia.org/wiki/Fisher%E2%80%93Yates_shuffle#The_modern_algorithm
        /// </summary>
        /// <typeparam name="T">Тип элементов массива</typeparam>
        /// <param name="array">Массив для перемешивания</param>
        public void Shuffle<T>(List<T> array)
        {
            Random rnd = new Random();
            for (int i = array.Count - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                var buffer = array[i];
                array[i] = array[j];
                array[j] = buffer;
            }
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
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            Bitmap bmp = ImageExtention.CreateBitmap(openFileDialog.FileName, size);
            var img = new Image<Bgr, byte>(bmp).GetMonohromImage();

            pictureBox.Image = img.Bitmap;
            byte[] data = bmp.GetData();
            double colorInfo = ImageExtention.GetPropBGR(img);
            int countPix = img.Width * img.Height;

            Print("файл загружен! Анализ...");
            Print("Всего ярковыраженых пикселей: " + colorInfo * countPix + " из " + countPix + " (" + (colorInfo * 100) + "%)");

            double[] result = Calculate(ConvectToDoubles(data));
            string info = "This is ";
            if (colorInfo >= 0.15)
                info += "class: " + result[0] + " with a chance: " + result[1];
            else
                info += "a void";
            Print(info);
            Print("==========================");
        }
    }
}
