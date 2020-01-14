using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reloaded.Memory.Sigscan;

namespace PatternScanBench.Implementations
{
    internal class PatternScanReloadedSigscan : PatternScanAlgorithm
    {
        internal override string Creator => "Sewer56";
        internal override string Init(in byte[] cbMemory) { return ""; }

        internal override long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            var scanner = new Scanner(cbMemory);
            var stringBuilder = new StringBuilder(cbPattern.Length * 3);

            for (int x = 0; x < cbPattern.Length; x++)
            {
                stringBuilder.Append(szMask[x] == 'x' ? $"{cbPattern[x]:X} " 
                                                      : $"?? ");
            }

            stringBuilder.Length -= 1;
            return scanner.CompiledFindPattern(stringBuilder.ToString()).Offset;
        }
    }
}
