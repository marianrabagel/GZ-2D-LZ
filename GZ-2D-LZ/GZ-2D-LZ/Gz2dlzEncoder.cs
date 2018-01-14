using System.Drawing;

namespace G2_2D_LZ
{
    public class Gz2dlzEncoder
    {
        private string _inputFileName;
        private byte[,] originalImage;

        private bool[,] matchFlag; //has true when a suitable mathc for a block is found
        private PixelLocation[,] matchLocation; //position of the match relative to the blopck being encoded
        private BlockDimension[,] matchDimenstions; //widht and heigth of the block being encoded
        private double[,] residual; //difference between the pixel in the actual block and the matching block
        private double[,] predtionError; // prediction error values
        

        public Gz2dlzEncoder(string inputFileName)
        {
            _inputFileName = inputFileName;
            SaveImageInMemory();
            var height = originalImage.GetLength(0);
            var width = originalImage.GetLength(1);
            matchFlag = new bool[height, width];
            matchLocation = new PixelLocation[height, width];
            matchDimenstions = new BlockDimension[height, width];
            residual = new double[height, width];
            predtionError = new double[height, width];
        }

        public void Encode()
        {
            //predict one row of pixels and record each prediction error in the prediction error table
            // advance the encoder to the next unencoded pixel
            //while (more pixels to be encoded)
                //locate the best match in search region
                //if( math was acceptable )
                    // record true in the match flag table
                    // update teh match dimension, location, residual tables
                //if( the match was unacceptable)
                    //record false in the match flag table
                    //predict a block of pixels and recod the prediction errors in the prediction error table
                //next unencoded pixel
            // end while
            // statistically encode the matching tables
            //output the encoded image
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
