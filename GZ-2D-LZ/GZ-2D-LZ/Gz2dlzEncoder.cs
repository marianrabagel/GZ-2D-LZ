using System;
using System.IO;
using BitOperations;
using BitOperations.Contracts;
using G2_2D_LZ.Contracts.Facades;
using G2_2D_LZ.Helpers;
using G2_2D_LZ.Writers;

namespace G2_2D_LZ
{
    public class Gz2DlzEncoder
    {
        private readonly byte[,] _originalImage;
        private readonly int _height;
        private readonly int _width;
        public string ArchivePath;
        
        public bool[,] IsMatchFound { get; private set; } //has true when a suitable match for a block is found
        public bool[,] IsPixelEncoded { get; private set; } //has true when a pixel has been encoded
        public PixelLocation[,] MatchLocation { get; private set; } //position of the match relative to the block being encoded
        public Dimension[,] MatchDimension { get; private set; } //width and heigth of the block being encoded
        public int[,] Residual { get; private set; } //difference between the pixel in the actual block and the matching block
        public byte[,] WorkImage { get; set; } //fractal debugging

        private readonly IGz2DlzEncoderFacade _gz2DlzEncoderFacade;

        public Gz2DlzEncoder(IGz2DlzEncoderFacade gz2DlzEncoderFacade)
        {
            _gz2DlzEncoderFacade = gz2DlzEncoderFacade;
            _originalImage = _gz2DlzEncoderFacade.ImageReader.GetImageFromFile(_gz2DlzEncoderFacade.InputFilePath);

            _height = _originalImage.GetLength(0);
            _width = _originalImage.GetLength(1);

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
            ArchivePath = _gz2DlzEncoderFacade.Archiver.Compress(_gz2DlzEncoderFacade.InputFilePath + Constants.Folder, null, 9);
        }

        public void WriteMatrixToFileAsText()
        {
            TxtWriter txtWriter = new TxtWriter(_gz2DlzEncoderFacade.InputFilePath + Constants.IntermediaryFileExtension);

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

                        bitWriter.WriteNBits(Convert.ToUInt32(dimension.Width), Constants.NumberOfBitsForSize);
                        bitWriter.WriteNBits(Convert.ToUInt32(dimension.Height), Constants.NumberOfBitsForSize);
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

                        bitWriter.WriteNBits(Convert.ToUInt32(pixelLocation.X), Constants.NumberOfBitsForX);
                        bitWriter.WriteNBits(Convert.ToUInt32(pixelLocation.Y), Constants.NumberOfBitsForX);
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
            bitWriter.WriteNBits(Convert.ToUInt32(_width), Constants.NumberOfBitsForSize);
            bitWriter.WriteNBits(Convert.ToUInt32(_height), Constants.NumberOfBitsForSize);
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
                        BestMatch bestMatch;

                        if (_gz2DlzEncoderFacade.GeometricTransformation == (int) Constants.GeometricTransformation.All)
                        {
                            _gz2DlzEncoderFacade.GeometricTransformation = (int) Constants.GeometricTransformation.Identity;
                            var bestMatchIdentity = LocateTheBestAproximateMatchForGivenRootPixel(encoderPoint, rootPoint);
                            _gz2DlzEncoderFacade.GeometricTransformation = (int) Constants.GeometricTransformation.VerticalMirror;

                            var bestMatchVerticalMirror = LocateTheBestAproximateMatchForGivenRootPixel(encoderPoint, rootPoint);

                            if (bestMatchIdentity.Size >= bestMatchVerticalMirror.Size)
                            {
                                bestMatch = bestMatchIdentity;
                                _gz2DlzEncoderFacade.GeometricTransformation = (int) Constants.GeometricTransformation.Identity;
                            }
                            else
                            {
                                bestMatch = bestMatchVerticalMirror;
                                _gz2DlzEncoderFacade.GeometricTransformation = (int) Constants.GeometricTransformation.VerticalMirror;
                            }
                        }
                        else
                        {
                            bestMatch = LocateTheBestAproximateMatchForGivenRootPixel(encoderPoint, rootPoint);
                        }

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
            int rowOffset = 0;
            var widthOfTheMatchInThePreviousRow = _width - 1 - encoderPoint.X;

            do
            {
                int colOffset = CalculateTheWidthOfTheMatch(encoderPoint, rowOffset, rootPoint, widthOfTheMatchInThePreviousRow);
                widthOfTheMatchInThePreviousRow = colOffset;
                rowOffset++;

                var matchSize = widthOfTheMatchInThePreviousRow * rowOffset;
                var blockDimensions = new Dimension(widthOfTheMatchInThePreviousRow, rowOffset);
                var matchMse = GetMse(encoderPoint, rootPoint, blockDimensions);

                if (matchSize >= bestMatch.Size && matchMse <= Constants.MaxMse)
                {
                    bestMatch.Height = rowOffset;
                    bestMatch.Width = widthOfTheMatchInThePreviousRow;
                    bestMatch.Size = matchSize;
                }
            } while (widthOfTheMatchInThePreviousRow != 0 && encoderPoint.Y + rowOffset != _height - 1);

            return bestMatch;
        }

