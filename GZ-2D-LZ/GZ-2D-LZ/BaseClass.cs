using G2_2D_LZ.Helpers;

namespace G2_2D_LZ
{
    public abstract class BaseClass
    {
        protected string InputFileName;
        protected bool[,] MatchFlag; //has true when a suitable match for a block is found
        public bool[,] IsPixelEncoded; //has true when a pixel has been encoded
        protected PixelLocation[,] MatchLocation; //position of the match relative to the block being encoded
        protected BlockDimension[,] MatchDimensions; //widht and heigth of the block being encoded
        protected double[,] Residual; //difference between the pixel in the actual block and the matching block
        protected double[,] PredictionError; // prediction error values
        public byte[,] WorkImage;

        protected BaseClass(string inputFileName)
        {
            InputFileName = inputFileName;
        }

        protected void InitializeTables(int height, int width)
        {
            MatchFlag = new bool[height, width];
            IsPixelEncoded = new bool[height, width];
            MatchLocation = new PixelLocation[height, width];
            MatchDimensions = new BlockDimension[height, width];
            Residual = new double[height, width];
            PredictionError = new double[height, width];
        }
    }
}
