
namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'BoyerMooreHorspool' - by DarthTon (ported to C# by uberhalit)
    /// https://github.com/DarthTon
    /// https://github.com/learn-more/findpattern-bench/blob/master/patterns/DarthTon.h
    /// 
    /// Boyer-Moore-Horspool with wildcards implementation
    /// </summary>
    internal class PatternScanBoyerMooreHorspool : PatternScanAlgorithm
    {
        internal override string Creator => "DarthTon";
        internal PatternScanBoyerMooreHorspool() { }

        private const byte wildcard = 0xCC;

        /// <summary>
        /// Initializes a new 'PatternScanBoyerMooreHorspool'.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <returns>An optional string to display next to benchmark results.</returns>
        internal override string Init(in byte[] cbMemory)
        {
            return "";
        }

        /// <summary>
        /// Returns address of pattern using 'BoyerMooreHorspool' implementation by DarthTon. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal override long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            int scanEnd = cbMemory.Length - cbPattern.Length;
            int last = cbPattern.Length - 1;

            byte[] newPattern = GenerateWildcardPattern(cbPattern, szMask);
            uint[] badCharSkip = FillShiftTable(newPattern);

            // Search
            uint pScanPos = 0;
            for (; pScanPos <= scanEnd; pScanPos += badCharSkip[cbMemory[pScanPos + last]])
            {
                for (int idx = last; idx >= 0; --idx)
                {
                    if (newPattern[idx] != wildcard && cbMemory[idx + pScanPos] != newPattern[idx])
                        break;
                    if (idx == 0)
                        return pScanPos;
                }         
            }
            return -1;
        }

        private byte[] GenerateWildcardPattern(byte[] cbPattern, string szMask)
        {
            byte[] newPattern = new byte[cbPattern.Length];
            for (int i = 0; i < szMask.Length; i++)
            {
                if (szMask[i] != 'x') newPattern[i] = wildcard;
                else newPattern[i] = cbPattern[i];
            }

            return newPattern;
        }

        private static uint[] FillShiftTable(byte[] pPattern)
        {
            uint idx = 0;
            uint last = (uint)pPattern.Length - 1;

            // Get last wildcard position
            for (idx = last; idx > 0 && pPattern[idx] != wildcard; --idx) { }
            uint diff = last - idx;
            if (diff == 0)
                diff = 1;

            // Prepare shift table
            uint[] badCharSkip = new uint[256];
            for (idx = 0; idx <= 255; ++idx)
                badCharSkip[idx] = diff;
            for (idx = last - diff; idx < last; ++idx)
                badCharSkip[pPattern[idx]] = last - idx;
            return badCharSkip;
        }
    }
}
