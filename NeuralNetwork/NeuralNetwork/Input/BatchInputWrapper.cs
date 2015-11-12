using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public class BatchInputWrapper
    {
        public const int MaxBatchSize = 1024;
        public int BatchSize { get; private set; }
        public bool IsFull
        {
            get { return this.BatchSize >= MaxBatchSize; }
        }
        public Tuple<float[], float[]> InputAndTrueLabelTuple { get; private set; }
        private float[] inputs, trueLabels;
        private int inputDimension, trueLabelsDimension;
        private bool isSealed;

        public Tuple<float[], float[], float[], float[], float[]> StateActionRewardNewStateIsLastEpisodeTuple { get; private set; }
        private float[] state, action, reward, newState, isLastEpisode;
        private int stateDimension;

        public BatchInputWrapper(int stateDimension)
        {
            this.BatchSize = 0;
            this.isSealed = false;

            this.state = new float[stateDimension * MaxBatchSize];
            this.action = new float[MaxBatchSize];
            this.reward = new float[MaxBatchSize];
            this.newState = new float[stateDimension * MaxBatchSize];
            this.isLastEpisode = new float[MaxBatchSize];
            this.stateDimension = stateDimension;

            this.InputAndTrueLabelTuple = null;
            this.inputs = null;
            this.trueLabels = null;
            this.inputDimension = 0;
            this.trueLabelsDimension = 0;
        }

        public BatchInputWrapper(int inputDimension, int trueLabelsDimension)
        {
            this.BatchSize = 0;
            this.isSealed = false;

            this.inputs = new float[inputDimension * MaxBatchSize];
            this.trueLabels = new float[trueLabelsDimension * MaxBatchSize];
            this.inputDimension = inputDimension;
            this.trueLabelsDimension = trueLabelsDimension;

            this.StateActionRewardNewStateIsLastEpisodeTuple = null;
            this.state = null;
            this.action = null;
            this.reward = null;
            this.newState = null;
            this.isLastEpisode = null;
            this.stateDimension = 0;
        }

        public void AddInputAndTrueLabelData(float[] inputs, float[] trueLabels)
        {
            CheckBatchSizeAndIsSealed();

            inputs.CopyTo(this.inputs, this.BatchSize * this.inputDimension);
            trueLabels.CopyTo(this.trueLabels, this.BatchSize * this.trueLabelsDimension);

            this.BatchSize = this.BatchSize + 1;
        }

        public void AddStateActionRewardNewStateIsLastEpisodeData(float[] state, float action, float reward, float[] newState, float isLastEpisode)
        {
            CheckBatchSizeAndIsSealed();

            state.CopyTo(this.state, this.BatchSize * this.stateDimension);
            this.action[this.BatchSize] = action;
            this.reward[this.BatchSize] = reward;
            newState.CopyTo(this.newState, this.BatchSize * this.stateDimension);
            this.isLastEpisode[this.BatchSize] = isLastEpisode;

            this.BatchSize = this.BatchSize + 1;
        }

        public void SealBatch()
        {
            if (inputDimension == 0)
            {
                if (this.BatchSize != MaxBatchSize)
                {
                    var tmp = this.state;
                    this.state = new float[this.stateDimension * this.BatchSize];
                    Array.ConstrainedCopy(tmp, 0, this.state, 0, this.state.Length);

                    tmp = this.action;
                    this.action = new float[this.BatchSize];
                    Array.ConstrainedCopy(tmp, 0, this.action, 0, this.action.Length);

                    tmp = this.reward;
                    this.reward = new float[this.BatchSize];
                    Array.ConstrainedCopy(tmp, 0, this.reward, 0, this.reward.Length);

                    tmp = this.newState;
                    this.newState = new float[this.stateDimension * this.BatchSize];
                    Array.ConstrainedCopy(tmp, 0, this.newState, 0, this.newState.Length);

                    tmp = this.isLastEpisode;
                    this.isLastEpisode = new float[this.BatchSize];
                    Array.ConstrainedCopy(tmp, 0, this.isLastEpisode, 0, this.isLastEpisode.Length);
                }

                this.StateActionRewardNewStateIsLastEpisodeTuple = Tuple.Create<float[], float[], float[], float[], float[]>(this.state, this.action, this.reward, this.newState, this.isLastEpisode);
            }
            else if (this.stateDimension == 0)
            {
                if (this.BatchSize != MaxBatchSize)
                {
                    var tmp = this.inputs;
                    this.inputs = new float[this.inputDimension * this.BatchSize];
                    Array.ConstrainedCopy(tmp, 0, this.inputs, 0, this.inputs.Length);

                    tmp = this.trueLabels;
                    this.trueLabels = new float[this.trueLabelsDimension * this.BatchSize];
                    Array.ConstrainedCopy(tmp, 0, this.trueLabels, 0, this.trueLabels.Length);
                }

                this.InputAndTrueLabelTuple = Tuple.Create<float[], float[]>(this.inputs, this.trueLabels);
            }
            this.isSealed = true;
        }

        private void CheckBatchSizeAndIsSealed()
        {
            if (this.BatchSize == MaxBatchSize)
            {
                throw new ArgumentException("Max batch size reached");
            }
            if(this.isSealed)
            {
                throw new ArgumentException("Batch is already sealed!");
            }
        }

    }
}
