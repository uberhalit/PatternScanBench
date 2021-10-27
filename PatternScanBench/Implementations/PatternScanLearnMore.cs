using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'LearnMore' - by learn_more (ported to C# by uberhalit)
    /// https://github.com/learn-more
    /// https://www.unknowncheats.me/forum/members/124636.html
    /// https://github.com/learn-more/findpattern-bench/blob/master/patterns/learn_more.h
    ///
    /// Could be improved by using Span<T>...
    /// </summary>
    internal class PatternScanLearnMore 
    {
        /// <summary>
        /// Represents a '?' in a byte pattern, can not be matched...
        /// </summary>
        private const byte wildcard = 0xCC;

        /// <summary>
        /// Returns address of pattern using 'LearnMore' implementation by learn_more. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal static long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            byte[] newPattern = new byte[cbPattern.Length];
            GenerateWildcardPattern(in cbPattern, ref newPattern, szMask);

            int rangeEnd = cbMemory.Length;
            int patternEnd = cbPattern.Length;
            ref byte pCur = ref cbMemory[0];
            ref byte pat = ref newPattern[0];
            int iFirstMatch = 0;
            int iPat = 0;

            for (int iPcur = 0; iPcur < rangeEnd; ++iPcur, pCur = ref Unsafe.Add(ref pCur, 1))
            {
                if (pCur == pat || pat == wildcard)
                {
                    if (iFirstMatch == 0)
                        iFirstMatch = iPcur;
                    iPat++;
                    if (iPat >= patternEnd)
                        return iFirstMatch;
                    pat = ref Unsafe.Add(ref pat, 1);
                }
                else if (iFirstMatch > 0)
                {
                    iPcur = iFirstMatch;
                    pCur = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(cbMemory), iFirstMatch);
                    pat = ref MemoryMarshal.GetArrayDataReference(newPattern);
                    iPat = 0;
                    iFirstMatch = 0;
                }
            }

            return -1;
        }

        private static void GenerateWildcardPattern(in byte[] cbPattern, ref byte[] newPattern, string szMask)
        {
            Buffer.BlockCopy(cbPattern, 0, newPattern, 0, cbPattern.Length);
            for (int i = 0; i < szMask.Length; i++)
            {
                if (szMask[i] != 'x') newPattern[i] = wildcard;
            }
        }

        /// <summary>
        /// Returns address of pattern using 'LearnMore v2' implementation by learn_more. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal static long FindPatternV2(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            int mskLen = szMask.Length;
            int len = cbMemory.Length - mskLen;
            ref byte rangestart = ref cbMemory[0];
            ref byte patt_base = ref cbPattern[0];
            ReadOnlySpan<char> msk_base = szMask.AsSpan();

            for (int n = 0; n < len; ++n)
            {
                if (IsMatch(ref Unsafe.Add(ref rangestart, n), ref patt_base, ref msk_base, mskLen)) {
                    return n;
                }
            }

            return -1;
        }

        private static bool IsMatch(ref byte addr, ref byte pat, ref ReadOnlySpan<char> msk, int mskLength)
        {
            int n = 0;
            while (Unsafe.Add(ref addr, n) == Unsafe.Add(ref pat, n) || msk[n] == '?') {
                if (++n >= mskLength) {
                    return true;
                }
            }
            return false;
        }
    }
}
