using System;
using System.IO;

namespace BitOperations
{
    public class BitReader : IDisposable
    {
        public int ReadCounter;
        private byte _buffer;
        private string _filePath;
        private readonly FileStream _reader;
        public long Length;

        public BitReader()
        {
            _filePath = "";
            ReadCounter = 0;
        }

        public BitReader(string filePath)
        {
            _filePath = filePath;
            ReadCounter = 0;
            _reader = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            Length = _reader.Length;
        }

        public byte ReadBit()
        {
            if (_reader.Length == 0)
            {
                throw new Exception("Empty file");
            }
            if (ReadCounter % 8 == 0)
            {
                _buffer = (byte) _reader.ReadByte();
            }

            byte value = (byte) ((_buffer >> (7 - (ReadCounter % 8))) & 0x01);
            ReadCounter++;

            return value;
        }

        public uint ReadNBits(int numberOfBits)
        {
            uint value = 0;

            for (int i = numberOfBits; i > 0; i--)
            {
                byte bit = ReadBit();
                value = (uint) (value | (bit << (i - 1)));
            }

            return value;
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
