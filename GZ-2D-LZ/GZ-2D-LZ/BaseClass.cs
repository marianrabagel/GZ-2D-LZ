using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G2_2D_LZ
{
    public abstract class BaseClass
    {
        protected string _inputFileName;
        protected bool[,] matchFlag; //has true when a suitable match for a block is found
        protected bool[,] isPixelEncoded; //has true when a pixel has been encoded
        protected PixelLocation[,] matchLocation; //position of the match relative to the block being encoded
        protected BlockDimension[,] matchDimenstions; //widht and heigth of the block being encoded
        protected double[,] residual; //difference between the pixel in the actual block and the matching block
        protected double[,] predtionError; // prediction error values
        

        protected BaseClass(string inputFileName)
        {
            _inputFileName = inputFileName;
        }

        protected void InitializeTables(int height, int width)
        {
            matchFlag = new bool[height, width];
            isPixelEncoded = new bool[height, width];
            matchLocation = new PixelLocation[height, width];
            matchDimenstions = new BlockDimension[height, width];
            residual = new double[height, width];
            predtionError = new double[height, width];
        }
    }
}
