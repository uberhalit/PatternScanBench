using System;
using System.Runtime.CompilerServices;

namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'Boyer-Moore-Horspool' - by mrexodia (ported to C# by uberhalit)
    /// https://github.com/mrexodia
    /// https://github.com/learn-more/findpattern-bench/blob/master/patterns/mrexodia_horspool.h
    /// Fast but can not match 0s and contains a bug where under certain circumstances the last few bytes of a partial match will get skipped.
    /// </summary>
    internal class PatternScanExodiaBMH
    {
        /// <summary>
        /// Represents a '?' in a byte pattern, can not be matched...
        /// </summary>
        private const byte WILDCARD = 0x00;

        /// <summary>
        /// Returns address of pattern using 'Boyer-Moore-Horspool' implementation by mrexodia. Can NOT match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal static long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            byte[] newPattern = new byte[cbPattern.Length];
            GenerateWildcardPattern(in cbPattern, ref newPattern, szMask);

            int hlen = cbMemory.Length;
            int nlen = cbPattern.Length;
            ref byte pHaystack = ref cbMemory[0];
            ref byte pNeedle = ref newPattern[0];

            uint[] bad_char_skip = new uint[256];   /* Officially called: bad character shift */

            /* ---- Preprocess ---- */
            /* Initialize the table to default value */
            /* When a character is encountered that does not occur
            * in the needle, we can safely skip ahead for the whole
            * length of the needle.
            */
            for (int scan = 0; scan <= 255; scan++)
            {
                bad_char_skip[scan] = (uint)nlen;
            }

            int last = nlen - 1;
            ref uint pBad_char_skip = ref bad_char_skip[0];

            /* Then populate it with the analysis of the needle */
            for (int scan = 0; scan < last; scan++)
            {
                byte needleByte = Unsafe.Add(ref pNeedle, scan);
                bad_char_skip[needleByte] = (uint)(last - scan);
            }

            /* ---- Do the matching ---- */

            /* Search the haystack, while the needle can still be within it. */
            int iHaystack = 0;
            while (hlen >= nlen)
            {
                /* scan from the end of the needle */
                for (int scan = last; matches(ref Unsafe.Add(ref pHaystack, scan), ref Unsafe.Add(ref pNeedle, scan)); scan--)
                {
                    if (scan == 0) /* If the first byte matches, we've found it. */
                        return iHaystack;
                }

                /* otherwise, we need to skip some bytes and start again.
                Note that here we are getting the skip value based on the last byte
                of needle, no matter where we didn't match. So if needle is: "abcd"
                then we are skipping based on 'd' and that value will be 4, and
                for "abcdd" we again skip on 'd' but the value will be only 1.
                The alternative of pretending that the mismatched character was
                the last character is slower in the normal case (E.g. finding
                "abcd" in "...azcd..." gives 4 by using 'd' but only
                4-2==2 using 'z'. */
                byte lastByte = Unsafe.Add(ref pHaystack, last);
                int skip = (int)Unsafe.Add(ref pBad_char_skip, lastByte);
                hlen -= skip;
                pHaystack = ref Unsafe.Add(ref pHaystack, skip);
                iHaystack += skip;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool matches(ref byte haystack_ch, ref byte needle_ch)
        {
            return haystack_ch == needle_ch || needle_ch == WILDCARD;
        }
    }
}
