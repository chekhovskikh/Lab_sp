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
        /// Обновляет кадр в PictureBox
        /// </summary>
        /// <param name="colorImage">Текущий кадр</param>
        /// <param name="rectangles">Содержит все найденные рамки номеров</param>
        private void RefreshFrame(Image<Bgr, Byte> colorImage, Rectangle[] rectangles)
        {
            foreach (var rectangle in rectangles)
                colorImage.Draw(rectangle, new Bgr(0, 255, 0), 1);
            pictureBox.Image = colorImage.Bitmap;
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

                Rectangle[] rectangles = Utils.HaarDetect(image);
                RefreshFrame(image.ToImage<Bgr, Byte>(), rectangles);
            }
            MessageBox.Show("Видео закончилось");
        }
    }
}
