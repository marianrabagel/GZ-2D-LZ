using System;
using G2_2D_LZ.Contracts;

namespace G2_2D_LZ.Predictors
{
    public class CalicPredictor : AbstractPredictor
    {
        public override int GetPredictionValue(int x, int y)
        {
            var width = Matrix.GetLength(1) - 1;

            var onePositionLeft = x - 1 < 0 ? 0 : x-1;
            var onePositionRight = x + 1 > width ? width : x + 1;
            var onePositionUp = y - 1 < 0 ? 0 : y - 1;
            var twoPositionsUp = y - 2 < 0 ? 0 : y - 2;
            var twoPositionsLeft = x - 2 < 0 ? 0 : x - 1;

            var w = Matrix[y, onePositionLeft];
            var ww = Matrix[y, twoPositionsLeft];
            var n = Matrix[onePositionUp, x];
            var nn = Matrix[twoPositionsUp, x];
            var nw = Matrix[onePositionUp, onePositionLeft];
            var ne = Matrix[onePositionUp, onePositionRight];
            var nne = Matrix[twoPositionsUp, onePositionRight];

            if (y == 0)
            {
                n = 0;
                nn = 0;
                ne = 0;
                nne = 0;
            }
            if (y == 1)
            {
                nn = 0;
                nne = 0;
            }

            var dh = Math.Abs(w - ww) + Math.Abs(n - nw) + Math.Abs(ne - n);
            var dv = Math.Abs(w - nw) + Math.Abs(n - nn) + Math.Abs(ne - nne);
            var dhMinusDv = dh - dv;

            if (IsSharpHorizontalEdge(dhMinusDv))
            {
                return n;
            }

            if (IsSharpVerticalEdge(dhMinusDv))
            {
                return w;
            }

            var prediction = (n + w) / 2 + (ne - nw) / 4;

            if (IsHorizontalEdge(dhMinusDv))
            {
                return (prediction + n) / 2;
            }

            if (IsVerticalEdge(dhMinusDv))
            {
                return (prediction + w) / 2;
            }

            if (IsWeakHorizontalEdge(dhMinusDv))
            {
                return (3 * prediction + n) / 4;
            }

            if (IsWeakVerticalEdge(dhMinusDv))
            {
                return (3 * prediction + w) / 4;
            }

            return prediction;
        }

        private static bool IsWeakVerticalEdge(int dhMinusDv)
        {
            return dhMinusDv < -8;
        }

        private static bool IsWeakHorizontalEdge(int dhMinusDv)
        {
            return dhMinusDv > 8;
        }

        private static bool IsVerticalEdge(int dhMinusDv)
        {
            return dhMinusDv < -32;
        }

        private static bool IsHorizontalEdge(int dhMinusDv)
        {
            return dhMinusDv > 32;
        }

        private static bool IsSharpHorizontalEdge(int dhMinusDv)
        {
            return dhMinusDv > 80;
        }

        private static bool IsSharpVerticalEdge(int dhMinusDv)
        {
            return dhMinusDv < -80;
        }
    }
}
