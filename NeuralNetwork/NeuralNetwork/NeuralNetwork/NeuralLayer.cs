using CudaLibManaged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    /// <summary>
    /// Represents a layer of neurons
    /// </summary>
    class NeuralLayer : IDisposable
    {
        public NeuralLayer(MathOperationManager mathManager, int neuronCount, int batchSize, bool isInputLayer = false, bool isOutputLayer = false)
        {
            this.IsInputLayer = isInputLayer;
            this.IsOutputLayer = isOutputLayer;
            this.Data = mathManager.CreateMatrix(neuronCount, batchSize);
            this.Bias = mathManager.CreateMatrix(ArrayUtilities.GetArrayWithRandomValues(neuronCount), neuronCount);
            this.ErrorGradient = mathManager.CreateMatrix(neuronCount, batchSize);
        }

        public NeuralLayer(MathOperationManager mathManager, float[] biasData, int neuronCount, int batchSize, bool isInputLayer = false, bool isOutputLayer = false)
        {
            this.IsInputLayer = isInputLayer;
            this.IsOutputLayer = isOutputLayer;
            this.Data = mathManager.CreateMatrix(neuronCount, batchSize);
            this.Bias = mathManager.CreateMatrix(biasData, neuronCount);
            this.ErrorGradient = mathManager.CreateMatrix(neuronCount, batchSize);
        }
        /// <summary>
        /// data = f(wx + b)
        /// </summary>
        internal Matrix Data
        {
            get;
            private set;
        }

        internal Matrix Bias
        {
            get;
            private set;
        }
        internal Matrix ErrorGradient
        {
            get;
            private set;
        }
        internal bool IsInputLayer
        {
            get;
            private set;
        }
        internal bool IsOutputLayer
        {
            get;
            private set;
        }

        internal void Resize(int row, int column = 1)
        {
            this.Data.Resize(row, column);
            this.ErrorGradient.Resize(row, column);
        }

        public void Dispose()
        {
            this.Data.Dispose();
            this.Bias.Dispose();
            this.ErrorGradient.Dispose();
        }
    }
}

