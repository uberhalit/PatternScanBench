using System;

namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'BluePie' - by aghontpi
    /// https://github.com/aghontpi
    /// https://github.com/aghontpi/MemoryPatternScanner
    /// </summary>
    internal class PatternScanBluePie
    {
        /// <summary>
        /// Represents a '?' in a byte pattern, can not be matched...
        /// </summary>
        private const byte WILDCARD = 0xCC;

        /// <summary>
        /// Returns address of pattern using 'BluePie' implementation by aghontpi. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal static long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            byte[] convertedByteArray = new byte[cbPattern.Length];
            GenerateWildcardPattern(in cbPattern, ref convertedByteArray, szMask);

            long address = -1;
            for (int indexAfterBase = 0; indexAfterBase < cbMemory.Length; indexAfterBase++)
            {
                bool noMatch = false;
                if (cbMemory[indexAfterBase] != convertedByteArray[0])
                    continue;
                for (var MatchedIndex = 0; MatchedIndex < convertedByteArray.Length && indexAfterBase + MatchedIndex < cbMemory.Length; MatchedIndex++)
                {
                    if (convertedByteArray[MatchedIndex] == WILDCARD)
                        continue;
                    if (convertedByteArray[MatchedIndex] != cbMemory[indexAfterBase + MatchedIndex])
                    {
                        noMatch = true;
                        break;
                    }
                }
                if (!noMatch)
                    return indexAfterBase;
            }
            return address;
        }

        private static void GenerateWildcardPattern(in byte[] cbPattern, ref byte[] newPattern, string szMask)
        {
            int mskLen = szMask.Length;
            Buffer.BlockCopy(cbPattern, 0, newPattern, 0, cbPattern.Length);
            for (int i = 0; i < mskLen; i++)
                if (szMask[i] != 'x') newPattern[i] = WILDCARD;
        }
    }
}
