using System;
using System.IO;
using BitOperations;
using BitOperations.Contracts;
using G2_2D_LZ.Contracts;
using G2_2D_LZ.Contracts.Facades;
using G2_2D_LZ.Helpers;
using G2_2D_LZ.Writers;

namespace G2_2D_LZ
{
    public class Gz2DlzEncoder
    {
        private readonly byte[,] _originalImage;
        private readonly uint _height;
        private readonly uint _width;

        public bool[,] IsMatchFound { get; private set; } //has true when a suitable match for a block is found
        public bool[,] IsPixelEncoded { get; private set; } //has true when a pixel has been encoded
        public PixelLocation[,] MatchLocation { get; private set; } //position of the match relative to the block being encoded
        public Dimension[,] MatchDimension { get; private set; } //width and heigth of the block being encoded
        public int[,] Residual { get; private set; } //difference between the pixel in the actual block and the matching block
        public byte[,] WorkImage { get; private set; }

        private readonly IGz2DlzEncoderFacade _gz2DlzEncoderFacade;

        public Gz2DlzEncoder(IGz2DlzEncoderFacade gz2DlzEncoderFacade)
        {
            _gz2DlzEncoderFacade = gz2DlzEncoderFacade;
            _originalImage = _gz2DlzEncoderFacade.ImageReader.GetImageFromFile(_gz2DlzEncoderFacade.InputFilePath);

            _height = Convert.ToUInt32(_originalImage.GetLength(0));
            _width = Convert.ToUInt32(_originalImage.GetLength(1));

            InstatiateTables();
            CopyOriginalImageWorkImage();
        }
        
        public void Encode()
        {
            _gz2DlzEncoderFacade.AbstractPredictor.SetOriginalMatrix(WorkImage);
            _gz2DlzEncoderFacade.AbstractPredictor.InitializePredictionError((int) _height, (int)_width);

            PredictFirstRow();
            EncodeWorkImage();
            Directory.CreateDirectory(_gz2DlzEncoderFacade.InputFilePath + Constants.Folder);
            WriteResultingMatricesToIndividualFiles();
            var compress = _gz2DlzEncoderFacade.Archiver.Compress(_gz2DlzEncoderFacade.InputFilePath + Constants.Folder);
        }

        public void WriteMatrixToFileAsText()
        {
            TxtWriter txtWriter = new TxtWriter(_gz2DlzEncoderFacade.InputFilePath + Constants.IntermediaryFileExtension); ;

            txtWriter.Write(WorkImage.GetLength(1) + Constants.Separator.ToString());
            txtWriter.Write(WorkImage.GetLength(0) + Constants.Separator.ToString());
            txtWriter.Write(Environment.NewLine);

            txtWriter.WriteMatchFlagToFile(IsMatchFound);
            txtWriter.WriteMatchLocationToFile(MatchLocation);
            txtWriter.WriteMatchDimensionsToFile(MatchDimension);
            txtWriter.WriteMatrixToFile(Residual);
            txtWriter.WriteMatrixToFile(_gz2DlzEncoderFacade.AbstractPredictor.PredictionError);
        }

        public void WriteResultingMatricesToIndividualFiles()
        {
            //todo move this into its own class
            SaveIsMatchFoundToFile(_gz2DlzEncoderFacade.InputFilePath, nameof(IsMatchFound), IsMatchFound);
            SaveMatchLocationToFile(_gz2DlzEncoderFacade.InputFilePath, nameof(MatchLocation), MatchLocation);
            SaveMatchDimensionsToFile(_gz2DlzEncoderFacade.InputFilePath, nameof(MatchDimension), MatchDimension);
            SaveResidualToFile(_gz2DlzEncoderFacade.InputFilePath, nameof(Residual), Residual);
            SaveResidualToFile(_gz2DlzEncoderFacade.InputFilePath, nameof(_gz2DlzEncoderFacade.AbstractPredictor.PredictionError), _gz2DlzEncoderFacade.AbstractPredictor.PredictionError);
        }

        private void SaveResidualToFile(string filePath, string matrixName, int[,] matrix)
        {
            var outputFileName = GetOutputFileName(filePath, matrixName);

            using (IBitWriter bitWriter = new BitWriter(outputFileName))
            {
                WriteWithdAndHeightToFile(bitWriter);

                for (int y = 0; y < matrix.GetLength(0); y++)
                {
                    for (int x = 0; x < matrix.GetLength(1); x++)
                    {
                        uint bits = Convert.ToUInt32(matrix[y, x] + 255);
                        bitWriter.WriteNBits(bits, Constants.NumberOfBitsForPredictionError);
                    }
                }
            }
        }

