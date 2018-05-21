﻿namespace GZ_2D_LZ.UnitTests.Common
{
    public class Constants
    {
        public static byte[,] TestBmp =
        {
            {0, 255, 0, 255, 0, 255},
            {0, 255, 0, 255, 0, 0},
            {255, 0, 0, 0, 255, 255},
            {0, 255, 0, 255, 0, 255},
            {255, 0, 255, 255, 0, 0}
        };

        public static byte[,] Image3X4Block =
        {
            {192, 192, 192, 192, 192, 192, 192, 192, 192, 192},
            {96, 0, 0, 0, 0, 192, 96, 192, 96, 0},
            {192, 0, 0, 0, 0, 96, 192, 0, 255, 96},
            {64, 0, 0, 0, 0, 255, 96, 255, 96, 0},
            {255, 0, 0, 0, 0, 96, 192, 96, 192, 96},
            {192, 96, 255, 192, 255, 0, 0, 0, 0, 64},
            {64, 192, 96, 255, 96, 0, 0, 0, 0, 64},
            {192, 64, 192, 96, 192, 0, 0, 0, 0, 64},
            {64, 255, 96, 255, 96, 0, 0, 0, 0, 64},
            {0, 96, 192, 64, 255, 64, 255, 64, 255, 192}
        };

        private static byte[,] _image2PossibleBlocks =
        {
            {192, 192, 192, 192, 192, 192, 192, 192, 192, 192},
            {192, 192, 192, 192, 192, 192, 128, 192, 128, 0},
            {192, 192, 192, 192, 192, 192, 192, 0, 255, 128},
            {192, 192, 192, 0, 0, 255, 128, 255, 128, 0},
            {192, 192, 192, 0, 0, 128, 192, 128, 192, 128},
            {192, 192, 192, 0, 0, 0, 0, 0, 0, 128},
            {96, 192, 128, 255, 128, 0, 0, 0, 0, 96},
            {192, 96, 192, 128, 192, 0, 0, 0, 0, 0},
            {96, 255, 128, 255, 128, 0, 0, 0, 0, 128},
            {0, 128, 192, 96, 255, 128, 255, 128, 255, 128}
        };
    }
}
