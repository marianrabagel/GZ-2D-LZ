using System;
using System.Drawing;
using System.Windows.Forms;
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
            PredictFirstRow();

            for (int y = 1; y < WorkImage.GetLength(0); y++)
            {
                for (int x = 0; x < WorkImage.GetLength(1); x++)
                {

                    if (!IsPixelEncoded[y, x])
                    {
                        var rootPoint = new PixelLocation(GetRootX(x), GetRootY(y));
                        var encoderPoint = new PixelLocation(x, y);

                        var bestMatch = LocateTheBestAproximateMatchForGivenRootPixel(encoderPoint, rootPoint);

                        if (bestMatch.Size > Constants.MinMatchSize)
                        {
                            MatchFlag[y, x] = true;
                            MatchDimensions[y, x] = new BlockDimension(bestMatch.Width, bestMatch.Height);
                            MatchLocation[y, x] = new PixelLocation(x - bestMatch.Width, y - bestMatch.Height);
                            SetResidualValue(x, y);
                        }
                        else
                        {
                            MatchFlag[y, x] = false;
                            for (int yy = 0; yy < Constants.NoMatchBlockWidth; yy++)
                            {
                                for (int xx = 0; xx < Constants.NoMatchBlockWidth; xx++)
                                {
                                    if (y + yy > WorkImage.GetLength(0) - 1 || x + xx > WorkImage.GetLength(1) -1 )
                                    {
                                        continue;
                                    }
                                    IsPixelEncoded[y + yy, x + xx] = true;
                                    PredictionError[y + yy, x + xx] = WorkImage[y + yy, x + xx] -
                                                                      GetPredictionValue(x + xx, y + yy);
                                }
                            }
                        }
                    }
                }
            }

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

        private void SetResidualValue(int encoderX, int encoderY)
        {
            var matchDimension = MatchDimensions[encoderY, encoderX];
            var matchLocation = MatchLocation[encoderY, encoderX];

            for (int y = 0; y < matchDimension.Height; y++)
            {
                for (int x = 0; x < matchDimension.Width; x++)
                {
                    Residual[encoderY + y, encoderX + x] = WorkImage[encoderY + y, encoderX + x] -
                                                           WorkImage[matchLocation.Y + y, matchLocation.X + x];
                }
            }
        }

        private int GetRootY(int y)
        {
            var rootY = y - Constants.SearchHeight;

            if (rootY < 0)
            {
                rootY = y;
            }

            return rootY;
        }

        private int GetRootX(int x)
        {
            var rootX = x - Constants.SearchWidth;

            if (rootX < 0)
            {
                rootX = x;
            }

            return rootX;
        }

        private BestMatch LocateTheBestAproximateMatchForGivenRootPixel(PixelLocation encoderPoint, PixelLocation rootPoint)
        {
            BestMatch bestMatch = new BestMatch();
            var rowOffset = 0;
            var widthOfTheMatchInThePreviousRow = GetOriginalImage().GetLength(1) - 1 - encoderPoint.X;

            do
            {
                var colOffset = 0;
                do
                {
                    var nextRootPoint = new PixelLocation(rootPoint.X + colOffset, rootPoint.Y + rowOffset);

                    if (IsPixelEncoded[nextRootPoint.Y, nextRootPoint.X] ||(
                        nextRootPoint.Y >= encoderPoint.Y && nextRootPoint.X >= encoderPoint.X))
                    {
                        var nextToBeEncoded = new PixelLocation(encoderPoint.X + colOffset, encoderPoint.Y + rowOffset);

                        if (IsPixelEncoded[nextToBeEncoded.Y, nextToBeEncoded.X])
                        {
                            colOffset++;
                        }
                        else
                        {
                            var pixelToBeEncoded = WorkImage[nextToBeEncoded.Y, nextToBeEncoded.X];
                            var possibleMatchPixel = WorkImage[nextRootPoint.Y, nextRootPoint.X];

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
                var matchSize = widthOfTheMatchInThePreviousRow * rowOffset;
                PixelLocation matchedPoint = new PixelLocation
                {
                    X = rootPoint.X + widthOfTheMatchInThePreviousRow,
                    Y = rootPoint.Y + rowOffset
                };
                var matchMse = GetMse(encoderPoint, matchedPoint, widthOfTheMatchInThePreviousRow, rowOffset);
                if (matchSize >= bestMatch.Size && matchMse <= Constants.MaxMse)
                {
                    bestMatch.Height = rowOffset;
                    bestMatch.Width = widthOfTheMatchInThePreviousRow;
                    bestMatch.Size = matchSize;
                }
            } while (widthOfTheMatchInThePreviousRow == 0 || encoderPoint.Y + rowOffset == WorkImage.GetLength(0) - 1);

            return bestMatch;
        }

        private int GetMse(PixelLocation encoderPoint, PixelLocation matchedPoint, int matchedBlockWidth, int matchBlockHeight)
        {
            var sum = 0;

            for (int y = 0; y < matchBlockHeight; y++)
            {
                for (int x = 0; x < matchedBlockWidth; x++)
                {
                    var index1 = encoderPoint.X + x;
                    if (index1 < WorkImage.GetLength(1))
                    {
                        sum += (int) Math.Pow(WorkImage[encoderPoint.Y + y, index1] -
                                              WorkImage[matchedPoint.Y + y, matchedPoint.X + x], 2);
                    }
                }
            }

            return sum / matchBlockHeight * matchedBlockWidth;
        }

        protected void PredictFirstRow()
        {
            var width = WorkImage.GetLength(1);

            for (int i = 0; i < width; i++)
            {
                IsPixelEncoded[0, i] = true;
                PredictionError[0, i] = WorkImage[0, i] - GetPredictionValue(i, 0);
            }
        }

        private int GetPredictionValue(int x, int y)
        {
            if (x == 0)
            {
                return 128;
            }

            return WorkImage[y, x - 1];
        }

        private void CloneOriginalImage(byte[,] workImage)
        {
            workImage = new byte[_originalImage.GetLength(0), _originalImage.GetLength(1)];

            for (int y = 0; y < _originalImage.GetLength(0); y++)
            {
                for (int x = 0; x < _originalImage.GetLength(1); x++)
                {
                    workImage[y, x] = _originalImage[y, x];
                }
            }
        }

        private void SaveImageInMemory()
        {
            using (Bitmap bitmap = new Bitmap(InputFileName))
            {
                _originalImage = new byte[bitmap.Height, bitmap.Width];
                WorkImage = new byte[bitmap.Height, bitmap.Width];

                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        _originalImage[y, x] = bitmap.GetPixel(x, y).R;
                        WorkImage[y, x] = bitmap.GetPixel(x, y).R;
                    }
                }
            }
        }

        public byte[,] GetOriginalImage()
        {
            return _originalImage;
        }

        public double[,] GetPredictionError()
        {
            return PredictionError;
        }

        public double[,] GetResidual()
        {
            return Residual;
        }

        public bool[,] GetMatchFlag()
        {
            return MatchFlag;
        }

        public bool[,] GetEncodedPixels()
        {
            return IsPixelEncoded;
        }

        public PixelLocation[,] GetMatchLocation()
        {
            return MatchLocation;
        }
        public BlockDimension[,] GetMatchDimensions()
        {
            return MatchDimensions;
        }
    }
}
