using ManagedCuda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CudaLibManaged
{
    public class Matrix : IDisposable
    {
        private MathOperationManager mathManager;
        private bool isDeviceDataInitialized;
        public Matrix(MathOperationManager mathManager, int row, int column = 1)
        {
            this.mathManager = mathManager;
            this.Row = row;
            this.Column = column;
            this.isDeviceDataInitialized = false;
            this.HostData = new float[this.Row * this.Column];
            this.SetZero();
        }
        public Matrix(MathOperationManager mathManager, float[] value, int row, int column = 1, bool skipMajorConversion = true)
        {
            if (value.Count() != row * column)
            {
                throw new ArgumentException("Length of data != row*column");
            }

            this.mathManager = mathManager;
            this.Row = row;
            this.Column = column;
            this.isDeviceDataInitialized = false;
            this.HostData = new float[this.Row * this.Column];
            SetValue(value, skipMajorConversion);
        }

        public void SetValue(float[] value, bool skipMajorConversion = true)
        {
            if (value.Count() != this.Row * this.Column)
            {
                throw new ArgumentException("Length of data != row*column");
            }

            if (skipMajorConversion)
            {
                value.CopyTo(this.HostData, 0);
                if (this.mathManager.MathType == MathType.GPU)
                {
                    if (this.isDeviceDataInitialized)
                    {
                        this.CopyToCuda();
                    }
                    else
                    {
                        this.DeviceData = this.HostData;
                    }
                    this.isDeviceDataInitialized = true;
                }
            }
            else
            {
                this.SetZero();
                this.ConvertFromRowMajorToColumnMajor(value);
            }
        }

        private void ConvertFromRowMajorToColumnMajor(float[] value)
        {
            using (Matrix m = this.mathManager.CreateMatrix(value, this.Column, this.Row, skipMajorConversion: true))
            {
                this.mathManager.GlobalInstance.MatrixTranspose(m, this, copyOutputsFromGpuToCpu: true);
            }
        }

        public void SetZero()
        {
            var tmp = new float[this.Row * this.Column];
            SetValue(tmp, skipMajorConversion: true);
        }

        public float[] GetValue()
        {
            if (this.mathManager.MathType == MathType.GPU)
            {
                this.DeviceData.CopyToHost(this.HostData);
            }
            return this.HostData;
        }

        public void Resize(int row, int column = 1)
        {
            this.Row = row;
            this.Column = column;
            this.HostData = new float[this.Row * this.Column];
            this.DisposeDeviceData();
            this.SetZero();
        }

        public void Resize(float[] value, int row, int column = 1)
        {
            this.Row = row;
            this.Column = column;
            this.HostData = new float[this.Row * this.Column];
            this.DisposeDeviceData();
            this.SetValue(value);
        }

        internal void CopyFromCuda()
        {
            this.DeviceData.CopyToHost(this.HostData);
        }

        internal void CopyToCuda()
        {
            this.DeviceData.CopyToDevice(this.HostData);
        }

        public int Row
        {
            get;
            private set;
        }
        public int Column
        {
            get;
            private set;
        }
        /// <summary>
        /// Data in CPU memory
        /// </summary>
        internal float[] HostData
        {
            get;
            private set;
        }
        /// <summary>
        /// Data in GPU memory
        /// </summary>
        internal CudaDeviceVariable<float> DeviceData
        {
            get;
            private set;
        }
        internal void DisposeDeviceData()
        {
            this.DeviceData.Dispose();
            this.isDeviceDataInitialized = false;
        }

        public void Dispose()
        {
            this.DisposeDeviceData();
        }
    }
}
