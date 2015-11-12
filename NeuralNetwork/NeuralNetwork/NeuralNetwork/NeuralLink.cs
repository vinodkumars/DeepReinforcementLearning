using CudaLibManaged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    /// <summary>
    /// Represents the links between two neuron layers
    /// </summary>
    class NeuralLink : IDisposable
    {
        public NeuralLink(MathOperationManager mathManager, NeuralLayer layerIn, NeuralLayer layerOut)
        {
            this.Weights = mathManager.CreateMatrix(ArrayUtilities.GetArrayWithRandomValues(layerOut.Data.Row * layerIn.Data.Row), layerOut.Data.Row, layerIn.Data.Row);
            this.ErrorGradient = mathManager.CreateMatrix(layerOut.Data.Row, layerIn.Data.Row);
        }

        public NeuralLink(MathOperationManager mathManager, float[] matrixData, NeuralLayer layerIn, NeuralLayer layerOut)
        {
            this.Weights = mathManager.CreateMatrix(matrixData, layerOut.Data.Row, layerIn.Data.Row);
            this.ErrorGradient = mathManager.CreateMatrix(layerOut.Data.Row, layerIn.Data.Row);
        }

        internal Matrix Weights
        {
            get;
            private set;
        }

        internal Matrix ErrorGradient
        {
            get;
            private set;
        }

        public void Dispose()
        {
            this.Weights.Dispose();
            this.ErrorGradient.Dispose();
        }
    }
}
