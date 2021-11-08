using System.Runtime.CompilerServices;

namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'Boyer-Moore-Horspool' - by Forza (ported and fixed to C# by uberhalit)
    /// https://github.com/MyriadSoft
    /// https://github.com/learn-more/findpattern-bench/blob/master/patterns/Forza.h
    /// 
    /// Original version counts from back to front and finds last occurence of pattern first, additionally contained a bug where last byte of pattern is skipped.
    /// </summary>
    internal class PatternScanForzaBMH
    {
        /// <summary>
        /// Returns address of pattern using 'Boyer-Moore-Horspool' implementation by Forza. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal static long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            int Length = cbMemory.Length;
            FindLargestArray(szMask, out int[] d);

            byte len = (byte)szMask.Length;
            byte mbeg = (byte)d[0];
            byte mlen = (byte)d[1];
            byte mfirst = cbPattern[mbeg];

            byte[] wildcard = new byte[256];

            for (int i = mbeg; i < mbeg + mlen; i++)
                wildcard[cbPattern[i]] = 1;

            ref byte pCbMemory = ref cbMemory[0];
            ref byte pCbPattern = ref cbPattern[0];
            ref byte pWildcard = ref wildcard[0];

            char[] mask = szMask.ToCharArray();
            ref char pMask = ref mask[0];

            for (int i = 0; i < Length - len; i++)
            {
                byte c = Unsafe.Add(ref pCbMemory, i);
                byte w = Unsafe.Add(ref pWildcard, c);
                int k = 0;
            
                while (w == 0 && i < mlen)
                {
                    i += mlen;
                    w = Unsafe.Add(ref pWildcard, Unsafe.Add(ref pCbMemory, i));
                    k = 1;
                }
            
                if (k == 1)
                {
                    i--;
                    continue;
                }
            
                if (c != mfirst)
                    continue;
            
                for (int j = 0; j < len; j++)
                {
                    if (j == mbeg || Unsafe.Add(ref pMask, j) != 'x')
                        continue;

                    if (Unsafe.Add(ref pCbMemory, i - mbeg + j) != Unsafe.Add(ref pCbPattern, j))
                        break;
            
                    if (j + 1 == len)
                        return i - mbeg;
                }
            }

             return -1;
        }

        /// <summary>
        /// Gets position and length of the largest matching block from the mask.
        /// </summary>
        /// <param name="szMask">The pattern mask.</param>
        /// <param name="Out">The array containing the result.</param>
        /// <returns>An int array where a[0] is the starting position and a[1] is the length of the largest matching block.</returns>
        private static void FindLargestArray(string szMask, out int[] Out)
        {
            int t1 = 0;
            int t2 = GetLengthToNextWildcard(szMask, 0);
            int len = szMask.Length;

            for (int j = t2; j < len; j++)
            {
                if (szMask[j] != 'x')
                    continue;

                int count = GetLengthToNextWildcard(szMask, j);

                if (count > t2)
                {
                    t1 = j;
                    t2 = count;
                }

                j += (count - 1);
            }

            Out = new int[2];
            Out[0] = t1;
            Out[1] = t2;
        }

        private static int GetLengthToNextWildcard(string szMask, int offset) 
        { 
            for (int i = 0; i + offset < szMask.Length; i++)
            {
                if (szMask[i + offset] == '?')
                    return i;
            }
            return szMask.Length - offset;
        }
    }
}
