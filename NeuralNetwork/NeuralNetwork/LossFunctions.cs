using CudaLibManaged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public enum LossFunctionType
    {
        CrossEntropyError,
        BellmanError
    }
    public static class LossFunctions
    {
        public static void CrossEntropyErrorAndDerivative(MathOperationManager mathManager, Matrix preSigmoidScores, Matrix trueLabels, Matrix outputErrorDerivative,
                                                          ref float errorAvg)
        {
            using (Matrix sigmoidScores = mathManager.CreateMatrix(preSigmoidScores.Row, preSigmoidScores.Column), outputError = mathManager.CreateMatrix(preSigmoidScores.Row, preSigmoidScores.Column))
            {
                // Calculate sigmoid
                mathManager.GlobalInstance.MatrixSigmoid(preSigmoidScores, sigmoidScores);
                // Get cross entropy error
                mathManager.GlobalInstance.MatrixCrossEntropyError(sigmoidScores, trueLabels, outputError);
                errorAvg = outputError.GetValue().Sum() / preSigmoidScores.Column;
                // calculate error derivative = sigmoidScores - trueLabels + (regularizationParameter * sigmoidScores)
                mathManager.GlobalInstance.MatrixAddition(sigmoidScores, MatrixTransformation.None, 1, trueLabels, MatrixTransformation.None, -1, outputErrorDerivative);
            }
        }

        public static void BellmanLossAndDerivative(MathOperationManager mathManager, Matrix predictedQValues, Matrix QHatValues, Matrix chosenActionIndices, Matrix currentRewards,
                                                    ref float errorAvg, Matrix errorDerivative, float discountFactor, Matrix isLastEpisode)
        {
            using (Matrix maxQHatValues = mathManager.CreateMatrix(predictedQValues.Column), errorMatrix = mathManager.CreateMatrix(predictedQValues.Column))
            {
                // Calculate column-wise max of the QHat values
                mathManager.GlobalInstance.ColumnWiseMax(QHatValues, maxQHatValues);

                // Calculate error & derivative
                mathManager.GlobalInstance.MatrixBellmanErrorAndDerivative(predictedQValues, maxQHatValues, chosenActionIndices, currentRewards, errorMatrix, errorDerivative,
                                                                           discountFactor, isLastEpisode);
                errorAvg = errorMatrix.GetValue().Sum() / predictedQValues.Column;
            }
        }

        public static void DqnStanfordEvaluation(MathOperationManager mathManager, Matrix predictedQValues, Matrix chosenActionIndices, Matrix currentRewards,
                                                 ref float matchPredictRewardSum, ref int matchPredictRewardCount, ref float nonMatchPredictRewardSum, ref int nonMatchPredictRewardCount)
        {
            float emptyIndicatorValue = float.Epsilon;
            using (Matrix predictedActionIndices = mathManager.CreateMatrix(predictedQValues.Column),
                          matchPredictrewardMatrix = mathManager.CreateMatrix(Enumerable.Repeat<float>(emptyIndicatorValue, predictedQValues.Column).ToArray(), predictedQValues.Column),
                          nonMatchPredictrewardMatrix = mathManager.CreateMatrix(Enumerable.Repeat<float>(emptyIndicatorValue, predictedQValues.Column).ToArray(), predictedQValues.Column))
            {
                mathManager.GlobalInstance.ColumnWiseMaxIndex(predictedQValues, predictedActionIndices);
                mathManager.GlobalInstance.DqnStanfordEvaluation(predictedActionIndices, chosenActionIndices, currentRewards, matchPredictrewardMatrix, nonMatchPredictrewardMatrix);

                var matchPredictRewards = matchPredictrewardMatrix.GetValue();
                matchPredictRewardCount = matchPredictRewards.Count(r => r != emptyIndicatorValue);
                matchPredictRewardSum = matchPredictRewards.Sum();

                var nonMatchPredictRewards = nonMatchPredictrewardMatrix.GetValue();
                nonMatchPredictRewardCount = nonMatchPredictRewards.Count(r => r != emptyIndicatorValue);
                nonMatchPredictRewardSum = nonMatchPredictRewards.Sum();
            }
        }
    }
}
