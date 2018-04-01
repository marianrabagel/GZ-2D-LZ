using System;

namespace BitOperations.Contracts
{
    interface IBitReader : IDisposable
    {
        byte ReadBit();
        uint ReadNBits(int numberOfBits);
    }
}
