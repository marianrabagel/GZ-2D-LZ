using System;
using System.Diagnostics;
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
        public int[,] GeometricTransformation { get; private set; } 
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
        
        public void Encode(int? specificGeometricTransform)
        {
            _gz2DlzEncoderFacade.AbstractPredictor.SetOriginalMatrix(WorkImage);
            _gz2DlzEncoderFacade.AbstractPredictor.InitializePredictionError((int) _height, (int)_width);
            PredictFirstRow();
            EncodeWorkImage(specificGeometricTransform);
            Directory.CreateDirectory(_gz2DlzEncoderFacade.InputFilePath + Constants.Folder);
            WriteResultingMatricesToIndividualFiles(specificGeometricTransform);
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
            txtWriter.WriteMatrixToFile(GeometricTransformation);
        }

        public void WriteResultingMatricesToIndividualFiles(int? specificGeometricTransform)
        {
            //todo move this into its own class
            SaveIsMatchFoundToFile(_gz2DlzEncoderFacade.InputFilePath, nameof(IsMatchFound), IsMatchFound);
            SaveMatchLocationToFile(_gz2DlzEncoderFacade.InputFilePath, nameof(MatchLocation), MatchLocation);
            SaveMatchDimensionsToFile(_gz2DlzEncoderFacade.InputFilePath, nameof(MatchDimension), MatchDimension);
            ConvertToPositiveAndSaveMatrixToFile(_gz2DlzEncoderFacade.InputFilePath, nameof(Residual), Residual);
            ConvertToPositiveAndSaveMatrixToFile(_gz2DlzEncoderFacade.InputFilePath, nameof(_gz2DlzEncoderFacade.AbstractPredictor.PredictionError), _gz2DlzEncoderFacade.AbstractPredictor.PredictionError);

            if (specificGeometricTransform != (int) Helpers.Constants.GeometricTransformation.NoGeometricTransformation)
            {
                SaveIntMatrixToFile(_gz2DlzEncoderFacade.InputFilePath, nameof(GeometricTransformation),
                    GeometricTransformation);
            }
        }

        private void SaveIntMatrixToFile(string filePath, string matrixName, int[,] matrix)
        {
            var outputFileName = GetOutputFileName(filePath, matrixName);

            using (IBitWriter bitWriter = new BitWriter(outputFileName))
            {
                WriteWithdAndHeightToFile(bitWriter);

                for (int y = 0; y < matrix.GetLength(0); y++)
                {
                    for (int x = 0; x < matrix.GetLength(1); x++)
                    {
                        uint bits = Convert.ToUInt32(matrix[y, x]);
                        bitWriter.WriteNBits(bits, Constants.NumberOfBitsForPredictionError);
                    }
                }
            }
        }

        private void ConvertToPositiveAndSaveMatrixToFile(string filePath, string matrixName, int[,] matrix)
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
        
        private void EncodeWorkImage(int? specificGeometricTransform)
        {
            int countIdentiy = 0;
            int countVertical = 0;
            int countHorizontal = 0;
            int count180 = 0;
            int countPrediction = 0;

            for (int y = 1; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (!IsPixelEncoded[y, x])
                    {
                        var encoderPoint = new PixelLocation(x, y);
                        BestMatchDetails bestMatchDetails;
                        var isForAllGeometricTransformation = specificGeometricTransform == null;

                        if (isForAllGeometricTransformation)
                        {
                            bestMatchDetails = GetBestMatchForAllGeometricTransformations(encoderPoint);
                        }
                        else
                        {
                            bestMatchDetails = GetBestMatch(encoderPoint, (Constants.GeometricTransformation) specificGeometricTransform);
                        }
                        if (bestMatchDetails.BestMatch.Size > Constants.MinMatchSize)
                        {
                            IsMatchFound[y, x] = true;
                            MatchDimension[y, x] = new Dimension(bestMatchDetails.BestMatch.Width, bestMatchDetails.BestMatch.Height);
                            MatchLocation[y, x] = bestMatchDetails.RootPoint;
                            GeometricTransformation[y, x] = bestMatchDetails.GeometricTransformation;
                            SetResidualAndIsPixelEncoded(x, y, bestMatchDetails.GeometricTransformation);
                            
                            if (bestMatchDetails.GeometricTransformation == (int) Constants.GeometricTransformation.Identity
                                || bestMatchDetails.GeometricTransformation == (int)Constants.GeometricTransformation.NoGeometricTransformation)
                            {
                                countIdentiy++;
                            }
                            if (bestMatchDetails.GeometricTransformation == (int)Constants.GeometricTransformation.VerticalMirror)
                            {
                                countVertical++;
                            }
                            if (bestMatchDetails.GeometricTransformation == (int)Constants.GeometricTransformation.HorizontalMirror)
                            {
                                countHorizontal++;
                            }
                            if (bestMatchDetails.GeometricTransformation == (int)Constants.GeometricTransformation.FirstDiagonalMirror)
                            {
                                count180++;
                            }
                        }
                        else
                        {
                            IsMatchFound[y, x] = false;
                            PredictNoMatchBlock(x, y);
                            x += Constants.NoMatchBlockWidth - 1;
                            countPrediction++;
                        }
                    }
                }
            }

            using (StreamWriter wr = new StreamWriter(_gz2DlzEncoderFacade.InputFilePath + "_counts.txt"))
            {
                wr.WriteLine(_gz2DlzEncoderFacade.InputFilePath);
                wr.WriteLine(nameof(countIdentiy) + ": " + countIdentiy);
                wr.WriteLine(nameof(countVertical)+ ": " + countVertical);
                wr.WriteLine(nameof(countHorizontal) + ": " + countHorizontal);
                wr.WriteLine(nameof(count180) + ": " + count180);
                wr.WriteLine(nameof(countPrediction) + ": " + countPrediction);
            }
        }

        private BestMatchDetails GetBestMatchForAllGeometricTransformations(PixelLocation encoderPoint)
        {
            BestMatchDetails bestMatchDetails;
            var bestMatchIdentity = GetBestMatch(encoderPoint, Constants.GeometricTransformation.Identity);
            var bestMatchVerticalMirror = GetBestMatch(encoderPoint, Constants.GeometricTransformation.VerticalMirror);
            var bestMatchHorizontalMirror = GetBestMatch(encoderPoint, Constants.GeometricTransformation.HorizontalMirror);
            var bestMatch180Flip = GetBestMatch(encoderPoint, Constants.GeometricTransformation.FirstDiagonalMirror);

            if (bestMatchIdentity.BestMatch.Size >= bestMatchVerticalMirror.BestMatch.Size)
            {
                bestMatchDetails = bestMatchIdentity;
            }
            else
            {
                bestMatchDetails = bestMatchVerticalMirror;
            }
            if (bestMatchHorizontalMirror.BestMatch.Size > bestMatchDetails.BestMatch.Size)
            {
                bestMatchDetails = bestMatchHorizontalMirror;
            }
            if (bestMatch180Flip.BestMatch.Size > bestMatchDetails.BestMatch.Size)
            {
                bestMatchDetails = bestMatch180Flip;
            }

            return bestMatchDetails;
        }

        private BestMatchDetails GetBestMatch(PixelLocation encoderPoint, Constants.GeometricTransformation geometricTransformation)
        {
            int tranformationValue = (int) geometricTransformation;
            var rootPoint = new PixelLocation
            {
                X = GetRootX(encoderPoint.X, tranformationValue),
                Y = GetRootY(encoderPoint.Y, tranformationValue)
            };

            return new BestMatchDetails
            {
                GeometricTransformation = tranformationValue,
                RootPoint = rootPoint,
                BestMatch = LocateTheBestAproximateMatchForGivenRootPixel(encoderPoint, rootPoint, geometricTransformation)
            };
        }

        public BestMatch LocateTheBestAproximateMatchForGivenRootPixel(PixelLocation encoderPoint,
            PixelLocation rootPoint, Constants.GeometricTransformation geometricTransformation)
        {
            BestMatch bestMatch = new BestMatch();
            int rowOffset = 0;
            var widthOfTheMatchInThePreviousRow = _width - encoderPoint.X;

            do
            {
                int colOffset = CalculateTheWidthOfTheMatch(encoderPoint, rowOffset, rootPoint,
                    widthOfTheMatchInThePreviousRow, geometricTransformation);
                widthOfTheMatchInThePreviousRow = colOffset;
                rowOffset++;

                var matchSize = widthOfTheMatchInThePreviousRow * rowOffset;
                var blockDimensions = new Dimension(widthOfTheMatchInThePreviousRow, rowOffset);
                var matchMse = GetMse(encoderPoint, rootPoint, blockDimensions, geometricTransformation);
                
                if (matchSize >= bestMatch.Size && matchMse <= Constants.MaxMse)
                {
                    bestMatch.Height = rowOffset;
                    bestMatch.Width = widthOfTheMatchInThePreviousRow;
                    bestMatch.Size = matchSize;
                }
            } while (widthOfTheMatchInThePreviousRow != 0 && encoderPoint.Y + rowOffset != _height);

            return bestMatch;
        }

        private int CalculateTheWidthOfTheMatch(PixelLocation encoderPoint, int rowOffset,
            PixelLocation rootPoint, int widthOfTheMatchInThePreviousRow,
            Constants.GeometricTransformation geometricTransformation)
        {
            int colOffset = 0;
            var nextRootY = NextRoot.GetNextRootY(rootPoint.Y, rowOffset, geometricTransformation);

            if (nextRootY < 0)
            {
                return colOffset;
            }

            var nextRootPoint = new PixelLocation(rootPoint.X, nextRootY);
            var isAvailableAtTheDecoder = nextRootPoint.Y >= encoderPoint.Y && nextRootPoint.X >= encoderPoint.X;

            if (IsPixelEncoded[nextRootPoint.Y, nextRootPoint.X] || isAvailableAtTheDecoder)
            {
                do
                {
                    var nextToBeEncoded = new PixelLocation(encoderPoint.X + colOffset, encoderPoint.Y + rowOffset);

                    if (IsOutOfBounds(nextToBeEncoded.X, nextToBeEncoded.Y) 
                        || IsPixelEncoded[nextToBeEncoded.Y, nextToBeEncoded.X])
                    {
                        break;
                    }

                    var pixelToBeEncoded = WorkImage[nextToBeEncoded.Y, nextToBeEncoded.X];
                    var nextRootX = NextRoot.GetNextRootX(nextRootPoint.X, colOffset, geometricTransformation);

                    if (nextRootX < 0)
                    {
                        break;
                    }

                    var possibleMatchPixel = WorkImage[nextRootPoint.Y, nextRootX];

                    int value = pixelToBeEncoded - possibleMatchPixel;
                    if (Math.Abs(value) <= Constants.Threshold)
                    {
                        colOffset++;
                    }
                    else
                    {
                        break;
                    }

                } while (colOffset != widthOfTheMatchInThePreviousRow);
            }

            return colOffset;
        }

        private bool IsOutOfBounds(int x, int y)
        {
            return x == _width || y == _height ;
        }

        private void PredictNoMatchBlock(int x, int y)
        {
            for (int yy = 0; yy < Constants.NoMatchBlockHeight; yy++)
            {
                for (int xx = 0; xx < Constants.NoMatchBlockWidth; xx++)
                {
                    if (y + yy < WorkImage.GetLength(0) && x + xx < WorkImage.GetLength(1))
                    {
                        IsPixelEncoded[y + yy, x + xx] = true;
                        _gz2DlzEncoderFacade.AbstractPredictor.PredictionError[y + yy, x + xx] = WorkImage[y + yy, x + xx] - _gz2DlzEncoderFacade.AbstractPredictor.GetPredictionValue(x + xx, y + yy);
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

        private void SetResidualAndIsPixelEncoded(int encoderX, int encoderY, int geometricTransformation)
        {
            var matchDimension = MatchDimension[encoderY, encoderX];
            var matchLocation = MatchLocation[encoderY, encoderX];

            for (int yy = 0; yy < matchDimension.Height; yy++)
            {
                for (int xx = 0; xx < matchDimension.Width; xx++)
                {
                    var nextX = NextRoot.GetNextRootX(matchLocation.X, xx, (Constants.GeometricTransformation) geometricTransformation);
                    var nextY = NextRoot.GetNextRootY(matchLocation.Y, yy, (Constants.GeometricTransformation) geometricTransformation);
                    
                    Residual[encoderY + yy, encoderX + xx] = WorkImage[encoderY + yy, encoderX + xx] -
                                                     WorkImage[nextY, nextX];
                    IsPixelEncoded[encoderY + yy, encoderX + xx] = true;
                }
            }
        }

        private static int GetRootY(int y, int geometricTransformation)
        {
            if (geometricTransformation == (int) Constants.GeometricTransformation.HorizontalMirror)
            {
                return y - 1;
            }

            if (geometricTransformation == (int) Constants.GeometricTransformation.FirstDiagonalMirror)
            {
                return y - 1;
            }

            if (geometricTransformation == (int) Constants.GeometricTransformation.VerticalMirror
                || geometricTransformation == (int) Constants.GeometricTransformation.Identity
                || geometricTransformation == (int) Constants.GeometricTransformation.NoGeometricTransformation)
            {

                var rootY = y - Constants.SearchHeight;

                if (rootY < 0)
                {
                    rootY = 0;
                }

                return rootY;
            }

            throw new InvalidOperationException("geometric tranformation not set" + nameof(GetRootY));
        }

        private static int GetRootX(int x, int geometricTransformation)
        {
            if (geometricTransformation == (int) Constants.GeometricTransformation.VerticalMirror
                || geometricTransformation == (int)Constants.GeometricTransformation.FirstDiagonalMirror)
            {
                return x;
            }

            if (geometricTransformation == (int) Constants.GeometricTransformation.HorizontalMirror
                || geometricTransformation == (int) Constants.GeometricTransformation.Identity
                || geometricTransformation == (int) Constants.GeometricTransformation.NoGeometricTransformation)
            {
                var rootX = x - Constants.SearchWidth;

                if (rootX < 0)
                {
                    rootX = 0;
                }

                return rootX;
            }

            throw new InvalidOperationException("geometric tranformation not set" + nameof(GetRootX));
        }

        private double GetMse(PixelLocation encoderPoint, PixelLocation matchedPoint, Dimension blockDimension,
            Constants.GeometricTransformation geometricTransformation)
        {
            int sum = 0;

            for (int yy = 0; yy < blockDimension.Height; yy++)
            {
                for (int xx = 0; xx < blockDimension.Width; xx++)
                {
                    var matchedLocation = new PixelLocation
                    {
                        X = NextRoot.GetNextRootX(matchedPoint.X, xx, geometricTransformation),
                        Y = NextRoot.GetNextRootY(matchedPoint.Y, yy, geometricTransformation)
                    };
                    sum += Convert.ToInt32(Math.Pow(WorkImage[encoderPoint.Y + yy, encoderPoint.X + xx] -
                                                    WorkImage[matchedLocation.Y, matchedLocation.X], 2));
                }
            }

            var size = blockDimension.Height * blockDimension.Width;

            return sum / (double) size;
        }

        private void InstatiateTables()
        {
            IsMatchFound = new bool[_height, _width];
            IsPixelEncoded = new bool[_height, _width];
            MatchLocation = new PixelLocation[_height, _width];
            MatchDimension = new Dimension[_height, _width];
            Residual = new int[_height, _width];
            WorkImage = new byte[_height, _width];
            GeometricTransformation = new int[_height, _width];
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
