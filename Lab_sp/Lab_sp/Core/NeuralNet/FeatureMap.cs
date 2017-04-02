using System;
using Recognition.NeuralUtils;

namespace Recognition.NeuralNet
{
    /// <summary>
    /// ����� ���������. �������� 2D �����, ������� �������� ����� ����� ���� (���������� ���������� ������� = ���������� ����� ����)
    /// </summary>
    public sealed class FeatureMap : Layer2D
    {
        /// <summary>
        /// �������������� ����� ����� ��������� � ��������� �������, ������� � ����������� �����.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="weightsCount"></param>
        public FeatureMap(int width, int height, int weightsCount)
            : base(width, height, weightsCount, weightsCount)
        {
        }

        public override void ConnectTo(Layer inputLayer)
        {
            throw new InvalidOperationException("������ ������������ ����� ��������� � ��������� ����.");
        }

        public void ConnectTo(Layer2D inputLayer, KernelParams kernel)
        {
            var inputsCounter = 0;
            ConnectWithoutBiasTo(inputLayer, kernel, ref inputsCounter);
            ConnectLastToBias();
        }

        public void ConnectTo(ConvolutionalLayer inputLayer, KernelParams kernel)
        {
            // ���������� ���������� ����� � ������ ����� �������� ����, 
            var inputsCounter = 0;
            foreach (var featureMap in inputLayer.FeatureMaps)
                ConnectWithoutBiasTo(featureMap, kernel, ref inputsCounter);

            ConnectLastToBias();
        }

        /// <summary>
        /// ������������ ����� � ���������� ������� �������� ��� �����.
        /// ��� ��� ���� ����� ���� ���������, ���������� ����������-������� ����������� �����
        /// </summary>
        /// <param name="inputMap"></param>
        /// <param name="kernel"></param>
        /// <param name="inputsCounter"></param>
        private void ConnectWithoutBiasTo(Layer2D inputMap, KernelParams kernel, ref int inputsCounter)
        {
            // ���������� ����� ����
            for (var mapY = 0; mapY < Height; mapY++)
            {
                for (var mapX = 0; mapX < Width; mapX++)
                {
                    // �������������� ������
                    var neuron = Neurons2D[mapY][mapX];

                    var i = 0;

                    // ������������ ���� 5*5, �������� ����������
                    // ������ ���� ������������ �� 2 ������� ������ � ���� ��� ������ �����
                    for (var kY = mapY*kernel.Step; kY < mapY*kernel.Step + kernel.Height; kY++)
                    {
                        for (var kX = mapX*kernel.Step; kX < mapX*kernel.Step + kernel.Width; kX++)
                        {
                            // ���������� ������ � ����������� ���� ����� ���� �����
                            var inputNeuron = inputMap.Neurons2D[kY][kX];
                            var weight = Weights[inputsCounter + i];
                            neuron.InputConnections[inputsCounter + i] = new InputConnection(inputNeuron, weight);
                            i++;
                        }
                    }
                }
            }

            // ����������� ��� ���� ������� ����
            inputsCounter += kernel.Length;
        }

        /// <summary>
        /// ���������� ��������� ������� ����������� ������� ������� ����� ��� ����
        /// </summary>
        private void ConnectLastToBias()
        {
            // ��������� ���� ��������� ����������� �������
            var lastWeight = Weights[Weights.GetUpperBound(0)];
            foreach (var neuron in Neurons)
            {
                neuron.InputConnections[neuron.InputConnections.GetUpperBound(0)]
                    = new InputConnection(Neuron.Bias, lastWeight);
            }
        }
    }
}