        private int CalculateTheWidthOfTheMatch(PixelLocation encoderPoint, int rowOffset,
            PixelLocation rootPoint, int widthOfTheMatchInThePreviousRow)
        {
            int colOffset = 0;
            var nextRootPoint = new PixelLocation(rootPoint.X + colOffset, rootPoint.Y + rowOffset);

            if (IsPixelEncoded[nextRootPoint.Y, nextRootPoint.X]
                || nextRootPoint.Y >= encoderPoint.Y && nextRootPoint.X >= encoderPoint.X)
            {
                do
                {
                    var nextToBeEncoded = new PixelLocation(encoderPoint.X + colOffset, encoderPoint.Y + rowOffset);

                    if (IsEdge(nextToBeEncoded.X, nextToBeEncoded.Y))
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
                        var x = GetNextRootX(nextRootPoint.X, colOffset);

                        if (x < 0)
                        {
                            break;
                        }

                        var possibleMatchPixel = WorkImage[nextRootPoint.Y, x];

                        if (Math.Abs(pixelToBeEncoded - possibleMatchPixel) <= Constants.Threshold)
                        {
                            colOffset++;
                        }
                        else
                        {
                            break;
                        }
                    }

                } while (colOffset != widthOfTheMatchInThePreviousRow);
            }

            return colOffset;
        }

        private int GetNextRootX(int x, int colOffset)
        {
            if (_gz2DlzEncoderFacade.GeometricTransformation == (int)Constants.GeometricTransformation.Identity)
            {
                return x + colOffset;
            }
            if (_gz2DlzEncoderFacade.GeometricTransformation == (int)Constants.GeometricTransformation.VerticalMirror)
            {
                return x - colOffset;
            }

            throw new Exception("not set");
        }

        private bool IsEdge(int x, int y)
        {
            return x == _width || y == _height ;
        }

        private int GetXAfterGeometricTransformation(int x)
        {
            if (_gz2DlzEncoderFacade.GeometricTransformation == (int) Constants.GeometricTransformation.Identity)
            {
                return x;
            }
            if (_gz2DlzEncoderFacade.GeometricTransformation == (int)Constants.GeometricTransformation.VerticalMirror)
            {
                return Constants.SearchWidth - 1 - x;
            }

            throw new Exception("not set");
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
            if (_gz2DlzEncoderFacade.GeometricTransformation == (int) Constants.GeometricTransformation.Identity)
            {
                var rootX = x - Constants.SearchWidth;

                if (rootX < 0)
                {
                    rootX = 0;
                }

                return rootX;
            }

            if (_gz2DlzEncoderFacade.GeometricTransformation == (int) Constants.GeometricTransformation.VerticalMirror)
            {
                return x;
            }

            throw new Exception("No geometric transformation set");
        }

        private int GetMse(PixelLocation encoderPoint, PixelLocation matchedPoint, Dimension blockDimension)
        {
            int sum = 0;

            for (int yy = 0; yy < blockDimension.Height; yy++)
            {
                for (int xx = 0; xx < blockDimension.Width; xx++)
                {
                    //var nextX = encoderPoint.X + xx;
                    var nextX = GetNextRootX(encoderPoint.X, xx);

                    if (nextX < _width && nextX >= 0)
                    {
                        sum += Convert.ToInt32(Math.Pow(WorkImage[encoderPoint.Y + yy, nextX] -
                                                         WorkImage[matchedPoint.Y + yy, matchedPoint.X + xx], 2));
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
