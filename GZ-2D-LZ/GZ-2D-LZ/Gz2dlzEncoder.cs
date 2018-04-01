using System;
using System.IO;
using BitOperations;
using BitOperations.Contracts;
using G2_2D_LZ.Contracts;
using G2_2D_LZ.Helpers;
using G2_2D_LZ.Writer;

namespace G2_2D_LZ
{
    public class Gz2DlzEncoder
    {
        private readonly string _inputFilePath;
        private const string IntermediaryFileExtension = ".mat";

        private readonly byte[,] _originalImage;
        private readonly int _height;
        private readonly int _width;

        public bool[,] IsMatchFound { get; private set; } //has true when a suitable match for a block is found
        public bool[,] IsPixelEncoded { get; private set; } //has true when a pixel has been encoded
        public PixelLocation[,] MatchLocation { get; private set; } //position of the match relative to the block being encoded
        public Dimensions[,] MatchDimensions { get; private set; } //width and heigth of the block being encoded
        public int[,] Residual { get; private set; } //difference between the pixel in the actual block and the matching block
        public byte[,] WorkImage { get; private set; }

        private readonly AbstractPredictor _abstractPredictor;

        public Gz2DlzEncoder(string inputFilePath, AbstractPredictor abstractPredictor, IReader reader)
        {
            //todo guard conditions
            _inputFilePath = inputFilePath;
            _originalImage = reader.GetImageFromFile(inputFilePath);

            _height = _originalImage.GetLength(0);
            _width = _originalImage.GetLength(1);

            InstatiateTables();

            CopyOriginalImageWorkImage();
            _abstractPredictor = abstractPredictor;
        }

        public void WriteMatrixToFileAsText()
        {
            TxtWriter txtWriter = new TxtWriter(_inputFilePath + IntermediaryFileExtension);;

            txtWriter.Write(WorkImage.GetLength(1) + " ");
            txtWriter.Write(WorkImage.GetLength(0) + " ");
            txtWriter.Write(Environment.NewLine);

            txtWriter.WriteMatchFlagToFile(IsMatchFound);
            txtWriter.WriteMatchLocationToFile(MatchLocation);
            txtWriter.WriteMatchDimensionsToFile(MatchDimensions);
            txtWriter.WriteMatrixToFile(Residual);
            txtWriter.WriteMatrixToFile(_abstractPredictor.PredictionError);
        }

        public void Encode()
        {
            _abstractPredictor.SetOriginalMatrix(WorkImage);

            PredictFirstRow();
            EncodeWorkImage();
            WriteResultingMatricesToIndividualFiles();
        }

        private void WriteResultingMatricesToIndividualFiles()
        {
            var fileName = Path.GetFileName(_inputFilePath);
            Directory.CreateDirectory(_inputFilePath + ".folder");
            var outputFileName = _inputFilePath + ".folder\\" + fileName + "." + nameof(IsPixelEncoded) + IntermediaryFileExtension;

            using (IBitWriter bitWriter = new BitWriter(outputFileName))
            {
                for (int y = 0; y < IsPixelEncoded.GetLength(0); y++)
                {
                    for (int x = 0; x < IsPixelEncoded.GetLength(1); x++)
                    {
                        var bit = IsPixelEncoded[y,x] ? 0X01 : 0X00;

                        bitWriter.WriteBit(bit);
                    }
                }
            }
        }

        private void EncodeWorkImage()
        {
            for (int y = 1; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (!IsPixelEncoded[y, x])
                    {
                        var rootPoint = new PixelLocation(GetRootX(x), GetRootY(y));
                        var encoderPoint = new PixelLocation(x, y);

                        var bestMatch = LocateTheBestAproximateMatchForGivenRootPixel(encoderPoint, rootPoint);

                        if (bestMatch.Size > Constants.MinMatchSize)
                        {
                            IsMatchFound[y, x] = true;
                            MatchDimensions[y, x] = new Dimensions(bestMatch.Width, bestMatch.Height);
                            MatchLocation[y, x] = rootPoint;
                            SetResidualAndIsPixelEncoded(x, y);
                        }
                        else
                        {
                            IsMatchFound[y, x] = false;
                            PredictNoMatchBlock(x, y);
                            x += Constants.NoMatchBlockWidth - 1;
                        }
                    }
                }
            }
        }

        public BestMatch LocateTheBestAproximateMatchForGivenRootPixel(PixelLocation encoderPoint, PixelLocation rootPoint)
        {
            BestMatch bestMatch = new BestMatch();
            var rowOffset = 0;
            var widthOfTheMatchInThePreviousRow = _width - 1 - encoderPoint.X;

            do
            {
                var colOffset = 0;
                var nextRootPoint = new PixelLocation(rootPoint.X + colOffset, rootPoint.Y + rowOffset);

                if (IsPixelEncoded[nextRootPoint.Y, nextRootPoint.X] ||
                    nextRootPoint.Y >= encoderPoint.Y && nextRootPoint.X >= encoderPoint.X)
                {
                    do
                    {
                        var nextToBeEncoded = new PixelLocation(encoderPoint.X + colOffset, encoderPoint.Y + rowOffset);

                        if (encoderPoint.X + colOffset == _width ||
                            encoderPoint.Y + rowOffset == _height)
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
                var blockDimensions = new Dimensions(widthOfTheMatchInThePreviousRow, rowOffset);
                var matchMse = GetMse(encoderPoint, rootPoint, blockDimensions);

                if (matchSize >= bestMatch.Size && matchMse <= Constants.MaxMse)
                {
                    bestMatch.Height = rowOffset;
                    bestMatch.Width = widthOfTheMatchInThePreviousRow;
                    bestMatch.Size = matchSize;
                }
            } while (widthOfTheMatchInThePreviousRow != 0 && encoderPoint.Y + rowOffset != _height - 1);//condition different than the one from the paper

            return bestMatch;
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
                        _abstractPredictor.PredictionError[y + i, x + j] = WorkImage[y + i, x + j] - _abstractPredictor.GetPredictionValue(x + j, y + i);
                    }
                }
            }
        }

        private void PredictFirstRow()
        {
            for (int i = 0; i < _width; i++)
            {
                IsPixelEncoded[0, i] = true;
                _abstractPredictor.PredictionError[0, i] = WorkImage[0, i] - _abstractPredictor.GetPredictionValue(i, 0);
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

        private int GetMse(PixelLocation encoderPoint, PixelLocation matchedPoint, Dimensions blockDimensions)
        {
            var sum = 0;

            for (int i = 0; i < blockDimensions.Height; i++)
            {
                for (int j = 0; j < blockDimensions.Width; j++)
                {
                    var nextX = encoderPoint.X + j;
                    if (nextX < _width)
                    {
                        sum += (int) Math.Pow(WorkImage[encoderPoint.Y + i, nextX] -
                                              WorkImage[matchedPoint.Y + i, matchedPoint.X + j], 2);
                    }
                }
            }

            return sum / blockDimensions.Height * blockDimensions.Width;
        }

        private void InstatiateTables()
        {
            IsMatchFound = new bool[_height, _width];
            IsPixelEncoded = new bool[_height, _width];
            MatchLocation = new PixelLocation[_height, _width];
            MatchDimensions = new Dimensions[_height, _width];
            Residual = new int[_height, _width];
            WorkImage = new byte[_height, _width];
        }

        private void CopyOriginalImageWorkImage()
        {
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    WorkImage[y, x] = _originalImage[y, x];
                }
            }
        }
    }
}
