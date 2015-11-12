using CudaLibManaged;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    class Program
    {
        static void Main(string[] args)
        {
            //const string modelFile = @"model.bin";
            IEnumerable<BatchInputWrapper> trainData = null;
            IEnumerable<BatchInputWrapper> cvData = null;

            //GetKDDInputData(ref trainData, ref cvData);
            //GetKDDSampleInputData(ref trainData, ref cvData);
            //DQN(trainData, cvData);

            GetMNISTInputData(ref trainData, ref cvData);
            //GetClassifySampleInputData(ref trainData, ref cvData);
            DNN(trainData, cvData);

            Console.ReadKey();
        }

        private static void GetMNISTInputData(ref IEnumerable<BatchInputWrapper> trainData, ref IEnumerable<BatchInputWrapper> cvData)
        {
            const string filename = @"D:\vinods\Data\MNIST\train.csv";
            // label, pixels
            List<Tuple<float[], float[]>> inputData = new List<Tuple<float[], float[]>>();

            using (StreamReader sr = new StreamReader(filename))
            {
                sr.ReadLine();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    var values = line.Split(',');
                    List<float> label = new List<float>();
                    List<float> pixels = new List<float>();
                    for (int i = 0; i < values.Count(); i++)
                    {
                        if (i == 0)
                        {
                            var tmp = Enumerable.Repeat<float>(0, 10);
                            label.AddRange(tmp);
                            label[int.Parse(values.ElementAt(i))] = 1;
                        }
                        else
                        {
                            pixels.Add(float.Parse(values.ElementAt(i)) / (float)255);
                        }
                    }
                    inputData.Add(Tuple.Create<float[], float[]>(label.ToArray(), pixels.ToArray()));
                }
            }

            Random rnd = new Random();
            for (int i = inputData.Count - 1; i >= 0; i--)
            {
                int swapIndex = rnd.Next(i + 1);
                var tmp = inputData.ElementAt(i);
                inputData[i] = inputData.ElementAt(swapIndex);
                inputData[swapIndex] = tmp;
            }

            int cutIndex = (int)(inputData.Count * 0.8);
            int inputDimension = 784, labelDimension = 10;
            // Creating train batches
            BatchInputWrapper currentBatch = new BatchInputWrapper(inputDimension, labelDimension);
            List<BatchInputWrapper> trainDataList = new List<BatchInputWrapper>();
            foreach (var item in inputData.GetRange(0, cutIndex))
            {
                if (!currentBatch.IsFull)
                {
                    currentBatch.AddInputAndTrueLabelData(item.Item2, item.Item1);
                }
                else
                {
                    currentBatch.SealBatch();
                    trainDataList.Add(currentBatch);
                    currentBatch = new BatchInputWrapper(inputDimension, labelDimension);
                    currentBatch.AddInputAndTrueLabelData(item.Item2, item.Item1);
                }
            }
            currentBatch.SealBatch();
            trainDataList.Add(currentBatch);
            trainData = trainDataList.AsEnumerable();

            // Creating test batches
            currentBatch = new BatchInputWrapper(inputDimension, labelDimension);
            List<BatchInputWrapper> testDataList = new List<BatchInputWrapper>();
            foreach (var item in inputData.GetRange(cutIndex, inputData.Count - cutIndex))
            {
                if (!currentBatch.IsFull)
                {
                    currentBatch.AddInputAndTrueLabelData(item.Item2, item.Item1);
                }
                else
                {
                    currentBatch.SealBatch();
                    testDataList.Add(currentBatch);
                    currentBatch = new BatchInputWrapper(inputDimension, labelDimension);
                    currentBatch.AddInputAndTrueLabelData(item.Item2, item.Item1);
                }
            }
            currentBatch.SealBatch();
            testDataList.Add(currentBatch);
            cvData = testDataList.AsEnumerable();
        }

        private static void DNN(IEnumerable<BatchInputWrapper> trainData, IEnumerable<BatchInputWrapper> cvData)
        {
            using (MathOperationManager mathManager = new MathOperationManager(MathType.GPU))
            {
                var hiddenLayers = new List<int>();
                hiddenLayers.Add(100);
                hiddenLayers.Add(100);
                NeuralNetworkConfiguration config = new NeuralNetworkConfiguration(784, hiddenLayers, 10);
                config.Epochs = 100;
                config.StepSize = (float)1.5;
                //config.Activation = NeuronActivationType.ReLu;

                using (NeuralNetwork dnn = new NeuralNetwork(mathManager, config))
                {
                    dnn.MiniBatchStochasticGradientDescent(trainData, cvData);
                }
            }
        }

        private static void GetClassifySampleInputData(ref IEnumerable<BatchInputWrapper> trainData, ref IEnumerable<BatchInputWrapper> cvData)
        {
            List<BatchInputWrapper> trainDataList = new List<BatchInputWrapper>();
            List<BatchInputWrapper> cvDataList = new List<BatchInputWrapper>();

            BatchInputWrapper currentBatch = new BatchInputWrapper(5, 1);
            currentBatch.AddInputAndTrueLabelData(new float[] { 1, 0, 1, 0, 1 }, new float[] { 1 });
            //currentBatch.AddInputAndTrueLabelData(new float[] { 1, 2, 3, 2, 1 }, new float[] { 0 });
            currentBatch.SealBatch();
            trainDataList.Add(currentBatch);
            //currentBatch = new BatchInputWrapper(5, 1);
            //currentBatch.AddInputAndTrueLabelData(new float[] { 1, 2, 0, 2, 1 }, new float[] { 0 });
            //currentBatch.AddInputAndTrueLabelData(new float[] { 1, 0, 3, 0, 1 }, new float[] { 1 });
            //currentBatch.SealBatch();
            //trainDataList.Add(currentBatch);
            trainData = trainDataList.AsEnumerable();

            currentBatch = new BatchInputWrapper(5, 1);
            currentBatch.AddInputAndTrueLabelData(new float[] { 1, 0, 1, 1, 1 }, new float[] { 0 });
            currentBatch.SealBatch();
            cvDataList.Add(currentBatch);
            cvData = cvDataList.AsEnumerable();
        }

        private static void DQN(IEnumerable<BatchInputWrapper> trainData, IEnumerable<BatchInputWrapper> cvData)
        {
            using (MathOperationManager mathManager = new MathOperationManager(MathType.GPU))
            {
                var hiddenLayers = new List<int>();
                hiddenLayers.Add(10);
                hiddenLayers.Add(5);
                DQNNeuralNetworkConfiguration config = new DQNNeuralNetworkConfiguration(5, hiddenLayers, 12);
                config.LossFunction = LossFunctionType.BellmanError;
                config.Epochs = 20;
                config.StepSize = (float)0.1;

                using (DQN nn = new DQN(mathManager, config))
                {
                    nn.MiniBatchStochasticGradientDescent(trainData, cvData);
                }
            }
        }

        private static void GetKDDSampleInputData(ref IEnumerable<BatchInputWrapper> trainData, ref IEnumerable<BatchInputWrapper> cvData)
        {
            List<BatchInputWrapper> trainDataList = new List<BatchInputWrapper>();
            List<BatchInputWrapper> cvDataList = new List<BatchInputWrapper>();

            BatchInputWrapper currentBatch = new BatchInputWrapper(5);
            currentBatch.AddStateActionRewardNewStateIsLastEpisodeData(new float[] { 5, 4, 4, 0, 0 }, 4, 9, new float[] { 5, 4, 4, 1, 1 }, 0);
            currentBatch.AddStateActionRewardNewStateIsLastEpisodeData(new float[] { 5, 4, 4, 1, 1 }, 3, 11, new float[] { 5, 4, 4, 2, 2 }, 1);
            currentBatch.AddStateActionRewardNewStateIsLastEpisodeData(new float[] { 5, 4, 4, 6, 8 }, 0, 11, new float[] { 5, 4, 4, 6, 9 }, 0);
            currentBatch.SealBatch();
            trainDataList.Add(currentBatch);
            trainData = trainDataList.AsEnumerable();

            currentBatch = new BatchInputWrapper(5);
            //currentBatch.AddStateActionRewardNewStateIsLastEpisodeData(new float[] { 5, 4, 4, 0, 0 }, 4, 9, new float[] { 5, 4, 4, 1, 1 }, 0);
            currentBatch.AddStateActionRewardNewStateIsLastEpisodeData(new float[] { 5, 4, 4, 6, 10 }, 4, 10, new float[] { 5, 4, 4, 6, 11 }, 0);
            currentBatch.AddStateActionRewardNewStateIsLastEpisodeData(new float[] { 5, 4, 4, 6, 14 }, 1, 10, new float[] { 5, 4, 4, 6, 15 }, 0);
            currentBatch.SealBatch();
            cvDataList.Add(currentBatch);
            cvData = cvDataList.AsEnumerable();
        }

        static void GetKDDInputData(ref IEnumerable<BatchInputWrapper> trainData, ref IEnumerable<BatchInputWrapper> cvData)
        {
            const string filename = @"D:\vinods\Data\Donation\lrn_data_220k.txt";
            List<Tuple<float[], float, float, float[], float>> formattedData = new List<Tuple<float[], float, float, float[], float>>();
            using (StreamReader sr = new StreamReader(filename))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] data = line.Split('\t');
                    var oldState = new float[] { float.Parse(data[2]), float.Parse(data[3]), float.Parse(data[4]), float.Parse(data[5]), float.Parse(data[6]) };
                    var actionIndex = float.Parse(data[7]);
                    var reward = float.Parse(data[8]);
                    var newState = new float[] { float.Parse(data[9]), float.Parse(data[10]), float.Parse(data[11]), float.Parse(data[12]), float.Parse(data[13]) };
                    var isLastEpisode = string.Equals(data[1], "21") ? (float)1 : (float)0;
                    formattedData.Add(Tuple.Create<float[], float, float, float[], float>(oldState, actionIndex, reward, newState, isLastEpisode));
                }
            }

            Random rnd = new Random();
            for (int i = formattedData.Count - 1; i >= 0; i--)
            {
                int swapIndex = rnd.Next(i + 1);
                var tmp = formattedData.ElementAt(i);
                formattedData[i] = formattedData.ElementAt(swapIndex);
                formattedData[swapIndex] = tmp;
            }

            int cutIndex = (int)(formattedData.Count * 0.8);
            int stateDimension = 5;
            // Creating train batches
            BatchInputWrapper currentBatch = new BatchInputWrapper(stateDimension);
            List<BatchInputWrapper> trainDataList = new List<BatchInputWrapper>();
            foreach (var item in formattedData.GetRange(0, cutIndex))
            {
                if (!currentBatch.IsFull)
                {
                    currentBatch.AddStateActionRewardNewStateIsLastEpisodeData(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5);
                }
                else
                {
                    currentBatch.SealBatch();
                    trainDataList.Add(currentBatch);
                    currentBatch = new BatchInputWrapper(stateDimension);
                }
            }
            currentBatch.SealBatch();
            trainDataList.Add(currentBatch);
            trainData = trainDataList.AsEnumerable();

            // Creating test batches
            currentBatch = new BatchInputWrapper(stateDimension);
            List<BatchInputWrapper> testDataList = new List<BatchInputWrapper>();
            foreach (var item in formattedData.GetRange(cutIndex, formattedData.Count - cutIndex))
            {
                if (!currentBatch.IsFull)
                {
                    currentBatch.AddStateActionRewardNewStateIsLastEpisodeData(item.Item1, item.Item2, item.Item3, item.Item4, item.Item5);
                }
                else
                {
                    currentBatch.SealBatch();
                    testDataList.Add(currentBatch);
                    currentBatch = new BatchInputWrapper(stateDimension);
                }
            }
            currentBatch.SealBatch();
            testDataList.Add(currentBatch);
            cvData = testDataList.AsEnumerable();
        }
    }
}
