using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    [Serializable]
    public enum NeuronActivationType
    {
        Tanh,
        ReLu,
        Sigmoid,
        None
    };
    [Serializable]
    public class NeuralNetworkConfiguration
    {
        public const int defaultBatchSize = 1024;
        public int InputLayerNeuronsCount { get; set; }
        public int OutputLayerNeuronsCount { get; set; }
        public List<int> HiddenLayerNeuronsCountList { get; set; }
        public NeuronActivationType Activation { get; set; }
        public LossFunctionType LossFunction { get; set; }
        public float StepSize { get; set; }
        public int Epochs { get; set; }

        public NeuralNetworkConfiguration()
        {
            this.SetNetworkParameters();
        }

        public NeuralNetworkConfiguration(int inputLayerNeuronsCount, List<int> hiddenLayerNeuronsCountList, int outputLayerNeuronsCount)
        {
            this.SetNeuralLayersData(inputLayerNeuronsCount, hiddenLayerNeuronsCountList, outputLayerNeuronsCount);
            this.SetNetworkParameters();
        }

        private void SetNetworkParameters()
        {
            this.Activation = NeuronActivationType.Sigmoid;
            this.LossFunction = LossFunctionType.CrossEntropyError;
            this.StepSize = (float)0.1;
            this.Epochs = 100;
        }

        public void SetNeuralLayersData(int inputLayerNeuronsCount, List<int> hiddenLayerNeuronsCountList, int outputLayerNeuronsCount)
        {
            if (inputLayerNeuronsCount <= 0 || outputLayerNeuronsCount <= 0)
            {
                throw new ArgumentException("Either inputLayerNeuronCount or outputLayerNeuronCount is <= 0");
            }

            this.InputLayerNeuronsCount = inputLayerNeuronsCount > 0 ? inputLayerNeuronsCount : 1;
            this.OutputLayerNeuronsCount = outputLayerNeuronsCount > 0 ? outputLayerNeuronsCount : 1;
            this.HiddenLayerNeuronsCountList = new List<int>();
            if (hiddenLayerNeuronsCountList != null && hiddenLayerNeuronsCountList.All(n => n > 0))
            {
                this.HiddenLayerNeuronsCountList.AddRange(hiddenLayerNeuronsCountList);
            }
        }
    }
    [Serializable]
    class NeuralNetworkStore
    {
        public NeuralNetworkConfiguration Configuration;
        public IEnumerable<float[]> MatrixDataList;
        public IEnumerable<float[]> BiasDataList;

        public NeuralNetworkStore(NeuralNetworkConfiguration configuration, IEnumerable<float[]> matrixDataList, IEnumerable<float[]> biasDataList)
        {
            this.Configuration = configuration;
            this.MatrixDataList = matrixDataList;
            this.BiasDataList = biasDataList;
        }
    }
}
