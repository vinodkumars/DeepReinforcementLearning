using CudaLibManaged;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace NeuralNetwork
{
    class NeuralNetwork : IDisposable
    {
        protected List<NeuralLayer> neuralLayers;
        protected MathOperationManager mathManager;
        protected NeuralNetworkConfiguration configuration;
        protected int currentEpoch;
        protected int currentBatchSize;
        private Matrix biasGradientAccumulator;

        protected NeuralNetwork()
        { }

        public NeuralNetwork(MathOperationManager mathManager, NeuralNetworkConfiguration configuration)
        {
            this.CreateNeuralNetwork(mathManager, configuration);
        }

        protected void CreateNeuralNetwork(MathOperationManager mathManager, NeuralNetworkConfiguration configuration)
        {
            this.mathManager = mathManager;
            this.configuration = configuration;
            this.currentEpoch = 0;
            this.currentBatchSize = NeuralNetworkConfiguration.defaultBatchSize;
            this.neuralLayers = new List<NeuralLayer>();
            this.biasGradientAccumulator = this.mathManager.CreateMatrix(Enumerable.Repeat<float>(1, this.currentBatchSize).ToArray(), this.currentBatchSize);
            this.CreateNetworkArchitecture();
        }

        protected virtual void CreateNetworkArchitecture()
        {
            // First layer is the input layer, last layer is the output layer and the rest are hidden layers.
            this.neuralLayers.Add(new NeuralLayer(mathManager, configuration.InputLayerNeuronsCount, this.currentBatchSize, this.configuration.Activation, true));
            configuration.HiddenLayerNeuronsCountList.ForEach(n => this.neuralLayers.Add(new NeuralLayer(mathManager, n, this.currentBatchSize, this.configuration.Activation)));
            this.neuralLayers.Add(new NeuralLayer(mathManager, configuration.OutputLayerNeuronsCount, this.currentBatchSize, NeuronActivationType.None, false, true));

            for (int i = 0; i < this.neuralLayers.Count - 1; i++)
            {
                this.neuralLayers.ElementAt(i).SetLayerOut(this.neuralLayers.ElementAt(i + 1));
            }
        }

        public NeuralNetwork(MathOperationManager mathManager, string modelPath)
        {
            NeuralNetworkStore nnStore = LoadModel(modelPath);
            this.CreateNeuralNetwork(mathManager, nnStore.Configuration);
            this.LoadFromNeuralNetworkStore(nnStore);
        }

        public NeuralNetwork(MathOperationManager mathManager, NeuralNetworkStore nnStore)
        {
            this.CreateNeuralNetwork(mathManager, nnStore.Configuration);
            this.LoadFromNeuralNetworkStore(nnStore);
        }

        internal void LoadFromNeuralNetworkStore(NeuralNetworkStore nnStore)
        {
            // First layer is the input layer, last layer is the output layer and the rest are hidden layers.
            Debug.Assert(this.neuralLayers.Count == nnStore.BiasDataList.Count());
            for (int i = 0; i < this.neuralLayers.Count; i++)
            {
                var layer = this.neuralLayers.ElementAt(i);
                layer.Bias.SetValue(nnStore.BiasDataList.ElementAt(i));
                if (!layer.IsOutputLayer)
                {
                    layer.NeuralLink.Weights.SetValue(nnStore.MatrixDataList.ElementAt(i));
                }
            }
        }

        protected void ForwardPropogate(float[] inputData, bool skipMajorConversion = true)
        {
            // Setting the data of the input layer.
            this.neuralLayers.First().Data.SetValue(inputData, skipMajorConversion);

            for (int i = 0; i < this.neuralLayers.Count - 1; i++)
            {
                var layer = this.neuralLayers[i];
                layer.ForwardPropogate();
            }
        }

        protected void BackPropogate()
        {
            for (int i = this.neuralLayers.Count - 1; i > 0; i--)
            {
                var layer = this.neuralLayers[i];
                layer.BackPropogate();
            }
        }

        protected virtual void TakeGradientDescentStep()
        {
            IncorporateGradients();
        }

        private void IncorporateGradients()
        {
            float stepSizeFactor = (-1 * this.configuration.StepSize) / this.currentBatchSize;
            this.neuralLayers.ForEach(l => l.IncorporateGradient(stepSizeFactor, this.biasGradientAccumulator));
        }

        private void GetErrorAndDerivative(Matrix preSigmoidScores, float[] trueLabels, ref float errorAvg)
        {
            using (Matrix TrueLabelsMatrix = this.mathManager.CreateMatrix(trueLabels, this.neuralLayers.Last().Data.Row, this.neuralLayers.Last().Data.Column))
            {
                switch (this.configuration.LossFunction)
                {
                    case LossFunctionType.CrossEntropyError:
                        LossFunctions.CrossEntropyErrorAndDerivative(this.mathManager, preSigmoidScores, TrueLabelsMatrix, this.neuralLayers.Last().ErrorGradient, ref errorAvg);
                        break;
                }
            }
        }

        public void CheckAndResizeNetwork(int batchSize)
        {
            if (this.currentBatchSize != batchSize)
            {
                this.currentBatchSize = batchSize;
                this.neuralLayers.ForEach(l => l.Resize(l.Data.Row, batchSize));
                this.biasGradientAccumulator.Resize(Enumerable.Repeat<float>(1, this.currentBatchSize).ToArray(), this.currentBatchSize);
            }
        }

        /// <summary>
        /// Calculates (J(theta+h)-J(theta-h))/2h to manually check the gradients
        /// </summary>
        /// <param name="input"></param>
        /// <param name="trueLabels"></param>
        /// <param name="oldError"></param>
        private void CheckGradients(float[] input, float[] trueLabels, float oldError)
        {
            // Note: This function is just a hack to verify the gradients during implementaion and not a production quality code.
            var neuralLink = this.neuralLayers.ElementAt(1).NeuralLink;
            int index = 0;
            float[] oldGradientVector = null;
            oldGradientVector = neuralLink.ErrorGradient.GetValue();
            //using (Matrix tmpMatrix = this.mathManager.CreateMatrix(Enumerable.Repeat<float>(1, neuralLayer.ErrorGradient.Column).ToArray(), neuralLayer.ErrorGradient.Column),
            //       result = this.mathManager.CreateMatrix(neuralLayer.ErrorGradient.Row))
            //{
            //    this.mathManager.GlobalInstance.MatrixMultiplication(neuralLayer.ErrorGradient, MatrixTransformation.None, 1, tmpMatrix, MatrixTransformation.None, result, 0);
            //    oldGradientVector = result.GetValue();
            //}
            float oldGradient = oldGradientVector[index];
            var oldOutput = this.neuralLayers.Last().Data.GetValue();
            float h = (float)0.001;

            var tmp = neuralLink.Weights.GetValue();
            tmp[index] += h;
            neuralLink.Weights.SetValue(tmp);
            var newOutput = this.Predict(input);
            float hPlusError = 0;
            this.GetErrorAndDerivative(this.neuralLayers.Last().Data, trueLabels, ref hPlusError);
            tmp = neuralLink.Weights.GetValue();
            tmp[index] -= h;
            neuralLink.Weights.SetValue(tmp);

            tmp = neuralLink.Weights.GetValue();
            tmp[index] -= h;
            neuralLink.Weights.SetValue(tmp);
            newOutput = this.Predict(input);
            float hMinusError = 0;
            this.GetErrorAndDerivative(this.neuralLayers.Last().Data, trueLabels, ref hMinusError);
            tmp = neuralLink.Weights.GetValue();
            tmp[index] += h;
            neuralLink.Weights.SetValue(tmp);

            float newGradient = (hPlusError - hMinusError) / (2 * h);
            // Manually compare if newGradient & oldGradient are reasonaly equal
            // Note that oldGradient = newGradient * batchSize
        }

        /// <summary>
        /// Trains the neural network on a single input x and output y
        /// </summary>
        /// <param name="inputBatch"></param>
        /// <param name="trueLabel"></param>
        /// <returns>Error from the network</returns>
        private float Train(BatchInputWrapper inputBatch)
        {
            var inputLayerData = inputBatch.InputAndTrueLabelTuple.Item1;
            var trueLabel = inputBatch.InputAndTrueLabelTuple.Item2;

            this.VerifyInputVectorDimentionality(inputLayerData, inputBatch.BatchSize);
            this.VerifyTrueLabelVectorDimentionality(trueLabel, inputBatch.BatchSize);

            this.CheckAndResizeNetwork(inputBatch.BatchSize);
            this.ForwardPropogate(inputLayerData);

            float error = 0;
            this.GetErrorAndDerivative(this.neuralLayers.Last().Data, trueLabel, ref error);

            this.BackPropogate();
            //this.CheckGradients(inputLayerData, trueLabel, error);

            return error;
        }

        protected void VerifyInputVectorDimentionality(float[] input, int batchSize)
        {
            if (input.Length != this.neuralLayers.First().Data.Row * batchSize)
            {
                throw new ArgumentException("input's dimentionality != input-layer's dimentionality");
            }
        }

        protected void VerifyTrueLabelVectorDimentionality(float[] trueLabel, int batchSize)
        {
            if (trueLabel.Length != this.neuralLayers.Last().Data.Row * batchSize)
            {
                throw new ArgumentException("true label's dimentionality != output-layer's dimentionality");
            }
        }

        private float previousAvgError = 0;
        private void PrintEpochStatus(int epoch, float avgError, DateTime startTime)
        {
            if (epoch == 1)
            {
                Console.WriteLine("Epoch:{0,3}\tAverageError={1}\tTime:{2}", epoch, avgError, DateTime.Now - startTime);
            }
            else
            {
                Console.WriteLine("Epoch:{0,3}\tAverageError={1} ({2}%)\tTime:{3}", epoch, avgError, (avgError - this.previousAvgError) * 100 / this.previousAvgError, (DateTime.Now - startTime));
            }
            this.previousAvgError = avgError;
        }

        public Matrix Predict(float[] input)
        {
            this.ForwardPropogate(input);
            return this.neuralLayers.Last().Data;
        }

        protected virtual void EvaluateCrossValidationSet(IEnumerable<BatchInputWrapper> cvData)
        {
            int correctPredictionCount = 0, totalPredictions = 0;
            foreach (var data in cvData)
            {
                var inputLayerData = data.InputAndTrueLabelTuple.Item1;
                var trueLabel = data.InputAndTrueLabelTuple.Item2;

                this.VerifyInputVectorDimentionality(inputLayerData, data.BatchSize);
                this.VerifyTrueLabelVectorDimentionality(trueLabel, data.BatchSize);

                this.CheckAndResizeNetwork(data.BatchSize);
                Matrix predictionOutputLayer = this.Predict(inputLayerData);
                totalPredictions += predictionOutputLayer.Column;

                using (Matrix trueLabelMatrix = this.mathManager.CreateMatrix(trueLabel, predictionOutputLayer.Row, predictionOutputLayer.Column),
                              trueLabels = this.mathManager.CreateMatrix(predictionOutputLayer.Column),
                             predictions = this.mathManager.CreateMatrix(predictionOutputLayer.Column))
                {
                    this.mathManager.GlobalInstance.ColumnWiseMaxIndex(predictionOutputLayer, predictions);
                    this.mathManager.GlobalInstance.ColumnWiseMaxIndex(trueLabelMatrix, trueLabels);
                    this.mathManager.GlobalInstance.MatrixAddition(predictions, MatrixTransformation.None, 1, trueLabels, MatrixTransformation.None, -1, predictions);
                    correctPredictionCount += predictions.GetValue().Count(v => v == 0);
                }
            }

            Console.WriteLine("Accuracy:{0}", (float)correctPredictionCount / (float)totalPredictions);
        }

        protected void GenericMiniBatchStochasticGradientDescent(IEnumerable<BatchInputWrapper> trainDataBatches, Func<BatchInputWrapper, float> trainFunction, IEnumerable<BatchInputWrapper> cvData)
        {
            int trainDataCount = 0;
            foreach (var inputBatch in trainDataBatches)
            {
                trainDataCount += inputBatch.BatchSize;
            }

            for (this.currentEpoch = 1; this.currentEpoch <= this.configuration.Epochs; this.currentEpoch++)
            {
                float totalError = 0;
                DateTime iterationStartTime = DateTime.Now, epochStartTime = DateTime.Now;
                for (int i = 0; i < trainDataBatches.Count(); i++)
                {
                    totalError += trainFunction(trainDataBatches.ElementAt(i));
                    this.TakeGradientDescentStep();
                }

                this.PrintEpochStatus(this.currentEpoch, totalError / trainDataCount, epochStartTime);
                this.EvaluateCrossValidationSet(cvData);
                Console.WriteLine();
            }
        }

        public virtual void MiniBatchStochasticGradientDescent(IEnumerable<BatchInputWrapper> trainDataBatches, IEnumerable<BatchInputWrapper> cvData)
        {
            this.GenericMiniBatchStochasticGradientDescent(trainDataBatches, this.Train, cvData);
        }

        public void SaveModel(string modelPath)
        {
            NeuralNetworkStore nnStore = CreateNeuralNetworkStore();
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(modelPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, nnStore);
            }
        }

        protected NeuralNetworkStore CreateNeuralNetworkStore()
        {
            List<float[]> matrixDataList = new List<float[]>();
            List<float[]> biasDataList = new List<float[]>();

            this.neuralLayers.ForEach(l =>
            {
                biasDataList.Add(l.Bias.GetValue());
                if (!l.IsOutputLayer)
                {
                    matrixDataList.Add(l.NeuralLink.Weights.GetValue());
                }
            });

            NeuralNetworkStore nnStore = new NeuralNetworkStore(this.configuration, matrixDataList.AsEnumerable(), biasDataList.AsEnumerable());
            return nnStore;
        }

        private static NeuralNetworkStore LoadModel(string modelPath)
        {
            NeuralNetworkStore nnStore;
            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                nnStore = (NeuralNetworkStore)formatter.Deserialize(stream);
            }
            return nnStore;
        }

        public virtual void Dispose()
        {
            this.neuralLayers.ForEach(l => l.Dispose());
            this.biasGradientAccumulator.Dispose();
        }
    }
}
