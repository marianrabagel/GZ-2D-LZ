using System;
using G2_2D_LZ.Contracts;

namespace G2_2D_LZ.Predictors
{
    public class ABasedPredictor: AbstractPredictor
    {
        public override int GetPredictionValue(int x, int y)
        {
            if (x == 0)
            {
                return 128;
            }

            return Convert.ToInt32(Matrix[y, x - 1]);
        }
    }
}
