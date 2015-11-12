#define __NV_CUBIN_HANDLE_STORAGE__ static
#include "crt/host_runtime.h"
#include "kernel.fatbin.c"
extern void __device_stub__Z16matrixPlusVectorPfS_S_ii(float *, float *, float *, int, int);
extern void __device_stub__Z10matrixTanhPfS_ii(float *, float *, int, int);
extern void __device_stub__Z26matrixIncorporateTanhDerivPfS_S_ii(float *, float *, float *, int, int);
extern void __device_stub__Z10matrixReLuPfS_ii(float *, float *, int, int);
extern void __device_stub__Z26matrixIncorporateReLuDerivPfS_S_ii(float *, float *, float *, int, int);
extern void __device_stub__Z13matrixSigmoidPfS_ii(float *, float *, int, int);
extern void __device_stub__Z29matrixIncorporateSigmoidDerivPfS_S_ii(float *, float *, float *, int, int);
extern void __device_stub__Z23matrixCrossEntropyErrorPfS_S_ii(float *, float *, float *, int, int);
extern void __device_stub__Z26matrixBellmanErrorAndDerivPfS_S_S_S_S_fS_ii(float *, float *, float *, float *, float *, float *, float, float *, int, int);
extern void __device_stub__Z21DqnStanfordEvaluationPfS_S_S_S_i(float *, float *, float *, float *, float *, int);
extern void __device_stub__Z14matrixHadamardPfS_fS_fii(float *, float *, float, float *, float, int, int);
extern void __device_stub__Z13columnwiseMaxPfS_ii(float *, float *, int, int);
extern void __device_stub__Z18columnwiseMaxIndexPfS_ii(float *, float *, int, int);
static void __nv_cudaEntityRegisterCallback(void **);
static void __sti____cudaRegisterAll_14_kernel_cpp1_ii_aea4bb27(void);
#pragma section(".CRT$XCU",read)
__declspec(allocate(".CRT$XCU"))static void (*__dummy_static_init__sti____cudaRegisterAll_14_kernel_cpp1_ii_aea4bb27[])(void) = {__sti____cudaRegisterAll_14_kernel_cpp1_ii_aea4bb27};
void __device_stub__Z16matrixPlusVectorPfS_S_ii(float *__par0, float *__par1, float *__par2, int __par3, int __par4){__cudaSetupArgSimple(__par0, 0Ui64);__cudaSetupArgSimple(__par1, 8Ui64);__cudaSetupArgSimple(__par2, 16Ui64);__cudaSetupArgSimple(__par3, 24Ui64);__cudaSetupArgSimple(__par4, 28Ui64);__cudaLaunch(((char *)((void ( *)(float *, float *, float *, int, int))matrixPlusVector)));}
#line 11 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
void matrixPlusVector( float *__cuda_0,float *__cuda_1,float *__cuda_2,int __cuda_3,int __cuda_4)
#line 12 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
{__device_stub__Z16matrixPlusVectorPfS_S_ii( __cuda_0,__cuda_1,__cuda_2,__cuda_3,__cuda_4);
#line 20 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
}
#line 1 "x64/Debug/kernel.cudafe1.stub.c"
void __device_stub__Z10matrixTanhPfS_ii( float *__par0,  float *__par1,  int __par2,  int __par3) {  __cudaSetupArgSimple(__par0, 0Ui64); __cudaSetupArgSimple(__par1, 8Ui64); __cudaSetupArgSimple(__par2, 16Ui64); __cudaSetupArgSimple(__par3, 20Ui64); __cudaLaunch(((char *)((void ( *)(float *, float *, int, int))matrixTanh))); }
#line 22 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
void matrixTanh( float *__cuda_0,float *__cuda_1,int __cuda_2,int __cuda_3)
#line 23 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
{__device_stub__Z10matrixTanhPfS_ii( __cuda_0,__cuda_1,__cuda_2,__cuda_3);
#line 31 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
}
#line 1 "x64/Debug/kernel.cudafe1.stub.c"
void __device_stub__Z26matrixIncorporateTanhDerivPfS_S_ii( float *__par0,  float *__par1,  float *__par2,  int __par3,  int __par4) {  __cudaSetupArgSimple(__par0, 0Ui64); __cudaSetupArgSimple(__par1, 8Ui64); __cudaSetupArgSimple(__par2, 16Ui64); __cudaSetupArgSimple(__par3, 24Ui64); __cudaSetupArgSimple(__par4, 28Ui64); __cudaLaunch(((char *)((void ( *)(float *, float *, float *, int, int))matrixIncorporateTanhDeriv))); }
#line 33 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
void matrixIncorporateTanhDeriv( float *__cuda_0,float *__cuda_1,float *__cuda_2,int __cuda_3,int __cuda_4)
#line 34 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
{__device_stub__Z26matrixIncorporateTanhDerivPfS_S_ii( __cuda_0,__cuda_1,__cuda_2,__cuda_3,__cuda_4);
#line 42 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
}
#line 1 "x64/Debug/kernel.cudafe1.stub.c"
void __device_stub__Z10matrixReLuPfS_ii( float *__par0,  float *__par1,  int __par2,  int __par3) {  __cudaSetupArgSimple(__par0, 0Ui64); __cudaSetupArgSimple(__par1, 8Ui64); __cudaSetupArgSimple(__par2, 16Ui64); __cudaSetupArgSimple(__par3, 20Ui64); __cudaLaunch(((char *)((void ( *)(float *, float *, int, int))matrixReLu))); }
#line 44 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
void matrixReLu( float *__cuda_0,float *__cuda_1,int __cuda_2,int __cuda_3)
#line 45 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
{__device_stub__Z10matrixReLuPfS_ii( __cuda_0,__cuda_1,__cuda_2,__cuda_3);
#line 53 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
}
#line 1 "x64/Debug/kernel.cudafe1.stub.c"
void __device_stub__Z26matrixIncorporateReLuDerivPfS_S_ii( float *__par0,  float *__par1,  float *__par2,  int __par3,  int __par4) {  __cudaSetupArgSimple(__par0, 0Ui64); __cudaSetupArgSimple(__par1, 8Ui64); __cudaSetupArgSimple(__par2, 16Ui64); __cudaSetupArgSimple(__par3, 24Ui64); __cudaSetupArgSimple(__par4, 28Ui64); __cudaLaunch(((char *)((void ( *)(float *, float *, float *, int, int))matrixIncorporateReLuDeriv))); }
#line 55 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
void matrixIncorporateReLuDeriv( float *__cuda_0,float *__cuda_1,float *__cuda_2,int __cuda_3,int __cuda_4)
#line 56 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
{__device_stub__Z26matrixIncorporateReLuDerivPfS_S_ii( __cuda_0,__cuda_1,__cuda_2,__cuda_3,__cuda_4);
#line 64 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
}
#line 1 "x64/Debug/kernel.cudafe1.stub.c"
void __device_stub__Z13matrixSigmoidPfS_ii( float *__par0,  float *__par1,  int __par2,  int __par3) {  __cudaSetupArgSimple(__par0, 0Ui64); __cudaSetupArgSimple(__par1, 8Ui64); __cudaSetupArgSimple(__par2, 16Ui64); __cudaSetupArgSimple(__par3, 20Ui64); __cudaLaunch(((char *)((void ( *)(float *, float *, int, int))matrixSigmoid))); }
#line 66 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
void matrixSigmoid( float *__cuda_0,float *__cuda_1,int __cuda_2,int __cuda_3)
#line 67 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
{__device_stub__Z13matrixSigmoidPfS_ii( __cuda_0,__cuda_1,__cuda_2,__cuda_3);
#line 76 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
}
#line 1 "x64/Debug/kernel.cudafe1.stub.c"
void __device_stub__Z29matrixIncorporateSigmoidDerivPfS_S_ii( float *__par0,  float *__par1,  float *__par2,  int __par3,  int __par4) {  __cudaSetupArgSimple(__par0, 0Ui64); __cudaSetupArgSimple(__par1, 8Ui64); __cudaSetupArgSimple(__par2, 16Ui64); __cudaSetupArgSimple(__par3, 24Ui64); __cudaSetupArgSimple(__par4, 28Ui64); __cudaLaunch(((char *)((void ( *)(float *, float *, float *, int, int))matrixIncorporateSigmoidDeriv))); }
#line 78 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
void matrixIncorporateSigmoidDeriv( float *__cuda_0,float *__cuda_1,float *__cuda_2,int __cuda_3,int __cuda_4)
#line 79 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
{__device_stub__Z29matrixIncorporateSigmoidDerivPfS_S_ii( __cuda_0,__cuda_1,__cuda_2,__cuda_3,__cuda_4);
#line 87 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
}
#line 1 "x64/Debug/kernel.cudafe1.stub.c"
void __device_stub__Z23matrixCrossEntropyErrorPfS_S_ii( float *__par0,  float *__par1,  float *__par2,  int __par3,  int __par4) {  __cudaSetupArgSimple(__par0, 0Ui64); __cudaSetupArgSimple(__par1, 8Ui64); __cudaSetupArgSimple(__par2, 16Ui64); __cudaSetupArgSimple(__par3, 24Ui64); __cudaSetupArgSimple(__par4, 28Ui64); __cudaLaunch(((char *)((void ( *)(float *, float *, float *, int, int))matrixCrossEntropyError))); }
#line 89 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
void matrixCrossEntropyError( float *__cuda_0,float *__cuda_1,float *__cuda_2,int __cuda_3,int __cuda_4)
#line 90 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
{__device_stub__Z23matrixCrossEntropyErrorPfS_S_ii( __cuda_0,__cuda_1,__cuda_2,__cuda_3,__cuda_4);
#line 99 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
}
#line 1 "x64/Debug/kernel.cudafe1.stub.c"
void __device_stub__Z26matrixBellmanErrorAndDerivPfS_S_S_S_S_fS_ii( float *__par0,  float *__par1,  float *__par2,  float *__par3,  float *__par4,  float *__par5,  float __par6,  float *__par7,  int __par8,  int __par9) {  __cudaSetupArgSimple(__par0, 0Ui64); __cudaSetupArgSimple(__par1, 8Ui64); __cudaSetupArgSimple(__par2, 16Ui64); __cudaSetupArgSimple(__par3, 24Ui64); __cudaSetupArgSimple(__par4, 32Ui64); __cudaSetupArgSimple(__par5, 40Ui64); __cudaSetupArgSimple(__par6, 48Ui64); __cudaSetupArgSimple(__par7, 56Ui64); __cudaSetupArgSimple(__par8, 64Ui64); __cudaSetupArgSimple(__par9, 68Ui64); __cudaLaunch(((char *)((void ( *)(float *, float *, float *, float *, float *, float *, float, float *, int, int))matrixBellmanErrorAndDeriv))); }
#line 101 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
void matrixBellmanErrorAndDeriv( float *__cuda_0,float *__cuda_1,float *__cuda_2,float *__cuda_3,float *__cuda_4,float *__cuda_5,float __cuda_6,float *__cuda_7,int __cuda_8,int __cuda_9)
#line 103 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
{__device_stub__Z26matrixBellmanErrorAndDerivPfS_S_S_S_S_fS_ii( __cuda_0,__cuda_1,__cuda_2,__cuda_3,__cuda_4,__cuda_5,__cuda_6,__cuda_7,__cuda_8,__cuda_9);
#line 120 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
}
#line 1 "x64/Debug/kernel.cudafe1.stub.c"
void __device_stub__Z21DqnStanfordEvaluationPfS_S_S_S_i( float *__par0,  float *__par1,  float *__par2,  float *__par3,  float *__par4,  int __par5) {  __cudaSetupArgSimple(__par0, 0Ui64); __cudaSetupArgSimple(__par1, 8Ui64); __cudaSetupArgSimple(__par2, 16Ui64); __cudaSetupArgSimple(__par3, 24Ui64); __cudaSetupArgSimple(__par4, 32Ui64); __cudaSetupArgSimple(__par5, 40Ui64); __cudaLaunch(((char *)((void ( *)(float *, float *, float *, float *, float *, int))DqnStanfordEvaluation))); }
#line 122 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
void DqnStanfordEvaluation( float *__cuda_0,float *__cuda_1,float *__cuda_2,float *__cuda_3,float *__cuda_4,int __cuda_5)
#line 123 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
{__device_stub__Z21DqnStanfordEvaluationPfS_S_S_S_i( __cuda_0,__cuda_1,__cuda_2,__cuda_3,__cuda_4,__cuda_5);
#line 136 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
}
#line 1 "x64/Debug/kernel.cudafe1.stub.c"
void __device_stub__Z14matrixHadamardPfS_fS_fii( float *__par0,  float *__par1,  float __par2,  float *__par3,  float __par4,  int __par5,  int __par6) {  __cudaSetupArgSimple(__par0, 0Ui64); __cudaSetupArgSimple(__par1, 8Ui64); __cudaSetupArgSimple(__par2, 16Ui64); __cudaSetupArgSimple(__par3, 24Ui64); __cudaSetupArgSimple(__par4, 32Ui64); __cudaSetupArgSimple(__par5, 36Ui64); __cudaSetupArgSimple(__par6, 40Ui64); __cudaLaunch(((char *)((void ( *)(float *, float *, float, float *, float, int, int))matrixHadamard))); }
#line 138 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
void matrixHadamard( float *__cuda_0,float *__cuda_1,float __cuda_2,float *__cuda_3,float __cuda_4,int __cuda_5,int __cuda_6)
#line 139 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
{__device_stub__Z14matrixHadamardPfS_fS_fii( __cuda_0,__cuda_1,__cuda_2,__cuda_3,__cuda_4,__cuda_5,__cuda_6);
#line 147 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
}
#line 1 "x64/Debug/kernel.cudafe1.stub.c"
void __device_stub__Z13columnwiseMaxPfS_ii( float *__par0,  float *__par1,  int __par2,  int __par3) {  __cudaSetupArgSimple(__par0, 0Ui64); __cudaSetupArgSimple(__par1, 8Ui64); __cudaSetupArgSimple(__par2, 16Ui64); __cudaSetupArgSimple(__par3, 20Ui64); __cudaLaunch(((char *)((void ( *)(float *, float *, int, int))columnwiseMax))); }
#line 149 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
void columnwiseMax( float *__cuda_0,float *__cuda_1,int __cuda_2,int __cuda_3)
#line 150 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
{__device_stub__Z13columnwiseMaxPfS_ii( __cuda_0,__cuda_1,__cuda_2,__cuda_3);
#line 165 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
}
#line 1 "x64/Debug/kernel.cudafe1.stub.c"
void __device_stub__Z18columnwiseMaxIndexPfS_ii( float *__par0,  float *__par1,  int __par2,  int __par3) {  __cudaSetupArgSimple(__par0, 0Ui64); __cudaSetupArgSimple(__par1, 8Ui64); __cudaSetupArgSimple(__par2, 16Ui64); __cudaSetupArgSimple(__par3, 20Ui64); __cudaLaunch(((char *)((void ( *)(float *, float *, int, int))columnwiseMaxIndex))); }
#line 167 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
void columnwiseMaxIndex( float *__cuda_0,float *__cuda_1,int __cuda_2,int __cuda_3)
#line 168 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
{__device_stub__Z18columnwiseMaxIndexPfS_ii( __cuda_0,__cuda_1,__cuda_2,__cuda_3);
#line 185 "D:/vinods/Repos/MSR-DSSM/private/vinods/CudalibManaged/Kernels/kernel.cu"
}
#line 1 "x64/Debug/kernel.cudafe1.stub.c"
static void __nv_cudaEntityRegisterCallback( void **__T20) {  __nv_dummy_param_ref(__T20); __nv_save_fatbinhandle_for_managed_rt(__T20); __cudaRegisterEntry(__T20, ((void ( *)(float *, float *, int, int))columnwiseMaxIndex), _Z18columnwiseMaxIndexPfS_ii, (-1)); __cudaRegisterEntry(__T20, ((void ( *)(float *, float *, int, int))columnwiseMax), _Z13columnwiseMaxPfS_ii, (-1)); __cudaRegisterEntry(__T20, ((void ( *)(float *, float *, float, float *, float, int, int))matrixHadamard), _Z14matrixHadamardPfS_fS_fii, (-1)); __cudaRegisterEntry(__T20, ((void ( *)(float *, float *, float *, float *, float *, int))DqnStanfordEvaluation), _Z21DqnStanfordEvaluationPfS_S_S_S_i, (-1)); __cudaRegisterEntry(__T20, ((void ( *)(float *, float *, float *, float *, float *, float *, float, float *, int, int))matrixBellmanErrorAndDeriv), _Z26matrixBellmanErrorAndDerivPfS_S_S_S_S_fS_ii, (-1)); __cudaRegisterEntry(__T20, ((void ( *)(float *, float *, float *, int, int))matrixCrossEntropyError), _Z23matrixCrossEntropyErrorPfS_S_ii, (-1)); __cudaRegisterEntry(__T20, ((void ( *)(float *, float *, float *, int, int))matrixIncorporateSigmoidDeriv), _Z29matrixIncorporateSigmoidDerivPfS_S_ii, (-1)); __cudaRegisterEntry(__T20, ((void ( *)(float *, float *, int, int))matrixSigmoid), _Z13matrixSigmoidPfS_ii, (-1)); __cudaRegisterEntry(__T20, ((void ( *)(float *, float *, float *, int, int))matrixIncorporateReLuDeriv), _Z26matrixIncorporateReLuDerivPfS_S_ii, (-1)); __cudaRegisterEntry(__T20, ((void ( *)(float *, float *, int, int))matrixReLu), _Z10matrixReLuPfS_ii, (-1)); __cudaRegisterEntry(__T20, ((void ( *)(float *, float *, float *, int, int))matrixIncorporateTanhDeriv), _Z26matrixIncorporateTanhDerivPfS_S_ii, (-1)); __cudaRegisterEntry(__T20, ((void ( *)(float *, float *, int, int))matrixTanh), _Z10matrixTanhPfS_ii, (-1)); __cudaRegisterEntry(__T20, ((void ( *)(float *, float *, float *, int, int))matrixPlusVector), _Z16matrixPlusVectorPfS_S_ii, (-1)); }
static void __sti____cudaRegisterAll_14_kernel_cpp1_ii_aea4bb27(void) {  __cudaRegisterBinary(__nv_cudaEntityRegisterCallback);  }
