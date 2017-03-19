using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.Util;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using Emgu.CV.Util;

namespace Lab_sp
{
    /// <summary>
    /// Класс, расширяющий возможности работы с изображениями
    /// </summary>
    public static class ImageExtention
    {
        /// <summary>
        /// Гауссовское размытие
        /// </summary>
        /// <param name="grayImage">Изображение для размытия</param>
        /// <param name="sigmaX">Стандартное отклонение</param>
        public static void GaussianBlur(this Image<Gray, Byte> grayImage, double sigmaX = 2)
        {
            Image<Gray, Byte> blurImage = new Image<Gray, Byte>(grayImage.Width, grayImage.Height);
            CvInvoke.GaussianBlur(grayImage, blurImage, new Size(13, 13), sigmaX);
            grayImage.Bitmap = blurImage.Bitmap;
        }

        /// <summary>
        /// Пороговая обработка
        /// </summary>
        /// <param name="grayImage">Изображения для обработки</param>
        /// <param name="threshold">Порог</param>
        /// <param name="maxValue">Максимальное значение</param>
        public static void Threshold(this Image<Gray, Byte> grayImage, double threshold = 127, double maxValue = 255)
        {
            Image<Gray, Byte> thrImage = new Image<Gray, Byte>(grayImage.Width, grayImage.Height);
            CvInvoke.Threshold(grayImage, thrImage, threshold, maxValue, ThresholdType.Binary);
            grayImage.Bitmap = thrImage.Bitmap;
        }

        /// <summary>
        /// Алгоритм Кенни
        /// </summary>
        /// <param name="binaryImage">Изображение для обработки</param>
        /// <param name="threshold1">Первый порог</param>
        /// <param name="threshold2">Второй порог</param>
        public static void Canny(this Image<Gray, Byte> binaryImage, double threshold1 = 127, double threshold2 = 255)
        {
            Image<Gray, Byte> cannyImage = new Image<Gray, Byte>(binaryImage.Width, binaryImage.Height);
            CvInvoke.Canny(binaryImage, cannyImage, threshold1, threshold2);
            binaryImage.Bitmap = cannyImage.Bitmap;
        }

        /// <summary>
        /// Нахождение контуров изображение
        /// </summary>
        /// <param name="image">Изображения, у которого требуется найти контуры</param>
        /// <returns>Список контуров</returns>
        public static VectorOfVectorOfPoint FindContours(this Mat image)
        {
            Image<Gray, Byte> grayImage = image.ToImage<Gray, Byte>();
            GaussianBlur(grayImage);
            Threshold(grayImage);
            Canny(grayImage);

            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(grayImage, contours, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxSimple);
            return contours;
        }

        /// <summary>
        /// Изменяет размер изображения
        /// </summary>
        /// <param name="source">Изображение, размеры которого требуется изменить</param>
        /// <param name="width">Ширина требуемого изображения</param>
        /// <param name="height">Высота требуемого изображения</param>
        /// <returns>Изображение с измененным размером</returns>
        public static Bitmap Resize(this Bitmap source, int width, int height)
        {
            Image<Bgr, Byte> captureImage = new Image<Bgr, byte>(source);
            return captureImage.Resize(width, height, Inter.Linear).ToBitmap();
        }

        /// <summary>
        /// Создание битмапа с ресайзом
        /// </summary>
        /// <param name="path">Путь к изображению</param>
        /// <param name="size">Размер выходного файла</param>
        /// <returns>Изображение с заданным размером</returns>
        public static Bitmap CreateBitmap(string path, int size)
        {
            Bitmap bmp = new Bitmap(Image.FromFile(path));
            if (bmp.Height != size || bmp.Width != size)
                bmp = bmp.Resize(size, size);
            return bmp;
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
        public static byte[] GetData(this Bitmap bmp)
        {
            return new Image<Bgr, Byte>(bmp).Mat.GetData();
        }

        /// <summary>
        /// Представление цвета одним числом
        /// </summary>
        /// <param name="red">Красный</param>
        /// <param name="green">Зеленый</param>
        /// <param name="blue">Синий</param>
        /// <returns>Цвет представленный одним числом</returns>
        public static double ToRGB(byte red, byte green, byte blue)
        {
            return (red << 16) | (green << 8) | blue;
        }

        #region На правах рекламы. Chekhovskikh (с)
        //+===================================+
        //| ЗДЕСЬ МОГЛА БЫТЬ ВАША РЕКЛАМА.    |
        //|          +79277457895             |
        //+===================================+
        #endregion

        //========================================================================================
        // Отдел по экспериментальным функциями
        //========================================================================================
        /// <summary>
        /// Преобразование цветного изображения в ч/б, которая получается сложением
        /// монохромных изображений, в которых белый цвет соответствует R,G или B на цветной картинке
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static Image<Gray, Byte> GetMonohromImage(this Image<Bgr, Byte> img)
        {
            Image<Hsv, Byte> hsvImage = img.Convert<Hsv, Byte>();
            //Определение красного
            Image<Gray, Byte> red = hsvImage.InRange(new Hsv(0, 70, 0), new Hsv(10, 255, 255))//0-255
                .Or(hsvImage.InRange(new Hsv(170, 70, 0), new Hsv(180, 255, 255)));//0-255
            Image<Gray, Byte> yellow = hsvImage.InRange(new Hsv(20, 150, 170), new Hsv(30, 255, 255));//170-255
            Image<Gray, Byte> blue = hsvImage.InRange(new Hsv(100, 120, 100), new Hsv(135, 255, 255));//100-255
            Image<Gray, Byte> white = hsvImage.InRange(new Hsv(0, 0, 230), new Hsv(180, 30, 255));//
            //var resing  = yellow.Or(red).Or(blue);
            //var resimg = white;
            //var resimg = blue;
            return yellow.Or(red).Or(blue);
        }

        /// <summary>
        /// Подсчитать отношение кол-ва белых пикселей к суммарному кол-ву пикселей изображения
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static double GetPropBGR(Image<Gray, Byte> img)
        {
            var chan = img.CountNonzero();
            return (double)chan[0] / (img.Width * img.Height);
        }
    }
}
