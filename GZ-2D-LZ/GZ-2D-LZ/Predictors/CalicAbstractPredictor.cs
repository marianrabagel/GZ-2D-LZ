using System;
using G2_2D_LZ.Contracts;

namespace G2_2D_LZ.Predictors
{
    class CalicAbstractPredictor : AbstractPredictor
    {
        public override int GetPredictionValue(int x, int y)
        {
            byte w = 0;
            byte ww = 0;
            byte n = 0;
            byte nn = 0;
            byte nw = 0;
            byte ne = 0;
            byte nne = 0;

            try
            {
                w = Matrix[y, x - 1];
                ww = Matrix[y, x - 2];
                n = Matrix[y - 1, x];
                nn = Matrix[y - 2, x];
                nw = Matrix[y - 1, x - 1];
                ne = Matrix[y - 1, x + 1];
                nne = Matrix[y - 2, x + 1];
            }
            catch (Exception e)
            {
                // ignored
            }

            var dh = Math.Abs(w - ww) + Math.Abs(n - nw) + Math.Abs(ne - n);
            var dv = Math.Abs(w - nw) + Math.Abs(n - nn) + Math.Abs(ne - nne);
            var dhMinusDv = dh - dv;

            if (dhMinusDv > 80)
            {
                return n;
            }
            if (dhMinusDv < -80)
            {
                return w;
            }

            var prediction = (n + w) / 2 + (ne - nw) / 4;

            if (dhMinusDv > 32)
            {
                return (prediction + n) / 2;
            }
            if (dhMinusDv < -32)
            {
                return (prediction + w) / 2;
            }
            if (dhMinusDv > 8)
            {
                return (3 * prediction + n) / 4;
            }
            if (dhMinusDv < -8)
            {
                return (3 * prediction + w) / 4;
            }

            return prediction;
        }
    }
}
