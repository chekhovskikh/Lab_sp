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

namespace Lab_sp
{
    public partial class Form1 : Form
    {
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
        private byte[] GetData(Mat image, Rectangle rectangle, int size = 40)
        {
            Mat objectFromFrame = new Mat(image, rectangle);
            Mat resizeObject = new Mat();
            CvInvoke.Resize(objectFromFrame, resizeObject, new Size(size, size));
            return resizeObject.GetData();
        }

        /// <summary>
        /// Запускает видео
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            capture = new Capture(Properties.Resources.video_path);
            new Thread(PlayVideo).Start();
        }

        /// <summary>
        /// Покадрово обновляет PictureBox, 
        /// пока в видео не закончатся кадры
        /// </summary>
        private void PlayVideo() 
        {
            while (true)
            {
                Mat image = capture.QueryFrame();

                if (image == null)
                    break;

                VectorOfVectorOfPoint contours = ImageUtils.FindContours(image);
                Image<Bgr, Byte> colorImage = image.ToImage<Bgr, Byte>();

                for (int i = 0; i < contours.Size; i++)
                {
                    if (CvInvoke.ContourArea(contours[i]) > 50)
                    {
                        Rectangle rectangle = CvInvoke.MinAreaRect(contours[i]).MinAreaRect();
                        try
                        {
                            byte[] imageInBytes = GetData(image, rectangle); //apply to the network
                        }
                        catch (CvException) { }
                        CvInvoke.Rectangle(colorImage, rectangle, new MCvScalar(0, 255, 0), 2);
                    }
                }
                lock (pictureBox) { pictureBox.Image = colorImage.Bitmap; }
            }
            MessageBox.Show("Видео закончилось");
        }
    }
}
