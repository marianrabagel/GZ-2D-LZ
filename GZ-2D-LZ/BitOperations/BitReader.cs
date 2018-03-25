using System;
using System.IO;

namespace BitOperations
{
    public class BitReader : IDisposable
    {
        public int ReadCounter;
        private byte _buffer;
        private string filename;
        private readonly FileStream reader;
        public long Length;

        public BitReader()
        {
            filename = "";
            ReadCounter = 0;
        }

        public BitReader(string filename)
        {
            this.filename = filename;
            ReadCounter = 0;
            reader = new FileStream(filename, FileMode.Open, FileAccess.Read);
            Length = reader.Length;
        }

        public byte ReadBit()
        {
            if (reader.Length == 0)
                throw new Exception("Empty file");
            if (ReadCounter % 8 == 0)
                _buffer = (byte) reader.ReadByte();

            byte value = (byte) ((_buffer >> (7 - (ReadCounter % 8))) & 0x01);
            ReadCounter++;

            return value;
        }

        public uint ReadNBit(int numberOfBits)
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
            reader.Dispose();
        }
    }
}
