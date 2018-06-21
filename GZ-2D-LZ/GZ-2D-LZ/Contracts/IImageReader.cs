namespace G2_2D_LZ.Contracts
{
    public interface IImageReader
    {
        byte[,]  GetImageFromFile(string inputFileName);
    }
}
