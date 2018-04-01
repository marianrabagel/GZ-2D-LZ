using System;

namespace G2_2D_LZ.Helpers
{
    public class PixelLocation
    {
        public uint Y;
        public uint X;

        public PixelLocation(int x, int y)
        {
            X = Convert.ToUInt32(x);
            Y = Convert.ToUInt32(y);
        }

        public PixelLocation(uint x, uint y)
        {
            X = x;
            Y = y;
        }

        public PixelLocation()
        {
        }
    }
}