        private void SaveMatchDimensionsToFile(string filePath, string matrixName, Dimension[,] matrix)
        {
            var outputFileName = GetOutputFileName(filePath, matrixName);

            using (IBitWriter bitWriter = new BitWriter(outputFileName))
            {
                WriteWithdAndHeightToFile(bitWriter);

                for (int y = 0; y < matrix.GetLength(0); y++)
                {
                    for (int x = 0; x < matrix.GetLength(1); x++)
                    {
                        var dimension = matrix[y, x] ?? new Dimension(0,0);

                        bitWriter.WriteNBits(dimension.Width, Constants.NumberOfBitsForSize);
                        bitWriter.WriteNBits(dimension.Height, Constants.NumberOfBitsForSize);
                    }
                }
            }
        }

        private void SaveMatchLocationToFile(string filePath, string matrixName, PixelLocation[,] matrix)
        {
            var outputFileName = GetOutputFileName(filePath, matrixName);

            using (IBitWriter bitWriter = new BitWriter(outputFileName))
            {
                WriteWithdAndHeightToFile(bitWriter);

                for (int y = 0; y < matrix.GetLength(0); y++)
                {
                    for (int x = 0; x < matrix.GetLength(1); x++)
                    {
                        var pixelLocation = matrix[y, x] ?? new PixelLocation(0, 0);

                        bitWriter.WriteNBits(pixelLocation.X, Constants.NumberOfBitsForX);
                        bitWriter.WriteNBits(pixelLocation.Y, Constants.NumberOfBitsForX);
                    }
                }
            }
        }

        private void SaveIsMatchFoundToFile(string filePath, string matrixName, bool[,] matrix)
        {
            var outputFileName = GetOutputFileName(filePath, matrixName);

            using (IBitWriter bitWriter = new BitWriter(outputFileName))
            {
                WriteWithdAndHeightToFile(bitWriter);

                for (int y = 0; y < matrix.GetLength(0); y++)
                {
                    for (int x = 0; x < matrix.GetLength(1); x++)
                    {
                        var bit = matrix[y, x] ? 0X01 : 0X00;

                        bitWriter.WriteBit(bit);
                    }
                }
            }
        }

        private string GetOutputFileName(string filePath, string matrixName)
        {
            string fileName = Path.GetFileName(filePath);

            return filePath + $"{Constants.Folder}\\" + fileName + "." + matrixName + Constants.IntermediaryFileExtension;
        }

        private void WriteWithdAndHeightToFile(IBitWriter bitWriter)
        {
            bitWriter.WriteNBits(_width, Constants.NumberOfBitsForSize);
            bitWriter.WriteNBits(_height, Constants.NumberOfBitsForSize);
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
                            MatchDimension[y, x] = new Dimension(bestMatch.Width, bestMatch.Height);
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
            uint rowOffset = 0;
            var widthOfTheMatchInThePreviousRow = Convert.ToUInt32(_width - 1 - encoderPoint.X);

            do
            {
                uint colOffset = 0;
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

                uint matchSize = widthOfTheMatchInThePreviousRow * rowOffset;
                var blockDimensions = new Dimension(widthOfTheMatchInThePreviousRow, rowOffset);
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
                        _gz2DlzEncoderFacade.AbstractPredictor.PredictionError[y + i, x + j] = WorkImage[y + i, x + j] - _gz2DlzEncoderFacade.AbstractPredictor.GetPredictionValue(x + j, y + i);
                    }
                }
            }
        }

        private void PredictFirstRow()
        {
            for (int i = 0; i < _width; i++)
            {
                IsPixelEncoded[0, i] = true;
                _gz2DlzEncoderFacade.AbstractPredictor.PredictionError[0, i] = WorkImage[0, i] - _gz2DlzEncoderFacade.AbstractPredictor.GetPredictionValue(i, 0);
            }
        }

        private void SetResidualAndIsPixelEncoded(int encoderX, int encoderY)
        {
            var matchDimension = MatchDimension[encoderY, encoderX];
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

        private uint GetMse(PixelLocation encoderPoint, PixelLocation matchedPoint, Dimension blockDimension)
        {
            uint sum = 0;

            for (int i = 0; i < blockDimension.Height; i++)
            {
                for (int j = 0; j < blockDimension.Width; j++)
                {
                    var nextX = encoderPoint.X + j;
                    if (nextX < _width)
                    {
                        sum += Convert.ToUInt32(Math.Pow(WorkImage[encoderPoint.Y + i, nextX] -
                                                         WorkImage[matchedPoint.Y + i, matchedPoint.X + j], 2));
                    }
                }
            }

            return sum / blockDimension.Height * blockDimension.Width;
        }

        private void InstatiateTables()
        {
            IsMatchFound = new bool[_height, _width];
            IsPixelEncoded = new bool[_height, _width];
            MatchLocation = new PixelLocation[_height, _width];
            MatchDimension = new Dimension[_height, _width];
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
