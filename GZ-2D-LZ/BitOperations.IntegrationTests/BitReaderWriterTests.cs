using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitOperations.IntegrationTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class BitReaderWriterTests
    {
        private readonly string _basePath = Environment.CurrentDirectory + "\\TestData\\";

        [TestMethod]
        public void ReadAndWriteATxtfile()
        {
            string inputFilePath = $"{_basePath}test.txt";
            string outputFilePath = $"{_basePath}test_copy.txt";

            if (File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }

            using (BitReader reader = new BitReader(inputFilePath))
            {
                using (BitWriter writer = new BitWriter(outputFilePath))
                {
                    long nrb = 8 * reader.Length;

                    do
                    {
                        Random random = new Random();
                        int randomNb = random.Next(1, 33);

                        uint readNBit = reader.ReadNBits(randomNb);
                        writer.WriteNBits(readNBit, randomNb);

                        nrb -= randomNb;

                    } while (nrb > 0);
                }
            }
        }

        [TestMethod]
        public void ReadAndWriteAJpegfile()
        {
            string inputFilePath = $"{_basePath}cover.jpg";
            string outputFilePath = $"{_basePath}cover_copy.jpg";

            if (File.Exists(outputFilePath))
            {
                File.Delete(outputFilePath);
            }

            using (BitReader reader = new BitReader(inputFilePath))
            {
                using (BitWriter writer = new BitWriter(outputFilePath))
                {
                    long nrb = 8 * reader.Length;

                    do
                    {
                        Random random = new Random();
                        int randomNb = random.Next(1, 33);

                        uint readNBit = reader.ReadNBits(randomNb);
                        writer.WriteNBits(readNBit, randomNb);

                        nrb -= randomNb;

                    } while (nrb > 0);
                }
            }
        }

        [TestMethod]
        public void ReadAndWriteAPdffile()
        {
            string inputFile = $"{_basePath}brin-page-98.pdf";
            string outputFile = $"{_basePath}brin-page-98_copy.pdf";

            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            using (BitReader reader = new BitReader(inputFile))
            {
                using (BitWriter writer = new BitWriter(outputFile))
                {
                    long nrb = 8 * reader.Length;

                    do
                    {
                        Random random = new Random();
                        int randomNb = random.Next(1, 33);

                        uint readNBit = reader.ReadNBits(randomNb);
                        writer.WriteNBits(readNBit, randomNb);

                        nrb -= randomNb;

                    } while (nrb > 0);
                }
            }
        }
    }
}
