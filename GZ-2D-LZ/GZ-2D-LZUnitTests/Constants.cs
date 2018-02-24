using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GZ_2D_LZUnitTests
{
    public class Constants
    {
        public static string InputTestImagePath = Environment.CurrentDirectory + "\\TestData\\test.bmp";
        public static string InputImage512x512Path = Environment.CurrentDirectory + "\\TestData\\test200.bmp";
        public static string Input4x4MatchBlockPath = Environment.CurrentDirectory + "\\TestData\\4x4Block.bmp";
        public static string InputChessPath = Environment.CurrentDirectory + "\\TestData\\testChess.bmp";
        public static string Input2PosibleMatchBlocksPath = Environment.CurrentDirectory + "\\TestData\\2PossibleMatchBlocks.bmp";
        public static string LenaImagePath = Environment.CurrentDirectory + "\\TestData\\Lenna256an.bmp";

        public static byte[,] TestImage =
        {
            {0, 255, 0, 255, 0, 255},
            {0, 255, 0, 255, 0, 0},
            {255, 0, 0, 0, 255, 255},
            {0, 255, 0, 255, 0, 255},
            {255, 0, 255, 255, 0, 0}
        };

        public static byte[,] Image4x4Block =
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
