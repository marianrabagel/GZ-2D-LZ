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
            var fileName = $"{_basePath}Value0xFF_bitWriter.txt";
            DeleteFile(fileName);

            using (var bitWriter = new BitWriter(fileName))
            {
                for (var i = 0; i < 8; i++)
                {
                    bitWriter.WriteBit(0x01);
                }
            }

            using (var reader = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                Assert.AreEqual(1, reader.Length);

                var solution = (byte)reader.ReadByte();
                Assert.AreEqual(0xFF, solution);
            }
        }

        [TestMethod]
        public void WriteA()
        {
            var fileName = $"{_basePath}ValueA_bitWriter.txt";
            DeleteFile(fileName);

            using (var bitWriter = new BitWriter(fileName))
            {
                bitWriter.WriteNBits(0x41, 8);
            }

            using (var reader = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                Assert.AreEqual(1, reader.Length);

                var solution = (byte)reader.ReadByte();
                Assert.AreEqual(0x41, solution);
            }
        }

        [TestMethod]
        public void Write1ByteOf0_WriteBit()
        {
            var fileName = $"{_basePath}Value0x00_bitWriter.txt";
            DeleteFile(fileName);

            using (var bitWriter = new BitWriter(fileName))
            {
                for (var i = 0; i < 8; i++)
                {
                    bitWriter.WriteBit(0x00);
                }
            }

            using (var reader = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                Assert.AreEqual(1, reader.Length);

                var solution = (byte)reader.ReadByte();
                Assert.AreEqual(0x00, solution);
            }
        }

        [TestMethod]
        public void Write0x5A_WriteBit()
        {
            var fileName = $"{_basePath}Value0x5A_bitWriter.txt";
            DeleteFile(fileName);

            using (var bitWriter = new BitWriter(fileName))
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

            using (var reader = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                Assert.AreEqual(1, reader.Length);

                var solution = (byte)reader.ReadByte();
                Assert.AreEqual(0x5A, solution);
            }
        }
        
        [TestMethod]
        public void Write1ByteOf1_WriteNBiti()
        {
            var fileName = $"{_basePath}Value0xFF_WriteNBiti.txt";
            DeleteFile(fileName);

            using (var bitWriter = new BitWriter(fileName))
            {
                bitWriter.WriteNBits(0xFF, 8);
            }

            using (var reader = new FileStream(fileName, FileMode.OpenOrCreate))
            {
                Assert.AreEqual(1, reader.Length);

                var solution = (byte) reader.ReadByte();
                Assert.AreEqual(0xFF, solution);
            }
        }
        
        private void DeleteFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }
    }
}
