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

namespace Lab_sp
{
    class Utils
    {
        private Utils() { }

        /// <summary>
        /// Классификатор каскадов
        /// </summary>
        private static CascadeClassifier cascade = new CascadeClassifier(Properties.Resources.haar_plate_cascade);

        /// <summary>
        /// Задает каскад совпадений
        /// </summary>
        /// <param name="path">Путь к файлу каскада Хаара</param>
        public static void SetCascade(string path)
        {
            try { cascade = new CascadeClassifier(path); }
            catch { cascade = new CascadeClassifier(Properties.Resources.haar_plate_cascade); }
        }

        /// <summary>
        /// Обнаруживает все объекты,
        /// которые распознает каскад Хаара
        /// </summary>
        /// <param name="image">Изображение для распознования</param>
        /// <returns>Координаты рамок, соответствующие каскаду Хаара</returns>
        public static Rectangle[] HaarDetect(Mat image)
        {
            return cascade.DetectMultiScale(image);
        }
    }
}
