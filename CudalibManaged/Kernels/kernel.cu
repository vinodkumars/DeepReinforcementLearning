
#include "cuda_runtime.h"
#include "device_launch_parameters.h"

#include <stdio.h>
#include <math.h>
#include <float.h>

#define IDX2C(i,j,rows) (((j)*(rows))+(i))

__global__ void matrixPlusVector(float* input, float* bias, float * output, int rows, int columns)
{
	int i = blockDim.x * blockIdx.x + threadIdx.x;
	int j = blockDim.y * blockIdx.y + threadIdx.y;
	if (i < rows && j < columns)
	{
		int ij = IDX2C(i, j, rows);
		output[ij] = input[ij] + bias[i];
	}
}

__global__ void matrixTanh(float* input, float* output, int rows, int columns)
{
	int i = blockDim.x * blockIdx.x + threadIdx.x;
	int j = blockDim.y * blockIdx.y + threadIdx.y;
	if (i < rows && j < columns)
	{
		int ij = IDX2C(i, j, rows);
		output[ij] = tanh(input[ij]);
	}
}

__global__ void matrixIncorporateTanhDeriv(float* base, float* activation, float* output, int rows, int columns)
{
	int i = blockDim.x * blockIdx.x + threadIdx.x;
	int j = blockDim.y * blockIdx.y + threadIdx.y;
	if (i < rows && j < columns)
	{
		int ij = IDX2C(i, j, rows);
		output[ij] = base[ij] * (1 + activation[ij])*(1 - activation[ij]);
	}
}

__global__ void matrixReLu(float* input, float* output, int rows, int columns)
{
	int i = blockDim.x * blockIdx.x + threadIdx.x;
	int j = blockDim.y * blockIdx.y + threadIdx.y;
	if (i < rows && j < columns)
	{
		int ij = IDX2C(i, j, rows);
		output[ij] = fmaxf(input[ij], 0);
	}
}

__global__ void matrixIncorporateReLuDeriv(float* base, float* activation, float* output, int rows, int columns)
{
	int j = blockDim.x * blockIdx.x + threadIdx.x;
	int i = blockDim.y * blockIdx.y + threadIdx.y;
	if (i < rows && j < columns)
	{
		int ij = IDX2C(i, j, rows);
		output[ij] = activation[ij] <= 0 ? 0 : base[ij];
	}
}

__global__ void matrixSigmoid(float* input, float* output, int rows, int columns)
{
	int i = blockDim.x * blockIdx.x + threadIdx.x;
	int j = blockDim.y * blockIdx.y + threadIdx.y;
	if (i < rows && j < columns)
	{
		int ij = IDX2C(i, j, rows);
		// how to refactor the sigmoid calculation???
		output[ij] = (tanhf((input[ij]) / 2) + 1) / 2.0f;
	}
}

__global__ void matrixIncorporateSigmoidDeriv(float* base, float* activation, float* output, int rows, int columns)
{
	int i = blockDim.x * blockIdx.x + threadIdx.x;
	int j = blockDim.y * blockIdx.y + threadIdx.y;
	if (i < rows && j < columns)
	{
		int ij = IDX2C(i, j, rows);
		output[ij] = base[ij] * activation[ij] * (1 - activation[ij]);
	}
}

__global__ void matrixCrossEntropyError(float* sigmoidScores, float* trueLabels, float* output, int rows, int columns)
{
	int i = blockDim.x * blockIdx.x + threadIdx.x;
	int j = blockDim.y * blockIdx.y + threadIdx.y;
	if (i < rows && j < columns)
	{
		int ij = IDX2C(i, j, rows);
		output[ij] = trueLabels[ij] > 0 ? logf(sigmoidScores[ij] + FLT_EPSILON) : logf(1 - sigmoidScores[ij] + FLT_EPSILON);
		output[ij] *= -1;
	}
}

__global__ void matrixBellmanErrorAndDeriv(float* predictedQValues, float* maxQHatValues, float* chosenActionIndices, float* currentRewards, float* error, float* errorDerivative,
	float discount, float* isLastEpisode, int rows, int columns)
{
	int i = blockDim.x * blockIdx.x + threadIdx.x;
	int j = blockDim.y * blockIdx.y + threadIdx.y;
	if (i < rows && j < columns)
	{
		int ij = IDX2C(i, j, rows);
		float y = isLastEpisode[j] > 0 ? currentRewards[j] : currentRewards[j] + (discount*maxQHatValues[j]);
		errorDerivative[ij] = 0;

		// Calculating error and errorDerivative
		if (i == chosenActionIndices[j])
		{
			float tmp = predictedQValues[i] - y;
			errorDerivative[ij] = tmp;
			error[j] = 0.5*tmp*tmp;
		}
	}
}

__global__ void DqnStanfordEvaluation(float* predictedactionIndices, float* chosenActionIndices, float* currentRewards, float* matchPredictRewards, float* nonMatchPredictRewards, int rows)
{
	int i = blockDim.x * blockIdx.x + threadIdx.x;
	if (i < rows)
	{
		if (predictedactionIndices[i] == chosenActionIndices[i])
		{
			matchPredictRewards[i] = currentRewards[i];
		}
		else
		{
			nonMatchPredictRewards[i] = currentRewards[i];
		}
	}
}

__global__ void matrixHadamard(float* input1, float* input2, float alpha, float* output, float beta, int rows, int columns)
{
	int i = blockDim.x * blockIdx.x + threadIdx.x;
	int j = blockDim.y * blockIdx.y + threadIdx.y;
	if (i < rows && j < columns)
	{
		int ij = IDX2C(i, j, rows);
		output[ij] = alpha*input1[ij] * input2[ij] + beta*output[ij];
	}
}

__global__ void columnwiseMax(float* input, float* output, int rows, int columns)
{
	int j = blockDim.x * blockIdx.x + threadIdx.x;
	if (j < columns)
	{
		float maxInColumn = input[IDX2C(0, j, rows)];
		for (int i = 0; i < rows; i++)
		{
			int ij = IDX2C(i, j, rows);
			if (input[ij] > maxInColumn)
			{
				maxInColumn = input[ij];
			}
		}
		output[j] = maxInColumn;
	}
}

__global__ void columnwiseMaxIndex(float* input, float* output, int rows, int columns)
{
	int j = blockDim.x * blockIdx.x + threadIdx.x;
	if (j < columns)
	{
		int maxInColumnIndex = 0;
		float maxInColumn = input[IDX2C(maxInColumnIndex, j, rows)];
		for (int i = 0; i < rows; i++)
		{
			int ij = IDX2C(i, j, rows);
			if (input[ij] > maxInColumn)
			{
				maxInColumn = input[ij];
				maxInColumnIndex = i;
			}
		}
		output[j] = (float)maxInColumnIndex;
	}
}

int main()
{
	return 0;
}