using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    public static class ArrayUtilities
    {
        public static float[] GetArrayWithRandomValues(int length)
        {
            float[] retVal = new float[length];
            Random rand = new Random();
            for (int i = 0; i < retVal.Length; i++)
            {
                retVal[i] = (float)rand.NextDouble();
                if (rand.NextDouble() > 0.5)
                {
                    retVal[i] *= -1;
                }
            }
            return retVal;
        }
    }
}
