using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CudaLibManaged
{
    public class MathOperationManager : IDisposable
    {
        MathOperations globalInstance;
        public MathType MathType { get; private set; }
        public MathOperationManager(MathType type)
        {
            this.MathType = type;
            if (type == MathType.GPU)
            {
                this.globalInstance = new GpuMathOperations();
            }
            else
            {
                this.globalInstance = new CpuMathOperations();
            }
        }

        public MathOperations GlobalInstance
        {
            get
            {
                return this.globalInstance;
            }
        }

        public Matrix CreateMatrix(int row, int column = 1)
        {
            return new Matrix(this, row, column);
        }
        public Matrix CreateMatrix(float[] data, int row, int column = 1, bool skipMajorConversion = true)
        {
            return new Matrix(this, data, row, column, skipMajorConversion);
        }

        public void Dispose()
        {
            this.globalInstance.Dispose();
        }
    }
}
