namespace GZ_2D_LZ.Archiver.Contracts
{
    public interface IArchiver
    {
        string Compress(string inputFolderPath, string outputName = null, int compresionRate = 3);
        string Decompress(string archivePath);
    }
}
