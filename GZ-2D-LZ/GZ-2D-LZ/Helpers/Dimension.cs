using System;

namespace G2_2D_LZ.Helpers
{
    public class Dimension
    {
        public uint Width;
        public uint Height;

        public Dimension(uint width, uint height)
        {
            Width = width;
            Height = height;
        }

        public Dimension(int width, int height)
        {
            Width = Convert.ToUInt32(width);
            Height = Convert.ToUInt32(height);
        }
    }
}
