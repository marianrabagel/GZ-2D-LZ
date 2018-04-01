
namespace G2_2D_LZ.Contracts
{
    public abstract class AbstractPredictor
    {
        protected byte[,] Matrix { get; set; }
        public int[,] PredictionError { get; set; }

        public abstract int GetPredictionValue(int x, int y);

        public void InitializePredictionError(int height, int width)
        {
            PredictionError = new int[height, width];
        }

        public void SetOriginalMatrix(byte[,] matrix)
        {
            Matrix = matrix;
        }
    }
}
