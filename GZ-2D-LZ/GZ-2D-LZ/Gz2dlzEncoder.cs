using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using G2_2D_LZ.Helpers;

namespace G2_2D_LZ
{
    public class Gz2dlzEncoder : BaseClass
    {
        private byte[,] _originalImage;

        public Gz2dlzEncoder(string inputFileName) : base(inputFileName)
        {
            SaveImageInMemory();
            InitializeTables(_originalImage.GetLength(0), _originalImage.GetLength(1));
        }

        public void WriteMatrixToFileAsText()
        {
            using (StreamWriter streamWriter = new StreamWriter(InputFileName + ".mat"))
            {
                streamWriter.Write(WorkImage.GetLength(1) + " ");
                streamWriter.Write(WorkImage.GetLength(0) + " ");
                streamWriter.WriteLine();

                WriteMatchFlagToFile(streamWriter);
                WriteMatchLocationToFile(streamWriter);
                WriteMatchDimensionsToFile(streamWriter);
                WriteMatrixToFile(Residual, streamWriter);
                WriteMatrixToFile(PredictionError, streamWriter);
            }
        }

        private void WriteMatrixToFile(int[,] matrix, StreamWriter streamWriter)
        {
            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    var format = matrix[y, x] + " ";
                    streamWriter.Write(format);
                }
                streamWriter.WriteLine();
            }
        }

        private void WriteMatchDimensionsToFile(StreamWriter streamWriter)
        {
            for (int y = 0; y < MatchDimensions.GetLength(0); y++)
            {
                for (int x = 0; x < MatchDimensions.GetLength(1); x++)
                {
                    var value = MatchDimensions[y, x] ?? new BlockDimension(0, 0);

                    streamWriter.Write(value.Width + " ");
                    streamWriter.Write(value.Height + " ");
                }
                streamWriter.WriteLine();
            }
        }

        private void WriteMatchLocationToFile(StreamWriter streamWriter)
        {
            for (int y = 0; y < MatchLocation.GetLength(0); y++)
            {
                for (int x = 0; x < MatchLocation.GetLength(1); x++)
                {
                    var value = MatchLocation[y, x] ?? new PixelLocation(0, 0);
                    streamWriter.Write(value.X + " ");
                    streamWriter.Write(value.Y + " ");
                }
                streamWriter.WriteLine();
            }
        }

        private void WriteMatchFlagToFile(StreamWriter streamWriter)
        {
            bool[,] matrix = MatchFlag;

            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                for (int x = 0; x < matrix.GetLength(1); x++)
                {
                    var value = matrix[y, x] ? 1 : 0;
                    streamWriter.Write(value + " ");
                }
                streamWriter.WriteLine();
            }
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
                            MatchLocation[y, x] = rootPoint;
                            SetResidualAndIsPixelEncoded(x, y);
                        }
                        else
                        {
                            MatchFlag[y, x] = false;
                            PredictNoMatchBlock(x, y); 
                            x += Constants.NoMatchBlockWidth - 1;
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

        private void PredictNoMatchBlock(int x, int y)
        {
            for (int i = 0; i < Constants.NoMatchBlockHeight; i++)
            {
                for (int j = 0; j < Constants.NoMatchBlockWidth; j++)
                {
                    if (y + i < WorkImage.GetLength(0) && x + j < WorkImage.GetLength(1))
                    {
                        IsPixelEncoded[y + i, x + j] = true;
                        PredictionError[y + i, x + j] = WorkImage[y + i, x + j] -
                                                        GetPredictionValue(x + j, y + i);
                    }
                }
            }
        }

        private void SetResidualAndIsPixelEncoded(int encoderX, int encoderY)
        {
            var matchDimension = MatchDimensions[encoderY, encoderX];
            var matchLocation = MatchLocation[encoderY, encoderX];

            for (int i = 0; i < matchDimension.Height; i++)
            {
                for (int j = 0; j < matchDimension.Width; j++)
                {
                    Residual[encoderY + i, encoderX + j] = WorkImage[encoderY + i, encoderX + j] -
                                                           WorkImage[matchLocation.Y + i, matchLocation.X + j];
                    IsPixelEncoded[encoderY + i, encoderX + j] = true;
                }
            }
        }

        private int GetRootY(int y)
        {
            var rootY = y - Constants.SearchHeight;

            if (rootY < 0)
            {
                rootY = 0;
            }

            return rootY;
        }

        private int GetRootX(int x)
        {
            var rootX = x - Constants.SearchWidth;

            if (rootX < 0)
            {
                rootX = 0;
            }

            return rootX;
        }

        public BestMatch  
            LocateTheBestAproximateMatchForGivenRootPixel(PixelLocation encoderPoint, PixelLocation rootPoint)
        {
            BestMatch bestMatch = new BestMatch();
            var rowOffset = 0;
            var widthOfTheMatchInThePreviousRow = GetOriginalImage().GetLength(1) - 1 - encoderPoint.X;

            do
            {
                var colOffset = 0;
                var nextRootPoint = new PixelLocation(rootPoint.X + colOffset, rootPoint.Y + rowOffset);
                if (IsPixelEncoded[nextRootPoint.Y, nextRootPoint.X] ||
                    (nextRootPoint.Y >= encoderPoint.Y && nextRootPoint.X >= encoderPoint.X))
                {
                    do
                    {
                        var nextToBeEncoded = new PixelLocation(encoderPoint.X + colOffset, encoderPoint.Y + rowOffset);
                        if (encoderPoint.X + colOffset == WorkImage.GetLength(1) ||
                            encoderPoint.Y + rowOffset == WorkImage.GetLength(0))
                        {
                            break;
                        }
                        
                        if (IsPixelEncoded[nextToBeEncoded.Y, nextToBeEncoded.X])
                        {
                            colOffset++;
                        }
                        else
                        {
                            var pixelToBeEncoded = WorkImage[nextToBeEncoded.Y, nextToBeEncoded.X];
                            var possibleMatchPixel = WorkImage[nextRootPoint.Y, nextRootPoint.X];

                            if (Math.Abs(pixelToBeEncoded - possibleMatchPixel) <= Constants.Threshold)
                            {
                                colOffset++;
                            }
                            else
                            {
                                break;
                            }

                        }
                    } while (colOffset != widthOfTheMatchInThePreviousRow);//condition different than the one from the paper
                }

                widthOfTheMatchInThePreviousRow = colOffset;
                rowOffset++;

                var matchSize = widthOfTheMatchInThePreviousRow * rowOffset;
                var matchMse = GetMse(encoderPoint, rootPoint, widthOfTheMatchInThePreviousRow, rowOffset);

                if (matchSize >= bestMatch.Size && matchMse <= Constants.MaxMse)
                {
                    bestMatch.Height = rowOffset;
                    bestMatch.Width = widthOfTheMatchInThePreviousRow;
                    bestMatch.Size = matchSize;
                }
            } while (widthOfTheMatchInThePreviousRow != 0 && encoderPoint.Y + rowOffset != WorkImage.GetLength(0) - 1);//condition different than the one from the paper

            return bestMatch;
        }

        private int GetMse(PixelLocation encoderPoint, PixelLocation matchedPoint, int matchedBlockWidth, int matchBlockHeight)
        {
            var sum = 0;

            for (int i = 0; i < matchBlockHeight; i++)
            {
                for (int j = 0; j < matchedBlockWidth; j++)
                {
                    var index1 = encoderPoint.X + j;
                    if (index1 < WorkImage.GetLength(1) - 1)
                    {
                        sum += (int) Math.Pow(WorkImage[encoderPoint.Y + i, index1] -
                                              WorkImage[matchedPoint.Y + i, matchedPoint.X + j], 2);
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

        public bool[,] GetEncodedPixels()
        {
            return IsPixelEncoded;
        }
    }
}
