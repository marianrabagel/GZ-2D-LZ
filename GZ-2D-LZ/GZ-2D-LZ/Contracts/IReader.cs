namespace G2_2D_LZ.Contracts
{
    public interface IReader
    {
        byte[,] GetImageFromFile(string inputFileName);
    }
}
