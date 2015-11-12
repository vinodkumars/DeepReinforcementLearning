using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedCuda.CudaBlas;
using ManagedCuda;
using ManagedCuda.BasicTypes;
using ManagedCuda.VectorTypes;

namespace CudaLibManaged
{
    public enum MatrixTransformation
    {
        None,
        Transpose
    }
    public enum MathType
    {
        CPU,
        GPU
    }

    public abstract class MathOperations : IDisposable
    {
        /// <summary>
        /// b = transpose of a
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        public abstract void MatrixTranspose(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false);
        /// <summary>
        /// Performs c = alpha*transA(a) + beta*transB(b)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="transA"></param>
        /// <param name="alpha"></param>
        /// <param name="b"></param>
        /// <param name="transB"></param>
        /// <param name="beta"></param>
        /// <param name="c"></param>
        /// <param name="skipCopyFromCuda"></param>
        public abstract void MatrixAddition(Matrix a, MatrixTransformation transA, float alpha, Matrix b, MatrixTransformation transB, float beta, Matrix c, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false);
        /// <summary>
        /// Performs c = alpha*transA(a)*transB(b) + beta*c
        /// </summary>
        /// <param name="a"></param>
        /// <param name="transA"></param>
        /// <param name="alpha"></param>
        /// <param name="b"></param>
        /// <param name="transB"></param>
        /// <param name="c"></param>
        /// <param name="beta"></param>
        /// <param name="skipCopyFromCuda"></param>
        public abstract void MatrixMultiplication(Matrix a, MatrixTransformation transA, float alpha, Matrix b, MatrixTransformation transB, Matrix c, float beta, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false);
        /// <summary>
        /// Performs c = tanh(a + b)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="bias"></param>
        public abstract void MatrixTanh(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false);
        /// <summary>
        /// Performs b = max(a,0)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="skipCopyFromCuda"></param>
        public abstract void MatrixReLu(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false);
        /// <summary>
        /// Calculates b = sigmoid(a)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="skipCopyFromCuda"></param>
        public abstract void MatrixSigmoid(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false);
        /// <summary>
        /// output = HadamardProduct(baseData, f'(activation)). Assumes 'activation' has the tanh values in it.
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="activation"></param>
        /// <param name="skipCopyFromCuda"></param>
        public abstract void MatrixIncorporateTanhDerivative(Matrix baseData, Matrix activation, Matrix output, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false);
        /// <summary>
        /// output = HadamardProduct(baseData, f'(activation)). Assumes 'activation' has the ReLu values in it.
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="activation"></param>
        /// <param name="skipCopyFromCuda"></param>
        public abstract void MatrixIncorporateReLuDerivative(Matrix baseData, Matrix activation, Matrix output, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false);
        /// <summary>
        /// output = HadamardProduct(baseData, f'(activation)). Assumes 'activation' has the sigmoid values in it.
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="activation"></param>
        /// <param name="skipCopyFromCuda"></param>
        public abstract void MatrixIncorporateSigmoidDerivative(Matrix baseData, Matrix activation, Matrix output, bool copyInputsFromCpuToGp = false, bool copyOutputsFromGpuToCpu = false);
        /// <summary>
        /// Adds vector 'b' to all the columns of 'a' and assigns to 'c'
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="copyInputsFromCpuToGpu"></param>
        /// <param name="copyOutputsFromGpuToCpu"></param>
        public abstract void MatrixPlusVector(Matrix a, Matrix b, Matrix c, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false);
        /// <summary>
        /// vector 'b' = columnwise max of matrix 'a'
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="copyInputsFromCpuToGpu"></param>
        /// <param name="copyOutputsFromGpuToCpu"></param>
        public abstract void ColumnWiseMax(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false);
        /// <summary>
        /// vector 'b' = zero based index of columnwise max of matrix 'a'
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="copyInputsFromCpuToGpu"></param>
        /// <param name="copyOutputsFromGpuToCpu"></param>
        public abstract void ColumnWiseMaxIndex(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false);
        /// <summary>
        /// Performs a.b and returns a scalar
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public abstract float Dot(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false);
        /// <summary>
        /// Performs c = alpha*ElementWiseProduct(a, b) + beta*c
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="skipCopyFromCuda"></param>
        public abstract void MatrixHadamardProduct(Matrix a, Matrix b, float alpha, Matrix c, float beta, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false);
        /// <summary>
        /// Calculates OutputError = -trueLabel*ln(sigmoidScores) - (1-trueLabel)*ln(1-sigmoidScores)
        /// </summary>
        /// <param name="sigmoidScores"></param>
        /// <param name="trueLabels"></param>
        /// <param name="OutputError"></param>
        /// <param name="skipCopyFromCuda"></param>
        public abstract void MatrixCrossEntropyError(Matrix sigmoidScores, Matrix trueLabels, Matrix OutputError, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false);
        /// <summary>
        /// y = isLastEpisode ? currentRewards : currentRewards + (discount*maxQHatValues);
        /// error = 0.5 * (predictedQValue-y) * (predictedQValue-y)
        /// errorDerivative = (predictedQValue-y)
        /// </summary>
        /// <param name="predictedQValues"></param>
        /// <param name="maxQHatValues"></param>
        /// <param name="chosenActionIndices"></param>
        /// <param name="currentRewards"></param>
        /// <param name="error"></param>
        /// <param name="errorDerivative"></param>
        /// <param name="discount"></param>
        /// <param name="isLastEpisode"></param>
        /// <param name="copyInputsFromCpuToGpu"></param>
        /// <param name="copyOutputsFromGpuToCpu"></param>
        public abstract void MatrixBellmanErrorAndDerivative(Matrix predictedQValues, Matrix maxQHatValues, Matrix chosenActionIndices, Matrix currentRewards, Matrix error, Matrix errorDerivative,
                                                          float discount, Matrix isLastEpisode, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false);
        public abstract void DqnStanfordEvaluation(Matrix predictedActionIndices, Matrix chosenActionIndices, Matrix currentRewards, Matrix matchPredictRewards, Matrix nonMatchPredictRewards, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false);
        public abstract void Dispose();
        protected void VerifyColumnWithRowOfMatrices(Matrix a, Matrix b)
        {
            if (a.Column != b.Row)
            {
                throw new ArgumentException("col of a != row of b");
            }
        }
        protected void VerifyRowsOfMatrices(Matrix a, Matrix b)
        {
            if (a.Row != b.Row)
            {
                throw new ArgumentException("row of a != row of b");
            }
        }
        protected void VerifyColumnsOfMatrices(Matrix a, Matrix b)
        {
            if (a.Column != b.Column)
            {
                throw new ArgumentException("col of a != col of b");
            }
        }
        protected void VerifyDimentionalityOfMatrices(Matrix a, Matrix b)
        {
            this.VerifyRowsOfMatrices(a, b);
            this.VerifyColumnsOfMatrices(a, b);
        }
        protected void VerifyDimentionalityOfMatrices(Matrix a, Matrix b, Matrix c)
        {
            VerifyDimentionalityOfMatrices(a, b);
            VerifyDimentionalityOfMatrices(b, c);
        }
        protected bool VerifyDimentionalityForMatrixMultiplication(Matrix a, MatrixTransformation transA, Matrix b, MatrixTransformation transB, Matrix c)
        {
            int m = transA == MatrixTransformation.None ? a.Row : a.Column;
            int k1 = transA == MatrixTransformation.None ? a.Column : a.Row;
            int k2 = transB == MatrixTransformation.None ? b.Row : b.Column;
            int n = transB == MatrixTransformation.None ? b.Column : b.Row;

            if (k1 != k2)
            {
                throw new ArgumentException("column of trnasA(a) != row of transB(b)");
            }
            if (m != c.Row)
            {
                throw new ArgumentException("row of trnasA(a) != row of c)");
            }
            if (n != c.Column)
            {
                throw new ArgumentException("column of trnasB(b) != column of c)");
            }
            return true;
        }
        protected bool VerifyDimentionalityForMatrixAddition(Matrix a, MatrixTransformation transA, Matrix b, MatrixTransformation transB, Matrix c)
        {
            int m1 = transA == MatrixTransformation.None ? a.Row : a.Column;
            int n1 = transA == MatrixTransformation.None ? a.Column : a.Row;
            int m2 = transB == MatrixTransformation.None ? b.Row : b.Column;
            int n2 = transB == MatrixTransformation.None ? b.Column : b.Row;

            if (m1 != m2 || m2 != c.Row)
            {
                throw new ArgumentException("row of transA(a) != row of transB(b) != row of c");
            }
            if (n1 != n2 || n2 != c.Column)
            {
                throw new ArgumentException("column of transA(a) != column of transB(b) != column of c");
            }
            return true;
        }
        protected Operation GetOperation(MatrixTransformation trans)
        {
            Operation retVal = Operation.NonTranspose;
            switch (trans)
            {
                case MatrixTransformation.None:
                    retVal = Operation.NonTranspose;
                    break;
                case MatrixTransformation.Transpose:
                    retVal = Operation.Transpose;
                    break;
                //case MatrixTransformation.ConjugateTranspose:
                //    retVal = Operation.ConjugateTranspose;
                //    break;
            }
            return retVal;
        }
        public MathOperations()
        {
        }
    }
    class GpuMathOperations : MathOperations
    {
        private CudaBlas cuBlas;
        private CudaContext cudaContext;
        private CUmodule cuModule;
        private const string kernalFile = @"kernel.ptx";
        private int maxThreadPerBlockDim;
        public GpuMathOperations()
        {
            this.cuBlas = new CudaBlas();
            this.cudaContext = new CudaContext();
            this.cuModule = cudaContext.LoadModulePTX(kernalFile);
            this.maxThreadPerBlockDim = (int)Math.Sqrt(this.cudaContext.GetDeviceInfo().MaxThreadsPerBlock);
        }
        public override void MatrixTranspose(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            this.VerifyDimentionalityForMatrixAddition(a, MatrixTransformation.Transpose, a, MatrixTransformation.Transpose, b);
            if (copyInputsFromCpuToGpu)
            {
                a.CopyToCuda();
            }

            float alpha = 1, beta = 0;
            cuBlas.Geam(Operation.Transpose, Operation.Transpose, b.Row, b.Column, alpha, a.DeviceData, a.Row, a.DeviceData, a.Row, beta, b.DeviceData, b.Row);
            if (copyOutputsFromGpuToCpu)
            {
                b.CopyFromCuda();
            }
        }
        public override void MatrixAddition(Matrix a, MatrixTransformation transA, float alpha, Matrix b, MatrixTransformation transB, float beta, Matrix c, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            this.VerifyDimentionalityForMatrixAddition(a, transA, b, transB, c);
            if (copyInputsFromCpuToGpu)
            {
                a.CopyToCuda();
                b.CopyToCuda();
            }

            cuBlas.Geam(GetOperation(transA), GetOperation(transB), transA == MatrixTransformation.None ? a.Row : a.Column, transA == MatrixTransformation.None ? a.Column : a.Row
                , alpha, a.DeviceData, a.Row, b.DeviceData, b.Row, beta, c.DeviceData, c.Row);

            if (copyOutputsFromGpuToCpu)
            {
                c.CopyFromCuda();
            }
        }
        public override void MatrixMultiplication(Matrix a, MatrixTransformation transA, float alpha, Matrix b, MatrixTransformation transB, Matrix c, float beta, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            VerifyDimentionalityForMatrixMultiplication(a, transA, b, transB, c);
            if (copyInputsFromCpuToGpu)
            {
                a.CopyToCuda();
                b.CopyToCuda();
            }

            cuBlas.Gemm(GetOperation(transA), GetOperation(transB), c.Row, c.Column, transA == MatrixTransformation.None ? a.Column : a.Row,
                alpha, a.DeviceData, a.Row, b.DeviceData, b.Row, beta, c.DeviceData, c.Row);

            if (copyOutputsFromGpuToCpu)
            {
                c.CopyFromCuda();
            }
        }
        public override void MatrixPlusVector(Matrix a, Matrix b, Matrix c, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            this.VerifyDimentionalityOfMatrices(a, c);
            this.VerifyRowsOfMatrices(a, b);
            this.KernelOperation("_Z16matrixPlusVectorPfS_S_ii", a, b, c, copyInputsFromCpuToGpu, copyOutputsFromGpuToCpu);
        }
        public override void MatrixTanh(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            this.VerifyDimentionalityOfMatrices(a, b);
            this.KernelOperation("_Z10matrixTanhPfS_ii", a, b, copyInputsFromCpuToGpu, copyOutputsFromGpuToCpu);
        }
        public override void MatrixIncorporateTanhDerivative(Matrix baseData, Matrix activation, Matrix output, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            this.VerifyDimentionalityOfMatrices(baseData, activation, output);
            this.KernelOperation("_Z26matrixIncorporateTanhDerivPfS_S_ii", baseData, activation, output, copyInputsFromCpuToGpu, copyOutputsFromGpuToCpu);
        }
        public override void MatrixReLu(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            this.VerifyDimentionalityOfMatrices(a, b);
            this.KernelOperation("_Z10matrixReLuPfS_ii", a, b, copyInputsFromCpuToGpu, copyOutputsFromGpuToCpu);
        }
        public override void MatrixIncorporateReLuDerivative(Matrix baseData, Matrix activation, Matrix output, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            this.VerifyDimentionalityOfMatrices(baseData, activation, output);
            this.KernelOperation("_Z26matrixIncorporateReLuDerivPfS_S_ii", baseData, activation, output, copyInputsFromCpuToGpu, copyOutputsFromGpuToCpu);
        }
        public override void MatrixSigmoid(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            this.VerifyDimentionalityOfMatrices(a, b);
            this.KernelOperation("_Z13matrixSigmoidPfS_ii", a, b, copyInputsFromCpuToGpu, copyOutputsFromGpuToCpu);
        }
        public override void MatrixIncorporateSigmoidDerivative(Matrix baseData, Matrix activation, Matrix output, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            this.VerifyDimentionalityOfMatrices(baseData, activation, output);
            this.KernelOperation("_Z29matrixIncorporateSigmoidDerivPfS_S_ii", baseData, activation, output, copyInputsFromCpuToGpu, copyOutputsFromGpuToCpu);
        }
        public override void MatrixHadamardProduct(Matrix a, Matrix b, float alpha, Matrix c, float beta, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            this.VerifyDimentionalityOfMatrices(a, b, c);
            this.KernelOperation("_Z14matrixHadamardPfS_fS_fii", a, b, c, copyInputsFromCpuToGpu, copyOutputsFromGpuToCpu);
        }
        public override void MatrixCrossEntropyError(Matrix sigmoidScores, Matrix trueLabels, Matrix OutputError, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            this.VerifyDimentionalityOfMatrices(sigmoidScores, trueLabels, OutputError);

            if (copyInputsFromCpuToGpu)
            {
                sigmoidScores.CopyToCuda();
                trueLabels.CopyToCuda();
            }

            CudaKernel kernel = InitializeGridsAndThreads("_Z23matrixCrossEntropyErrorPfS_S_ii", OutputError);
            kernel.Run(sigmoidScores.DeviceData.DevicePointer, trueLabels.DeviceData.DevicePointer, OutputError.DeviceData.DevicePointer,
                       sigmoidScores.Row, sigmoidScores.Column);

            if (copyOutputsFromGpuToCpu)
            {
                OutputError.CopyFromCuda();
            }
        }
        public override void MatrixBellmanErrorAndDerivative(Matrix predictedQValues, Matrix maxQHatValues, Matrix chosenActionIndices, Matrix currentRewards, Matrix error, Matrix errorDerivative,
                                                          float discount, Matrix isLastEpisode, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            this.VerifyDimentionalityOfMatrices(predictedQValues, errorDerivative);
            this.VerifyColumnWithRowOfMatrices(predictedQValues, maxQHatValues);
            this.VerifyDimentionalityOfMatrices(maxQHatValues, chosenActionIndices, currentRewards);
            this.VerifyDimentionalityOfMatrices(currentRewards, error, isLastEpisode);

            if (copyInputsFromCpuToGpu)
            {
                predictedQValues.CopyToCuda();
                maxQHatValues.CopyToCuda();
            }

            CudaKernel kernel = InitializeGridsAndThreads("_Z26matrixBellmanErrorAndDerivPfS_S_S_S_S_fS_ii", errorDerivative);
            kernel.Run(predictedQValues.DeviceData.DevicePointer, maxQHatValues.DeviceData.DevicePointer, chosenActionIndices.DeviceData.DevicePointer, currentRewards.DeviceData.DevicePointer, error.DeviceData.DevicePointer,
                       errorDerivative.DeviceData.DevicePointer, discount, isLastEpisode.DeviceData.DevicePointer, errorDerivative.Row, errorDerivative.Column);

            if (copyOutputsFromGpuToCpu)
            {
                error.CopyFromCuda();
                errorDerivative.CopyFromCuda();
            }
        }
        public override void DqnStanfordEvaluation(Matrix predictedActionIndices, Matrix chosenActionIndices, Matrix currentRewards, Matrix matchPredictRewards, Matrix nonMatchPredictRewards,
                                                   bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            this.VerifyDimentionalityOfMatrices(predictedActionIndices, chosenActionIndices, currentRewards);
            this.VerifyDimentionalityOfMatrices(currentRewards, matchPredictRewards, nonMatchPredictRewards);

            if (copyInputsFromCpuToGpu)
            {
                predictedActionIndices.CopyToCuda();
                chosenActionIndices.CopyToCuda();
                currentRewards.CopyToCuda();
            }

            CudaKernel kernel = InitializeGridsAndThreads("_Z21DqnStanfordEvaluationPfS_S_S_S_i", matchPredictRewards);
            kernel.Run(predictedActionIndices.DeviceData.DevicePointer, chosenActionIndices.DeviceData.DevicePointer, currentRewards.DeviceData.DevicePointer,
                       matchPredictRewards.DeviceData.DevicePointer, nonMatchPredictRewards.DeviceData.DevicePointer, matchPredictRewards.Row);


            if (copyOutputsFromGpuToCpu)
            {
                matchPredictRewards.CopyToCuda();
                nonMatchPredictRewards.CopyToCuda();
            }
        }
        public override void ColumnWiseMax(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            this.VerifyColumnWithRowOfMatrices(a, b);
            this.KernelOperation("_Z13columnwiseMaxPfS_ii", a, b, copyInputsFromCpuToGpu, copyOutputsFromGpuToCpu);
        }
        public override void ColumnWiseMaxIndex(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            this.VerifyColumnWithRowOfMatrices(a, b);
            this.KernelOperation("_Z18columnwiseMaxIndexPfS_ii", a, b, copyInputsFromCpuToGpu, copyOutputsFromGpuToCpu);
        }
        public override float Dot(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false)
        {
            this.VerifyDimentionalityOfMatrices(a, b);
            if (copyInputsFromCpuToGpu)
            {
                a.CopyToCuda();
                b.CopyToCuda();
            }

            return cuBlas.Dot(a.DeviceData, 1, b.DeviceData, 1);
        }
        private void KernelOperation(string kernalName, Matrix a, Matrix b, Matrix c, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            if (copyInputsFromCpuToGpu)
            {
                a.CopyToCuda();
                b.CopyToCuda();
            }

            CudaKernel kernel = InitializeGridsAndThreads(kernalName, c);
            kernel.Run(a.DeviceData.DevicePointer, b.DeviceData.DevicePointer, c.DeviceData.DevicePointer, a.Row, a.Column);

            if (copyOutputsFromGpuToCpu)
            {
                c.CopyFromCuda();
            }
        }
        private void KernelOperation(string kernalName, Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            if (copyInputsFromCpuToGpu)
            {
                a.CopyToCuda();
            }

            CudaKernel kernel = InitializeGridsAndThreads(kernalName, b);
            kernel.Run(a.DeviceData.DevicePointer, b.DeviceData.DevicePointer, a.Row, a.Column);

            if (copyOutputsFromGpuToCpu)
            {
                b.CopyFromCuda();
            }
        }
        private CudaKernel InitializeGridsAndThreads(string kernalName, Matrix a)
        {
            int maxThreads = Math.Min(this.maxThreadPerBlockDim, a.Row);
            dim3 threads = a.Column == 1 ? new dim3(maxThreads, 1) : new dim3(maxThreads, maxThreads);
            dim3 blocks = new dim3((a.Row + maxThreads - 1) / maxThreads, (a.Column + maxThreads - 1) / maxThreads);
            CudaKernel kernel = new CudaKernel(kernalName, this.cuModule, this.cudaContext)
            {
                GridDimensions = blocks,
                BlockDimensions = threads
            };
            return kernel;
        }
        public override void Dispose()
        {
            cuBlas.Dispose();
            cudaContext.Dispose();
        }
    }
    class CpuMathOperations : MathOperations
    {
        public override void MatrixTranspose(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            throw new NotImplementedException();
        }

