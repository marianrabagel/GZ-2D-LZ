using System;
using System.Configuration;
using G2_2D_LZ.Contracts;

namespace G2_2D_LZ.Predictors
{
    public class ABasedPredictor: IPredictor
    {
        private object[,] Matrix { get; set; }

        public void SetOriginalMatrix<T>(T[,] matrix)
        {
            Matrix = new object[matrix.GetLength(0), matrix.GetLength(1)];

            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    Matrix[y, x] = matrix[y, x];
                }
            }
        }

        public int GetPredictionValue(int x, int y)
        {
            if (x == 0)
            {
                return 128;
            }

            return Convert.ToInt32(Matrix[y, x - 1]);
        }
    }
}
