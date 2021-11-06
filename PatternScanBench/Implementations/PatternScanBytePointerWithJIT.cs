using System.Runtime.CompilerServices;

namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'BytePointerWithJIT' - by M i c h a e l (ported to C# by uberhalit)
    /// https://www.unknowncheats.me/forum/members/1074564.html
    /// https://github.com/learn-more/findpattern-bench/blob/master/patterns/Michael.h
    ///
    /// Ported in such a manner that the JIT uses some pointer arithmetics like in the original version.
    /// </summary>
    internal class PatternScanBytePointerWithJIT
    {
        /// <summary>
        /// Returns address of pattern using 'BytePointerWithJIT' implementation by M i c h a e l. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal static long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            int maskLength = szMask.Length;
            int search_len = cbMemory.Length;
            ref byte region_it = ref cbMemory[0];
            ref byte pattern = ref cbPattern[0];

            for (int i = 0; i < search_len; ++i, region_it = ref Unsafe.Add(ref region_it, 1))
            {
                if (region_it == pattern)
                {
                    ref byte pattern_it = ref pattern;
                    ref byte memory_it = ref region_it;
                    bool found = true;

                    for (int j = 0; j < maskLength && (i + j) < search_len; ++j, memory_it = ref Unsafe.Add(ref memory_it, 1), pattern_it = ref Unsafe.Add(ref pattern_it, 1))
                    {
                        if (szMask[j] != 'x') continue;
                        if (memory_it != pattern_it)
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                        return i;
                }
            }

            return -1;
        }
    }
}
