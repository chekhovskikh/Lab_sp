using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_sp
{
    class TrainingImage
    {
        /// <summary>
        /// Изображение для подачи на нейронную сеть
        /// </summary>
        public double[] Input { get; set; }

        /// <summary>
        /// Класс изображения
        /// </summary>
        private int type;

        /// <summary>
        /// Количство классов всего
        /// </summary>
        public int CountTypes { get; set; }

        /// <summary>
        /// Желаемый результат обучения
        /// </summary>
        private double[] desiredOutput;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="data">Изображение</param>
        /// <param name="type">Класс</param>
        /// <param name="countTypes">Общее количество классов</param>
        public TrainingImage(double[] data, int type, int countTypes)
        {
            Input = data;
            CountTypes = countTypes;
            Type = type;
            SetOutput();
        }

        public int Type
        {
            get { return type; }
            set
            {
                if (value >= -1 && value < CountTypes)
                    type = value;
                else throw new ArgumentException();
            }
        }

        public double[] DesiredOutput
        {
            get { return desiredOutput; }
        }

        /// <summary>
        /// Создает желаемый выходной вектор
        /// </summary>
        private void SetOutput()
        {
            desiredOutput = new double[CountTypes];
            for (int i = 0; i < desiredOutput.Length; i++)
                desiredOutput[i] = (i == type) ? 1 : -1;
        }
    }
}
