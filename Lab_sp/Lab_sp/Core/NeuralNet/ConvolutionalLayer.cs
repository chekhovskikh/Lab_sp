using Recognition.NeuralUtils;

namespace Recognition.NeuralNet
{
    public sealed class ConvolutionalLayer : Layer
    {
        private readonly KernelParams _kernel;
        public readonly FeatureMap[] FeatureMaps;

        public ConvolutionalLayer(int mapsCount, int mapWidth, int mapHeight, int inputsPerNeuron, KernelParams kernel)
        {
            // ������������� �������. ��������� ������ ����, ����� ����� ��������� � �� ��� ���������� ������ �� ������� � ���� ������ ����
            FeatureMaps = new FeatureMap[mapsCount];
            Neurons = new Neuron[mapWidth * mapHeight * mapsCount];
            Weights = new Weight[inputsPerNeuron * mapsCount];

            for (int fi = 0, w = 0, n = 0; fi < FeatureMaps.Length; fi++)
            {
                // ������� ��������� ����� ��������� - 2D ����
                FeatureMaps[fi] = new FeatureMap(mapWidth, mapHeight, inputsPerNeuron);

                // ��������� �� ����� ����� ��������� �������� ������� ���� - ������� � ����
                foreach (var neuron in FeatureMaps[fi].Neurons)
                    Neurons[n++] = neuron;
                foreach (var weight in FeatureMaps[fi].Weights)
                    Weights[w++] = weight;
            }

            _kernel = kernel;
        }

        #region ������ ������������� � ������ �����

        public override void ConnectTo(Layer inputLayer)
        {
            foreach (var featureMap in FeatureMaps)
            {
                featureMap.ConnectTo(inputLayer);
            }
        }

        /// <summary>
        /// ������������ ���������� ������ ����� ��������� �� ������� 2D-�����.
        /// </summary>
        /// <param name="inputLayer">������� ���������� 2D-����.</param>
        public void ConnectTo(Layer2D inputLayer)
        {
            // ������ ����� ��������� ���������� � �������� ����
            foreach (var featureMap in FeatureMaps)
            {
                featureMap.ConnectTo(inputLayer, _kernel);
            }
        }

        /// <summary>
        /// ������������ ���������� ������ ����� ��������� � ������ ������ ��������� �������� ����������� ����.
        /// </summary>
        /// <param name="inputLayer">������� ���������� ����.</param>
        public void ConnectTo(ConvolutionalLayer inputLayer)
        {
            // ������ ����� ��������� ���������� � �������� ����
            foreach (var featureMap in FeatureMaps)
            {
                featureMap.ConnectTo(inputLayer, _kernel);
            }
        }

        #endregion
    }
}