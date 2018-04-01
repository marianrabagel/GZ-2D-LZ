using System.Drawing;
using G2_2D_LZ.Contracts;

namespace G2_2D_LZ.Readers
{
    public class BmpReader : IReader
    {
        public byte[,] GetImageFromFile(string inputFileName)
        {
            using (Bitmap bitmap = new Bitmap(inputFileName))
            {
                var originalImage = new byte[bitmap.Height, bitmap.Width];

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        originalImage[y, x] = bitmap.GetPixel(x, y).R;
                    }
                }

                return originalImage;
            }
        }
    }
}
