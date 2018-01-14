using System.Drawing;

namespace G2_2D_LZ
{
    public class Gz2dlzEncoder
    {
        private string _inputFileName;
        private byte[,] originalImage;

        public Gz2dlzEncoder(string inputFileName)
        {
            _inputFileName = inputFileName;
            SaveImageInMemory();
        }

        public void Encode()
        {
            
        }

        private void SaveImageInMemory()
        {
            using (Bitmap bitmap = new Bitmap(_inputFileName))
            {
                originalImage = new byte[bitmap.Height, bitmap.Width];

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        originalImage[y, x] = bitmap.GetPixel(x, y).R;
                    }
                }
            }
        }

        public byte[,] GetOriginalImage()
        {
            return originalImage;
        }
    }
}
