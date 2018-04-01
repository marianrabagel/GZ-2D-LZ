using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitOperations.Contracts
{
    interface IBitWriter : IDisposable
    {
        void WriteBit(int bit);
        void WriteNBits(uint bits, int n);
    }
}
