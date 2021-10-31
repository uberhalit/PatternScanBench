using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'SpanEquals' - by uberhalit
    /// https://github.com/uberhalit
    /// </summary>
    internal class PatternScanSpanEquals
    {
        /// <summary>
        /// Returns address of pattern using 'SpanEquals' implementation by uberhalit. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal static long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            ReadOnlySpan<byte> cxMemory = cbMemory.AsSpan();
            ReadOnlySpan<byte> cxPattern = cbPattern.AsSpan();

            ref byte pCxMemory = ref MemoryMarshal.GetReference(cxMemory);
            ref byte pCxPattern = ref MemoryMarshal.GetReference(cxPattern);

            ReadOnlySpan<ushort> matchTable = BuildMatchRanges(szMask, szMask.Length);
            int matchTableLength = matchTable.Length;

            int searchLength = cbMemory.Length - cbPattern.Length;
            for (int position = 0; position < searchLength; position++, pCxMemory = ref Unsafe.Add(ref pCxMemory, 1))
            {
                if (pCxPattern != pCxMemory)
                {
                    if (pCxPattern != Unsafe.Add(ref pCxMemory, 1))
                    {
                        position++;
                        pCxMemory = ref Unsafe.Add(ref pCxMemory, 1);
                    }
                    continue;
                }

                bool found = true;
                for (int i = 0; i < matchTableLength; i += 2)
                {
                    int matchLen = matchTable[i + 1] - matchTable[i];
                    if (matchLen < 2)
                    {
                        if (Unsafe.Add(ref pCxPattern, 1 + matchTable[i]) != Unsafe.Add(ref pCxMemory, 1 + matchTable[i]))
                        {
                            found = false;
                            break;
                        }
                    }
                    else if (!cxPattern.Slice(1 + matchTable[i], matchLen).SequenceEqual(cxMemory.Slice(1 + position + matchTable[i], matchLen)))
                    //else if (!IsEqualAsRef(ref Unsafe.Add(ref pCxPattern, 1 + matchTable[i]), ref Unsafe.Add(ref pCxMemory, 1 + matchTable[i]), matchLen))
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                    return position;
            }

            return -1;
        }

        //internal static bool IsEqualAsRef(ref byte left, ref byte right, int length)
        //{
        //    return MemoryMarshal.CreateSpan(ref left, length).SequenceEqual(MemoryMarshal.CreateSpan(ref right, length));
        //}

        /// <summary>
        /// Builds a table with position ranges from pattern that need to match; i: start of match, i+1: end of match.
        /// The first match is skipped and all indexes are shifted to the left by 1.
        /// </summary>
        /// <param name="szMask">The mask.</param>
        /// <param name="maskLength">The length of the mask.</param>
        /// <returns></returns>
        private static ReadOnlySpan<ushort> BuildMatchRanges(string szMask, int maskLength)
        {
            Span<ushort> fullMatchTable = stackalloc ushort[maskLength];
            fullMatchTable.Clear();

            int matchCount = 0;
            bool findEnd = false;
            for (ushort i = 1; i < maskLength; ++i)
            {
                if (szMask[i] != (findEnd ? '?' : 'x')) continue;
                findEnd = !findEnd;
                fullMatchTable[matchCount] = (ushort)(i - 1);

                matchCount++;
            }
            if (matchCount % 2 != 0)
                fullMatchTable[matchCount] = (ushort)(maskLength - 1);

            Span<ushort> matchTable = new Span<ushort>(new ushort[matchCount + 1]);
            fullMatchTable.Slice(0, matchCount + 1).CopyTo(matchTable);
            return matchTable;
        }
    }
}
