using CudaLibManaged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    class DQNNeuralNetworkConfiguration : NeuralNetworkConfiguration
    {
        public float DiscountFactor { get; set; }
        public int QHatRefreshCount { get; set; }
        public DQNNeuralNetworkConfiguration(int inputLayerNeuronsCount, List<int> hiddenLayerNeuronsCountList, int outputLayerNeuronsCount)
            : base(inputLayerNeuronsCount, hiddenLayerNeuronsCountList, outputLayerNeuronsCount)
        {
            this.DiscountFactor = (float)0.9;
            this.QHatRefreshCount = 5000;
        }
    }
    class DQN : NeuralNetwork
    {
        private NeuralNetwork QHat;
        private DQNNeuralNetworkConfiguration DQNConfiguration;
        private int gradientStepCount;

        public DQN(MathOperationManager mathManager, DQNNeuralNetworkConfiguration configuration)
        {
            if (configuration.LossFunction != LossFunctionType.BellmanError)
            {
                throw new ArgumentException("DQN only supports Bellman error. Pls check the configuration passed in.");
            }

            base.CreateNeuralNetwork(mathManager, configuration);

            this.DQNConfiguration = configuration;
            this.gradientStepCount = 0;

            var nnStore = this.CreateNeuralNetworkStore();
            this.QHat = new NeuralNetwork(this.mathManager, nnStore);
        }

        private float DQNTrain(BatchInputWrapper inputBatch)
        {
            var currentState = inputBatch.StateActionRewardNewStateIsLastEpisodeTuple.Item1;
            var chosenActionIndex = inputBatch.StateActionRewardNewStateIsLastEpisodeTuple.Item2;
            var currentReward = inputBatch.StateActionRewardNewStateIsLastEpisodeTuple.Item3;
            var newState = inputBatch.StateActionRewardNewStateIsLastEpisodeTuple.Item4;
            var isLastEpisode = inputBatch.StateActionRewardNewStateIsLastEpisodeTuple.Item5;

            this.VerifyInputVectorDimentionality(currentState, inputBatch.BatchSize);
            this.VerifyInputVectorDimentionality(newState, inputBatch.BatchSize);

            this.CheckAndResizeNetwork(inputBatch.BatchSize);
            this.QHat.CheckAndResizeNetwork(inputBatch.BatchSize);

            Matrix predictedQValues = this.Predict(currentState);
            Matrix QHatValues = this.QHat.Predict(newState);

            float errorAvg = 0;
            this.GetBellmanErrorAndDerivative(predictedQValues, QHatValues, chosenActionIndex, currentReward, ref errorAvg, isLastEpisode);

            this.BackPropogate();

            return errorAvg;
        }

        private void GetBellmanErrorAndDerivative(Matrix predictedQValues, Matrix QHatValues, float[] chosenActionIndices, float[] currentRewards, ref float errorAvg, float[] isLastEpisode)
        {
            using (Matrix chosenActionIndicesMatrix = this.mathManager.CreateMatrix(chosenActionIndices, chosenActionIndices.Count()),
                          currentRewardsMatrix = this.mathManager.CreateMatrix(currentRewards, currentRewards.Count()),
                          isLastEpisodeMatrix = this.mathManager.CreateMatrix(isLastEpisode, isLastEpisode.Count()))
            {
                LossFunctions.BellmanLossAndDerivative(this.mathManager, predictedQValues, QHatValues, chosenActionIndicesMatrix, currentRewardsMatrix, ref errorAvg,
                                                       this.neuralLayers.Last().ErrorGradient, this.DQNConfiguration.DiscountFactor, isLastEpisodeMatrix);
            }
        }

        protected override void TakeGradientDescentStep()
        {
            base.TakeGradientDescentStep();
            this.gradientStepCount++;

            // Updating QHat neural network with the new weights
            if (this.gradientStepCount % this.DQNConfiguration.QHatRefreshCount == 0)
            {
                Console.WriteLine("Updating QHat neural network");
                var nnStore = this.CreateNeuralNetworkStore();
                this.QHat.LoadFromNeuralNetworkStore(nnStore);
                this.gradientStepCount = 0;
            }
        }

        protected override void EvaluateCrossValidationSet(IEnumerable<BatchInputWrapper> cvData)
        {
            float matchPredictRewardAccumulator = 0, nonMatchPredictRewardAccumulator = 0;
            int matchPredictRewardCountAccumulator = 0, nonMatchPredictRewardCountAccumulator = 0;

            foreach (var data in cvData)
            {
                float matchPredictReward = 0, nonMatchPredictReward = 0;
                int matchPredictRewardCount = 0, nonMatchPredictRewardCount = 0;
                var currentState = data.StateActionRewardNewStateIsLastEpisodeTuple.Item1;
                var chosenActionIndices = data.StateActionRewardNewStateIsLastEpisodeTuple.Item2;
                var currentRewards = data.StateActionRewardNewStateIsLastEpisodeTuple.Item3;

                this.VerifyInputVectorDimentionality(currentState, data.BatchSize);
                this.CheckAndResizeNetwork(data.BatchSize);

                Matrix predictedQValues = this.Predict(currentState);
                using (Matrix chosenActionIndicesMatrix = this.mathManager.CreateMatrix(chosenActionIndices, chosenActionIndices.Count()),
                              currentRewardsMatrix = this.mathManager.CreateMatrix(currentRewards, currentRewards.Count()))
                {
                    LossFunctions.DqnStanfordEvaluation(this.mathManager, predictedQValues, chosenActionIndicesMatrix, currentRewardsMatrix, ref matchPredictReward, ref matchPredictRewardCount,
                                                        ref nonMatchPredictReward, ref nonMatchPredictRewardCount);
                }

                matchPredictRewardAccumulator += matchPredictReward;
                matchPredictRewardCountAccumulator += matchPredictRewardCount;
                nonMatchPredictRewardAccumulator += nonMatchPredictReward;
                nonMatchPredictRewardCountAccumulator += nonMatchPredictRewardCount;
            }

            Console.WriteLine("Average predict match reward:{0}", matchPredictRewardAccumulator / matchPredictRewardCountAccumulator);
            Console.WriteLine("Average predict non-match reward:{0}", nonMatchPredictRewardAccumulator / nonMatchPredictRewardCountAccumulator);
        }

        public override void MiniBatchStochasticGradientDescent(IEnumerable<BatchInputWrapper> trainDataBatches, IEnumerable<BatchInputWrapper> cvData)
        {
            this.GenericMiniBatchStochasticGradientDescent(trainDataBatches, this.DQNTrain, cvData);
        }

        public override void Dispose()
        {
            this.QHat.Dispose();
            base.Dispose();
        }
    }
}
