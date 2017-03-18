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

namespace Lab_sp
{
    public partial class Form1 : Form
    {
        private Net net;
        private TrainerBase trainer;
        private Bitmap[][] exemples;

        private Size sizeImage;

        /// <summary>
        /// Пусть будет - это конструктор
        /// </summary>
        public Form1()
        {
            InitializeComponent();
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
        private byte[] GetData(Mat image, Rectangle rectangle, int size = 48)
        {
            Mat objectFromFrame = new Mat(image, rectangle);
            Mat resizeObject = new Mat();
            CvInvoke.Resize(objectFromFrame, resizeObject, new Size(size, size));
            return resizeObject.GetData();
        }

        /// <summary>
        /// Перегрузка функции - на входе Bitmap, а не Mat
        /// Получает массив байтов (каждый пиксель занимает 3 элемента, так как color = RGB)
        /// пикселей входящих в облать заданного прямоугольника изображения
        /// </summary>
        /// <param name="image"></param>
        /// <param name="rectangle"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private byte[] GetData(Bitmap bmp, Rectangle rect)
        {
            return new Image<Bgr, Byte>(bmp).Mat.GetData();
        }


        /// <summary>
        /// Запускает видео
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
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

        private Boolean isPlay;
        private Boolean stopFrame = true;
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

                VectorOfVectorOfPoint contours = ImageUtils.FindContours(image);
                Image<Bgr, Byte> colorImage = image.ToImage<Bgr, Byte>();
                Image<Bgr, Byte> colorImageCopy = colorImage.Copy();
                Image<Gray, Byte> grayImage = ImageUtils.GetMonohromImage(colorImage);
                //Эррозия
                //Уберем шумы на ч/б изображении
                CvInvoke.GaussianBlur(grayImage, grayImage, new Size(13, 13), 6);
                CvInvoke.Threshold(grayImage, grayImage, 127, 255, ThresholdType.Binary);

                for (int i = 0; i < contours.Size; i++)
                {
                    if (CvInvoke.ContourArea(contours[i]) > 200)
                    {
                        Rectangle rectangle = CvInvoke.MinAreaRect(contours[i]).MinAreaRect();
                        Image<Gray, Byte> im = null;
                        try
                        {
                            CvInvoke.cvSetImageROI(grayImage, rectangle);
                            im = grayImage.Copy();
                            CvInvoke.cvResetImageROI(grayImage);//.ROI = new Rectangle(0, 0, grayImage.Width, grayImage.Height);
                        }
                        catch (CvException) { }
                        CvInvoke.DrawContours(colorImage, contours, i, new MCvScalar(255, 0, 0), 1);
                        //im - бинарное изображение, белый цвет - соответствует красному, синему и желтому цветам на цветном кадре
                        //Проверка на процентное содержание этих 3-х цветов в распознаваемом фрагменте (от 0 до 1)
                        if (im != null && ImageUtils.GetPropBGR(im) >= 0.15)
                            try
                            {
                                var data = GetData(image, rectangle, sizeImage.Width);
                                var classImage = Def(GetDoubles(data));
                                if (classImage[1] >= 0.7)
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


        private void button2_Click(object sender, EventArgs e)
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
            for (int j = 0; j < 10; j++)
                for (int i = 0; i < 4; i++)
                {
                    Learning("C:\\Programing\\СисПр\\Lab_sp\\Examples\\Class" + (i + 1), 48, i, trainer, 1);
                }

        }

        private double[] Def(double[] image)
        {
            var x = new Volume(image);
            var prob = net.Forward(x);
            try
            {
                for (int i = 0; i < prob.Length; i++)
                    Print("probability that x is class " + i + ": " + prob.Get(i));
                Print("==================================================: ");
            }
            catch { }
            if (prob.Length == 0)
                return null;
            double max = prob.Get(0);
            int max_i = 0;
            for (int i = 1; i < prob.Length; i++)
                if (max < prob.Get(i))
                {
                    max = prob.Get(i);
                    max_i = i;
                }
            return new double[] { max_i, max };
        }

        public void Learning(Bitmap[][] sources, int[] classes, TrainerBase trainer, int iters)
        {
            if (sources.Length != classes.Length || sources.Length == 0)
                return;

            var gu = GraphicsUnit.Pixel;
            for (int i = 0; i < iters; i++)
            {
                Print(i + "=================");
                for (int j = 0; j < sources.Length; j++)
                {
                    for (int k = 0; k < sources[0].Length; k++)
                    {
                        Bitmap bmp = null;
                        bmp = sources[j][k];
                        var rectA = new Rectangle(0, 0, bmp.Width, bmp.Height);
                        var x = new Volume(GetDoubles(GetData(bmp, rectA)));
                        //var prob = net.Forward(x);
                        //for (int r = 0; r < prob.Length; r++)
                        //    Print("Probability that x is class " + r + ": " + prob.Get(r));
                        trainer.Train(x, classes[j]);
                    }
                }
            }
        }

        public void Learning(String pathDir, int size, int classImage, TrainerBase trainer, int iters = 1)
        {
            var pathToFiles = Directory.GetFiles(pathDir);
            var gu = GraphicsUnit.Pixel;
            var bmps = new Bitmap[pathToFiles.Length];
            for (int j = 0; j < bmps.Length; j++)
                bmps[j] = new Bitmap(Image.FromFile(pathToFiles[j]));

            for (int i = 0; i < iters; i++)
            {
                Print(i + "=================");
                for (int j = 0; j < pathToFiles.Length; j++)
                {
                    Bitmap bmp = bmps[j];
                    pictureBox.Image = bmp;
                    if (size != bmp.Width || size != bmp.Height)
                    {
                        bmp = ScaleImage(bmp, size, size);
                    }
                    Rectangle rect = new Rectangle(0, 0, size, size);
                    var data = GetDoubles(GetData(bmp, rect));
                    var x = new Volume(data);
                    var prob = net.Forward(x);
                    for (int r = 0; r < prob.Length; r++)
                        Print("Probability that x is class " + r + ": " + prob.Get(r));
                    trainer.Train(x, classImage);
                }
            }
        }



        private static Bitmap ScaleImage(Bitmap source, int width, int height)
        {
            Image<Bgr, Byte> captureImage = new Image<Bgr, byte>(source);
            return captureImage.Resize(width, height, Inter.Linear).ToBitmap();
        }

        private void Print(String text)
        {
            textBox1.Text = text + Environment.NewLine + textBox1.Text;
        }

        private void Clear()
        {
            textBox1.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var resdial = openFileDialog1.ShowDialog();
            if (resdial == DialogResult.Cancel)
                return;
            var stream = openFileDialog1.OpenFile();
            if (stream != null)
            {
                var bmp = new Bitmap(Image.FromStream(stream));
                var rectA = new Rectangle(0, 0, bmp.Width, bmp.Height);
                if (rectA.Width != sizeImage.Width || sizeImage.Height != sizeImage.Height)
                    bmp = ScaleImage(bmp, sizeImage.Width, sizeImage.Height);
                Image<Bgr, Byte> img = new Image<Bgr, byte>(bmp);
                var img2 = ImageUtils.GetMonohromImage(img);
                pictureBox.Image = img2.Bitmap;
                var data = GetData(bmp, new Rectangle(0,0,bmp.Width,bmp.Height));
                var colorInfo = ImageUtils.GetPropBGR(img2);
                var countPix = img2.Width * img2.Height;
                Print("файл загружен! Анализ...");
                Print("Всего ярковыраженых пикселей: " + colorInfo*countPix + " из " + countPix + " (" + (colorInfo*100) +"%)");
                var res = Def(GetDoubles(data));
                String info = "This is ";
                if (colorInfo >= 0.15)
                    info += "class: " + res[0] + " with a chance: " + res[1];
                else
                    info += "a void";
                Print(info);
                Print("==========================");

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            sizeImage = new Size(48, 48);
            net = new Net();
            net.AddLayer(new InputLayer(Properties.Resources.a1_48x48.Width,
                Properties.Resources.a1_48x48.Height, 3));
            net.AddLayer(new FullyConnLayer(32));//32
            net.AddLayer(new TanhLayer());
            net.AddLayer(new FullyConnLayer(16));//16
            net.AddLayer(new TanhLayer());
            net.AddLayer(new FullyConnLayer(4));
            net.AddLayer(new SoftmaxLayer(4));

            //sizeImage = new Size(48, 48);
            //net = new Net();
            //net.AddLayer(new InputLayer(48,
            //    48, 3));
            //net.AddLayer(new ConvLayer(32, 32, 16));//7,7,24
            //net.AddLayer(new ReluLayer());
            //net.AddLayer(new ConvLayer(24, 24, 8));//5,5,30
            //net.AddLayer(new ReluLayer());
            //net.AddLayer(new PoolLayer(2, 2));
            //net.AddLayer(new ReluLayer());
            //net.AddLayer(new ConvLayer(12, 12, 8));// 5,5,30
            //net.AddLayer(new ReluLayer());
            //net.AddLayer(new PoolLayer(3, 3));
            //net.AddLayer(new FullyConnLayer(4));
            //net.AddLayer(new SoftmaxLayer(4));

            // FluentNet net = FluentNet.Create(48, 48, 3)
            //.Conv(7, 7, 24).Stride(1).Pad(3)
            //.Relu()
            //.Pool(2, 2).Stride(2)
            //.Conv(5, 5, 30).Stride(1).Pad(2)
            //.Relu()
            //.Pool(2, 2).Stride(2)
            //.Conv(5, 5, 30).Stride(1).Pad(2)
            //.Relu()
            //.Pool(3, 3).Stride(2)
            //.FullyConn(4)
            //.Softmax(4)
            //.Build();

            //Формируем эталоны для обучения
            trainer = new SgdTrainer(net) { LearningRate = 0.01, L2Decay = 0.001 };
        }

        /// <summary>
        /// Перевод массива byte[] -> double[]
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        static double[] GetDoubles(byte[] bytes)
        {
            var doubles = new double[bytes.Length];
            for (int i = 0; i < doubles.Length; i++)
                doubles[i] = bytes[i];
            return doubles;
        }

        
    }
}
