﻿using System;
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
using System.IO;
using System.Drawing.Imaging;

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
        public static Bitmap Resize(this Bitmap source, Size size)
        {
            Image<Bgr, Byte> captureImage = new Image<Bgr, byte>(source);
            return captureImage.Resize(size.Width, size.Height, Inter.Linear).ToBitmap();
        }

        /// <summary>
        /// Создание битмапа с ресайзом
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="size">Размер выходного файла</param>
        /// <returns>Изображение с заданным размером</returns>
        public static Bitmap CreateBitmap(Image image, Size size)
        {
            Bitmap bmp = new Bitmap(image);
            if (bmp.Height != size.Height || bmp.Width != size.Width)
                bmp = bmp.Resize(size);
            return bmp;
        }

        /// <summary>
        /// Создание битмапа с ресайзом
        /// </summary>
        /// <param name="path">Путь к изображению</param>
        /// <param name="size">Размер выходного файла</param>
        /// <returns>Изображение с заданным размером</returns>
        public static Bitmap CreateBitmap(string path, Size size)
        {
            Bitmap bmp = new Bitmap(path);
            if (bmp.Height != size.Height || bmp.Width != size.Width)
                bmp = bmp.Resize(size);
            return bmp;
        }

        /// <summary>
        /// Создает Bitmap из массива байт
        /// </summary>
        /// <param name="data">Массив</param>
        /// <returns>Изображение</returns>
        public static Bitmap BytesToBitmap(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data, 0, data.Length))
            {
                ms.Write(data, 0, data.Length);
                return new Bitmap(ms);
            }
        }

        /// <summary>
        /// Создает массива байт из Bitmap
        /// </summary>
        /// <param name="bmp">Изображение</param>
        /// <returns>Массив</returns>
        public static byte[] BitmapToBytes(Image img)
        {
            Bitmap bmp = new Bitmap(img);
            using (MemoryStream ms = new MemoryStream())
            {
                bmp.Save(ms, ImageFormat.Bmp);
                byte[] imageBytes = ms.ToArray();
                return imageBytes;
            }
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
        public static double[] GetData(this Bitmap bmp)
        {
            Image<Bgr, byte> image = new Image<Bgr, byte>(bmp);
            return GetRGB(image.Mat.GetData());
        }

        /// <summary>
        /// Перевод массива byte[] -> double[] с преобразованием 
        /// представления цвета одним числом
        /// </summary>
        /// <param name="bytes">Массив для конвектирования</param>
        /// <returns>Конвектированный массив</returns>

        public static double[] GetRGB(byte[] bytes)
        {
            double[] doubles = new double[bytes.Length / 3];
            for (int i = 0; i < doubles.Length; i++)
            {
                byte red = bytes[3 * i + 1];
                byte green = bytes[3 * i + 2];
                byte blue = bytes[3 * i];
                int color = (red << 16) | (green << 8) | blue;
                doubles[i] = color;
            }
            return doubles;
        }

        /// <summary>
        /// Перегрузка функции - на входе Bitmap, а не Mat
        /// Получает массив байтов бинарного изображения
        /// пикселей входящих в облать заданного прямоугольника изображения
        /// </summary>
        /// <param name="image"></param>
        /// <param name="rectangle"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static byte[] GetBinaryData(this Bitmap bmp)
        {
            Image<Gray, Byte> grayImage = new Image<Gray, Byte>(bmp);
            CvInvoke.Threshold(grayImage, grayImage, 127, 255, ThresholdType.Binary);
            return grayImage.Mat.GetData();
        }

        /// <summary>
        /// Перевод массива byte[] -> double[]
        /// </summary>
        /// <param name="bytes">Массив для конвектирования</param>
        /// <returns>Конвектированный массив</returns>
        public static double[] ConvertToDoubles(byte[] bytes)
        {
            double[] doubles = new double[bytes.Length];
            for (int i = 0; i < doubles.Length; i++)
            {
                /*byte red = bytes[3 * i + 1];
                byte green = bytes[3 * i + 2];
                byte blue = bytes[3 * i];*/
                doubles[i] = (bytes[i] == 0) ? 0 : 1;
            }
            return doubles;
        }

        /// <summary>
        /// Получает масштабированный прямоугольник, вырезанных из изображения
        /// пикселей входящих в облать заданного прямоугольника изображения
        /// </summary>
        /// <param name="image">изображение</param>
        /// <param name="rectangle">рассматриваемая область изображения</param>
        /// <param name="size">размер для ресайза</param>
        /// <returns>масштабированный прямоугольник</returns>
        public static Bitmap ResizedRectangle(Mat image, Rectangle rectangle, Size size)
        {
            Mat objectFromFrame = new Mat(image, rectangle);
            Mat resizeObject = new Mat();
            CvInvoke.Resize(objectFromFrame, resizeObject, size);
            return resizeObject.Bitmap;
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
