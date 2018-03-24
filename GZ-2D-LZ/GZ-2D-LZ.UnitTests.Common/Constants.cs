using System;

namespace GZ_2D_LZ.UnitTests.Common
{
    public class Constants
    {
        private static readonly string basePath = "\\TestData\\Encoder\\";
        public static string TestBmpPath = Environment.CurrentDirectory + $"{basePath}test.bmp";
        public static string One4X4MatchBlockBmpPath = Environment.CurrentDirectory + $"{basePath}4x4Block.bmp";
        public static string TwoPossibleMatchBlocksBmpPath = Environment.CurrentDirectory + $"{basePath}2PossibleMatchBlocks.bmp";
        public static string LenaImagePath = Environment.CurrentDirectory + $"{basePath}Lenna256an.bmp";
        public static string LenaTxtImagePath = Environment.CurrentDirectory + $"{basePath}TxtMatrices\\Lenna256an.txt";
        public static string Input2PosibleMatchBlocksTxtPath = Environment.CurrentDirectory + $"{basePath}TxtMatrices\\2PossibleMatchBlocks.txt";

        public static byte[,] TestBmp =
        {
            {0, 255, 0, 255, 0, 255},
            {0, 255, 0, 255, 0, 0},
            {255, 0, 0, 0, 255, 255},
            {0, 255, 0, 255, 0, 255},
            {255, 0, 255, 255, 0, 0}
        };

        public static byte[,] Image4X4Block =
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

        public static byte[,] Image2posibleBlocks =
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