        public override void MatrixAddition(Matrix a, MatrixTransformation transA, float alpha, Matrix b, MatrixTransformation transB, float beta, Matrix c, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            throw new NotImplementedException();
        }

        public override void MatrixMultiplication(Matrix a, MatrixTransformation transA, float alpha, Matrix b, MatrixTransformation transB, Matrix c, float beta, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            throw new NotImplementedException();
        }

        public override void MatrixTanh(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            throw new NotImplementedException();
        }

        public override void MatrixReLu(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            throw new NotImplementedException();
        }

        public override void MatrixSigmoid(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            throw new NotImplementedException();
        }

        public override void MatrixIncorporateTanhDerivative(Matrix baseData, Matrix activation, Matrix output, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            throw new NotImplementedException();
        }

        public override void MatrixIncorporateReLuDerivative(Matrix baseData, Matrix activation, Matrix output, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            throw new NotImplementedException();
        }

        public override void MatrixIncorporateSigmoidDerivative(Matrix baseData, Matrix activation, Matrix output, bool copyInputsFromCpuToGp = false, bool copyOutputsFromGpuToCpu = false)
        {
            throw new NotImplementedException();
        }

        public override void MatrixPlusVector(Matrix a, Matrix b, Matrix c, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            throw new NotImplementedException();
        }

        public override void ColumnWiseMax(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            throw new NotImplementedException();
        }

        public override void ColumnWiseMaxIndex(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            throw new NotImplementedException();
        }

        public override float Dot(Matrix a, Matrix b, bool copyInputsFromCpuToGpu = false)
        {
            throw new NotImplementedException();
        }

        public override void MatrixHadamardProduct(Matrix a, Matrix b, float alpha, Matrix c, float beta, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            throw new NotImplementedException();
        }

        public override void MatrixCrossEntropyError(Matrix sigmoidScores, Matrix trueLabels, Matrix OutputError, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            throw new NotImplementedException();
        }

        public override void MatrixBellmanErrorAndDerivative(Matrix predictedQValues, Matrix maxQHatValues, Matrix chosenActionIndices, Matrix currentRewards, Matrix error, Matrix errorDerivative, float discount, Matrix isLastEpisode, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            throw new NotImplementedException();
        }

        public override void DqnStanfordEvaluation(Matrix predictedActionIndices, Matrix chosenActionIndices, Matrix currentRewards, Matrix matchPredictRewards, Matrix nonMatchPredictRewards, bool copyInputsFromCpuToGpu = false, bool copyOutputsFromGpuToCpu = false)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
