namespace G2_2D_LZ.Contracts
{
    public interface IReader
    {
        byte[,] LoadImageInMemory(string inputFileName);
    }
}
