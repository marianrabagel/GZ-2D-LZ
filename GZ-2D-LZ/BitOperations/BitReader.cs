using System;
using System.IO;

namespace BitOperations
{
    public class BitReader : IDisposable
    {
        public int readCounter;
        private byte buffer;
        private string filename;
        private FileStream reader;
        public long length;

        public BitReader()
        {
            this.filename = "";
            readCounter = 0;
        }

        public BitReader(string filename)
        {
            this.filename = filename;
            readCounter = 0;
            reader = new FileStream(filename, FileMode.Open, FileAccess.Read);
            length = reader.Length;
        }

        public byte ReadBit()
        {
            if (reader.Length == 0)
                throw new Exception("Empty file");
            if (readCounter % 8 == 0)
                buffer = (byte) reader.ReadByte();

            byte value = (byte) ((buffer >> (7 - (readCounter % 8))) & 0x01);
            readCounter++;

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
