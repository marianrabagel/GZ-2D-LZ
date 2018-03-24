using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BitOperations.UnitTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class BitWriterTests
    {
        private readonly string _basePath = Environment.CurrentDirectory + "\\TestData\\";

        [TestMethod]
        public void Write1ByteOf1_WriteBit()
        {
            string fileName = $"{_basePath}Value0xFF_bitWriter.txt";

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            using (BitWriter bitWriter = new BitWriter(fileName))
            {
                for (int i = 0; i < 8; i++)
                {
                    bitWriter.WriteBit(0x01);
                }
            }

            using (FileStream reader = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                Assert.AreEqual(1, reader.Length);

                byte solution = (byte)reader.ReadByte();
                Assert.AreEqual(0xFF, solution);
            }
        }

        [TestMethod]
        public void WriteA()
        {
            string fileName = $"{_basePath}ValueA_bitWriter.txt";

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            using (BitWriter bitWriter = new BitWriter(fileName))
            {
                bitWriter.WriteNBiti(0x41, 8);
            }

            using (FileStream reader = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                Assert.AreEqual(1, reader.Length);

                byte solution = (byte)reader.ReadByte();
                Assert.AreEqual(0x41, solution);
            }
        }

        [TestMethod]
        public void Write1ByteOf0_WriteBit()
        {
            string fileName = $"{_basePath}Value0x00_bitWriter.txt";

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            using (BitWriter bitWriter = new BitWriter(fileName))
            {
                for (int i = 0; i < 8; i++)
                {
                    bitWriter.WriteBit(0x00);
                }
            }

            using (FileStream reader = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                Assert.AreEqual(1, reader.Length);

                byte solution = (byte)reader.ReadByte();
                Assert.AreEqual(0x00, solution);
            }
        }

        [TestMethod]
        public void Write0x5A_WriteBit()
        {
            string fileName = $"{_basePath}Value0x5A_bitWriter.txt";

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            using (BitWriter bitWriter = new BitWriter(fileName))
            {
                bitWriter.WriteBit(0x00);
                bitWriter.WriteBit(0x01);
                bitWriter.WriteBit(0x00);
                bitWriter.WriteBit(0x01);
                bitWriter.WriteBit(0x01);
                bitWriter.WriteBit(0x00);
                bitWriter.WriteBit(0x01);
                bitWriter.WriteBit(0x00);
            }

            using (FileStream reader = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                Assert.AreEqual(1, reader.Length);

                byte solution = (byte)reader.ReadByte();
                Assert.AreEqual(0x5A, solution);
            }
        }
        
        [TestMethod]
        public void Write1ByteOf1_WriteNBiti()
        {
            string fileName = $"{_basePath}Value0xFF_WriteNBiti.txt";
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            using (BitWriter bitWriter = new BitWriter(fileName))
            {
                bitWriter.WriteNBiti(0xFF, 8);
            }

            using (FileStream reader = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                Assert.AreEqual(1, reader.Length);

                byte solution = (byte) reader.ReadByte();
                Assert.AreEqual(0xFF, solution);
            }
        }

        [TestMethod]
        public void ReadAndWriteATxtfile()
        {
            string inputFile = $"{_basePath}test.txt";
            string outputFile = $"{_basePath}test_copy.txt";

            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            using (BitReader reader = new BitReader(inputFile))
            {
                using (BitWriter writer = new BitWriter(outputFile))
                {
                    long nrb = 8 * reader.length;

                    do
                    {
                        Random random = new Random();
                        int randomNb = random.Next(1, 33);

                        uint readNBit = reader.ReadNBit(randomNb);
                        writer.WriteNBiti(readNBit, randomNb);

                        nrb -= randomNb;

                    } while (nrb > 0);
                }
            }
        }

        [TestMethod]
        public void ReadAndWriteAJpegfile()
        {
            string inputFile = $"{_basePath}cover.jpg";
            string outputFile = $"{_basePath}cover_copy.jpg";

            if (File.Exists(outputFile))
            {
                File.Delete(outputFile);
            }

            using (BitReader reader = new BitReader(inputFile))
            {
                using (BitWriter writer = new BitWriter(outputFile))
                {
                    long nrb = 8 * reader.length;

                    do
                    {
                        Random random = new Random();
                        int randomNb = random.Next(1, 33);

                        uint readNBit = reader.ReadNBit(randomNb);
                        writer.WriteNBiti(readNBit, randomNb);

                        nrb -= randomNb;

                    } while (nrb > 0);
                }
            }
        }

        [Ignore]
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
                    long nrb = 8 * reader.length;

                    do
                    {
                        Random random = new Random();
                        int randomNb = random.Next(1, 33);

                        uint readNBit = reader.ReadNBit(randomNb);
                        writer.WriteNBiti(readNBit, randomNb);

                        nrb -= randomNb;

                    } while (nrb > 0);
                }
            }
        }
    }
}
