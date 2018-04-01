using System;

namespace BitOperations.Contracts
{
    public interface IBitWriter : IDisposable
    {
        void WriteBit(int bit);
        void WriteNBits(uint bits, int n);
    }
}
