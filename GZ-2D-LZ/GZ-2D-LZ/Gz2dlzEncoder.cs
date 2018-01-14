﻿using System.Drawing;
using GZ_2D_LZ;

namespace G2_2D_LZ
{
    public class Gz2dlzEncoder : BaseClass
    {
        private byte[,] _originalImage;

        public Gz2dlzEncoder(string inputFileName) : base(inputFileName)
        {
            SaveImageInMemory();

            var height = _originalImage.GetLength(0);
            var width = _originalImage.GetLength(1);

            InitializeTables(height, width);
        }

        public void Encode()
        {
            //predict one row of pixels and record each prediction error in the prediction error table
            //advance the encoder to the next unencoded pixel
            //while (more pixels to be encoded)
                //locate the best match in search region
                //if( match was acceptable )
                    // record true in the match flag table
                    // update the match dimension, location, residual tables
                //if( the match was unacceptable)
                    //record false in the match flag table
                    //predict a block of pixels and recod the prediction errors in the prediction error table
                //next unencoded pixel
            // end while
            // statistically encode the matching tables
            //output the encoded image
        }

        private BestMatch LocateTheBestAproximateMatchForAGivenRootPixel(int x, int y, int rootX, int rootY)
        {
            var workImage = CloneOriginalImage();

            BestMatch bestMatch = new BestMatch();
            var rowOffset = 0;
            var widthOfTheMatchInThePreviousRow = GetOriginalImage().GetLength(1) - x;

            do
            {
                var colOffset = 0;
                do
                {
                    var yIndex = rootY + rowOffset;
                    var xIndex = rootX + colOffset;
                    if (isPixelEncoded[yIndex, xIndex] ||
                        (yIndex >= y && xIndex >= x))
                    {
                        if (isPixelEncoded[y + rowOffset, x + colOffset])
                        {
                            colOffset++;
                        }
                        else
                        {
                            var pixelToBeEncoded = workImage[y + rowOffset, x + rowOffset];
                            var possibleMatchPixel = workImage[yIndex, xIndex];
                            if (pixelToBeEncoded - possibleMatchPixel <= Constants.Threshold)
                            {
                                colOffset++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                } while (colOffset == widthOfTheMatchInThePreviousRow);
                widthOfTheMatchInThePreviousRow = colOffset;
                rowOffset++;
                var matchSize = 0; // the number of possible newly encoded pixels inside the block identified
                //by widthOfTheMatchInThePreviousRow x rowOffset
                var matchMse = 0; // the mse between those possible newly encoded pixels and their corresponding 
                //pixels inside the matched area
                if (matchSize >= bestMatch.bestMatchSize && matchMse <= Constants.MaxMse)
                {
                    bestMatch.bestMatchHeight = rowOffset;
                    bestMatch.bestMatchWidth = widthOfTheMatchInThePreviousRow;
                    bestMatch.bestMatchSize = matchSize;
                }
            } while (widthOfTheMatchInThePreviousRow == 0 || y + rowOffset == workImage.GetLength(0));

            return bestMatch;
        }

        private byte[,] CloneOriginalImage()
        {
            byte[,] workImage = new byte[_originalImage.GetLength(0), _originalImage.GetLength(1)];

            for (int y = 0; y < _originalImage.GetLength(0); y++)
            {
                for (int x = 0; x < _originalImage.GetLength(1); x++)
                {
                    workImage[y, x] = _originalImage[y, x];
                }
            }

            return workImage;
        }

        private void SaveImageInMemory()
        {
            using (Bitmap bitmap = new Bitmap(_inputFileName))
            {
                _originalImage = new byte[bitmap.Height, bitmap.Width];

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        _originalImage[y, x] = bitmap.GetPixel(x, y).R;
                    }
                }
            }
        }

        public byte[,] GetOriginalImage()
        {
            return _originalImage;
        }
    }
}
