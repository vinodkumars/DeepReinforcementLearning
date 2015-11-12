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
        private MathOperationManager mathManager;
        public NeuralLayer(MathOperationManager mathManager, int neuronCount, int batchSize, NeuronActivationType activation, bool isInputLayer = false, bool isOutputLayer = false)
        {
            this.mathManager = mathManager;
            this.IsInputLayer = isInputLayer;
            this.IsOutputLayer = isOutputLayer;
            this.NeuralLink = null;
            this.LayerIn = null;
            this.LayerOut = null;
            this.Activation = activation;
            this.Data = mathManager.CreateMatrix(neuronCount, batchSize);
            this.Bias = mathManager.CreateMatrix(ArrayUtilities.GetArrayWithRandomValues(neuronCount), neuronCount);
            this.ErrorGradient = mathManager.CreateMatrix(neuronCount, batchSize);
        }

        public NeuralLayer(MathOperationManager mathManager, float[] biasData, int neuronCount, int batchSize, NeuronActivationType activation, bool isInputLayer = false, bool isOutputLayer = false)
        {
            this.mathManager = mathManager;
            this.IsInputLayer = isInputLayer;
            this.IsOutputLayer = isOutputLayer;
            this.NeuralLink = null;
            this.LayerIn = null;
            this.LayerOut = null;
            this.Activation = activation;
            this.Data = mathManager.CreateMatrix(neuronCount, batchSize);
            this.Bias = mathManager.CreateMatrix(biasData, neuronCount);
            this.ErrorGradient = mathManager.CreateMatrix(neuronCount, batchSize);
        }
        internal NeuronActivationType Activation
        {
            get;
            private set;
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
        internal NeuralLink NeuralLink
        {
            get;
            private set;
        }
        internal NeuralLayer LayerIn
        {
            get;
            private set;
        }
        internal NeuralLayer LayerOut
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

        internal void SetLayerOut(NeuralLayer layerOut)
        {
            this.LayerOut = layerOut;
            this.LayerOut.LayerIn = this;
            this.NeuralLink = new NeuralLink(this.mathManager, this, this.LayerOut);
        }

        public void ForwardPropogate()
        {
            if (this.IsOutputLayer)
            {
                throw new Exception("Can't forward propogate in the output layer");
            }

            // Wx
            this.mathManager.GlobalInstance.MatrixMultiplication(this.NeuralLink.Weights, MatrixTransformation.None, 1, this.Data, MatrixTransformation.None, this.LayerOut.Data, 0);
            this.LayerOut.AddBias();
            if (!this.LayerOut.IsOutputLayer)
            {
                this.LayerOut.ComputeActivation();
            }
        }

        public void BackPropogate()
        {
            if (this.IsInputLayer)
            {
                throw new Exception("Can't back propogate in the input layer");
            }

            // Calculating dW_l = (dL_l+1).(a_l)^T and accumulate on top of existing values
            this.mathManager.GlobalInstance.MatrixMultiplication(this.ErrorGradient, MatrixTransformation.None, 1, this.LayerIn.Data, MatrixTransformation.Transpose,
                                                                 this.LayerIn.NeuralLink.ErrorGradient, 0);

            if (!this.LayerIn.IsInputLayer)
            {
                this.LayerIn.ComputeErrorGradient();
            }
        }

        public void IncorporateGradient(float stepSizeFactor, Matrix biasGradientAccumulator)
        {
            if (!this.IsInputLayer)
            {
                // We multiply l.ErrorGradient (layerSize*batchSize) with biasGradientAccumulator (batchSize*1) to get a sum of gradients vector (layerSize*1)
                // We then add this sum of gradients vector to l.Bias
                this.mathManager.GlobalInstance.MatrixMultiplication(this.ErrorGradient, MatrixTransformation.None, stepSizeFactor, biasGradientAccumulator, MatrixTransformation.None, this.Bias, 1);
            }
            if (!this.IsOutputLayer)
            {
                this.mathManager.GlobalInstance.MatrixAddition(this.NeuralLink.Weights, MatrixTransformation.None, 1, this.NeuralLink.ErrorGradient, MatrixTransformation.None, stepSizeFactor,
                                                               this.NeuralLink.Weights);
            }

        }

        protected void AddBias()
        {
            // Wx + b
            // TODO: Do we add bias in the output layer?
            this.mathManager.GlobalInstance.MatrixPlusVector(this.Data, this.Bias, this.Data);
        }

        protected void ComputeActivation()
        {
            // f(Wx + b)
            switch (this.Activation)
            {
                case NeuronActivationType.ReLu:
                    this.mathManager.GlobalInstance.MatrixReLu(this.Data, this.Data);
                    break;
                case NeuronActivationType.Tanh:
                    this.mathManager.GlobalInstance.MatrixTanh(this.Data, this.Data);
                    break;
                case NeuronActivationType.Sigmoid:
                    this.mathManager.GlobalInstance.MatrixSigmoid(this.Data, this.Data);
                    break;
                case NeuronActivationType.None:
                    break;
            }
        }

        protected void ComputeErrorGradient()
        {
            // Calculating dL_l, the error gradient at layer l
            // dL_l = (W_l)^T.(dL_l+1)
            this.mathManager.GlobalInstance.MatrixMultiplication(this.NeuralLink.Weights, MatrixTransformation.Transpose, 1, this.LayerOut.ErrorGradient, MatrixTransformation.None,
                                                                 this.ErrorGradient, 0);
            // dL_l = HadamardProduct(dL_l, f'(z_l)). Note that LayerIn.Data already has the f(z_l) in it. We reuse them to calculate f'(z_l)
            switch (this.LayerIn.Activation)
            {
                case NeuronActivationType.ReLu:
                    this.mathManager.GlobalInstance.MatrixIncorporateReLuDerivative(this.ErrorGradient, this.Data, this.ErrorGradient);
                    break;
                case NeuronActivationType.Tanh:
                    this.mathManager.GlobalInstance.MatrixIncorporateTanhDerivative(this.ErrorGradient, this.Data, this.ErrorGradient);
                    break;
                case NeuronActivationType.Sigmoid:
                    this.mathManager.GlobalInstance.MatrixIncorporateSigmoidDerivative(this.ErrorGradient, this.Data, this.ErrorGradient);
                    break;
                case NeuronActivationType.None:
                    break;
            }
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
            if (!this.IsOutputLayer)
            {
                this.NeuralLink.Dispose();
            }
        }
    }
}

