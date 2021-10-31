using System;

namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'BoyerMooreHorspool' - by DarthTon (ported to C# by uberhalit)
    /// https://github.com/DarthTon
    /// https://github.com/learn-more/findpattern-bench/blob/master/patterns/DarthTon.h
    /// 
    /// Boyer-Moore-Horspool with wildcards implementation
    /// </summary>
    internal class PatternScanBoyerMooreHorspool
    {
        /// <summary>
        /// Represents a '?' in a byte pattern, can not be matched...
        /// </summary>
        private const byte WILDCARD = 0xCC;

        /// <summary>
        /// Returns address of pattern using 'BoyerMooreHorspool' implementation by DarthTon. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal static long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            int scanEnd = cbMemory.Length - cbPattern.Length;
            int last = cbPattern.Length - 1;

            byte[] newPattern = new byte[cbPattern.Length];
            GenerateWildcardPattern(in cbPattern, ref newPattern, szMask);
            uint[] badCharSkip = FillShiftTable(ref newPattern);

            // Search
            uint pScanPos = 0;
            for (; pScanPos <= scanEnd; pScanPos += badCharSkip[cbMemory[pScanPos + last]])
            {
                for (int idx = last; idx >= 0; --idx)
                {
                    if (newPattern[idx] != WILDCARD && cbMemory[idx + pScanPos] != newPattern[idx])
                        break;
                    if (idx == 0)
                        return pScanPos;
                }         
            }
            return -1;
        }

        private static void GenerateWildcardPattern(in byte[] cbPattern, ref byte[] newPattern, string szMask)
        {
            int mskLen = szMask.Length;
            Buffer.BlockCopy(cbPattern, 0, newPattern, 0, cbPattern.Length);
            for (int i = 0; i < mskLen; i++)
                if (szMask[i] != 'x') newPattern[i] = WILDCARD;
        }

        private static uint[] FillShiftTable(ref byte[] pPattern)
        {
            uint idx = 0;
            uint last = (uint)pPattern.Length - 1;

            // Get last wildcard position
            for (idx = last; idx > 0 && pPattern[idx] != WILDCARD; --idx) { }
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
