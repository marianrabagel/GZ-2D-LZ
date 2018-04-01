using System;
using System.IO;
using BitOperations.Contracts;

namespace BitOperations
{
    public class BitWriter : IBitWriter
    {
        private string _filePath;
        private byte _buffer;
        private int _writeCounter;
        private readonly FileStream _writer;

        public BitWriter(string filePath)
        {
            _writeCounter = 0;
            _filePath = filePath;
            _writer = new FileStream(filePath, FileMode.OpenOrCreate);
        }

        public void WriteBit(int bit)
        {
            _buffer = (byte) (_buffer | (bit << (7 - _writeCounter % 8)));
            _writeCounter++;

            if (_writeCounter % 8 == 0)
            {
                _writer.WriteByte(_buffer);
                _buffer = 0;
            }
        }

        public void WriteNBits(uint bits, int n)
        {
            if (n > 32 || n == 0)
            {
                return;
            }

            for (int i = n; i > 0; i--)
            {
                WriteBit((int) (bits >> i - 1));
            }
        }

        public void Dispose()
        {
            _writer.Dispose();
        }
    }
}
