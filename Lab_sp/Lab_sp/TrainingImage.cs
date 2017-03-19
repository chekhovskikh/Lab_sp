using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_sp
{
    class TrainingImage
    {
        private double[] input { get; set; }
        private int type;
        private int countType { get; set; }

        public TrainingImage(double[] data, int type, int countType)
        {
            input = data;
            this.countType = countType;
            Type = type;
        }

        public int Type
        {
            get { return type; }
            set
            {
                if (value >= -1 && value < countType)
                    type = value;
                else throw new ArgumentException();
            }
        }

        public double[] desiredOutput()
        {
            double[] output = new double[countType];
            for (int i = 0; i < output.Length; i++)
                output[i] = (i == type) ? 1 : -1;
            return output;
        }
    }
}
