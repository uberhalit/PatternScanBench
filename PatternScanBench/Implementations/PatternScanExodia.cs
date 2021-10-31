using System;

namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'Exodia' - by mrexodia
    /// https://github.com/mrexodia
    /// https://github.com/mrexodia/PatternFinder/blob/master/PatternFinder/Pattern.cs
    /// </summary>
    internal class PatternScanExodia
    {
        /// <summary>
        /// Represents a '?' in a byte pattern, can not be matched...
        /// </summary>
        private const byte WILDCARD = 0xCC;

        /// <summary>
        /// Returns address of pattern using 'Exodia' implementation by mrexodia. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal static long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            long dataLongLength = cbMemory.Length;

            byte[] newPattern = new byte[cbPattern.Length];
            GenerateWildcardPattern(in cbPattern, ref newPattern, szMask);
            var patternSize = newPattern.Length;

            for (long i = 0, pos = 0; i < dataLongLength; i++)
            {
                if (matchByte(cbMemory[i], ref newPattern[pos]))
                {
                    pos++;
                    if (pos == patternSize)
                    {
                        return i - patternSize + 1;
                    }
                }
                else
                {
                    i -= pos;
                    pos = 0;
                }
            }

            return -1;
        }

        private static bool matchByte(byte b, ref byte p)
        {
            if (p != WILDCARD)
            {
                if (b != p)
                    return false;
            }
            return true;
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
