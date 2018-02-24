
namespace G2_2D_LZ.Contracts
{
    public interface IPredictor
    {
        int GetPredictionValue(int x, int y);
        void SetOriginalMatrix<T>(T[,] matrix);
    }
}
