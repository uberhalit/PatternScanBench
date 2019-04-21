
namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'BytePointerWithJIT' - by M i c h a e l (ported to C# by uberhalit)
    /// https://www.unknowncheats.me/forum/members/1074564.html
    /// https://github.com/learn-more/findpattern-bench/blob/master/patterns/Michael.h
    ///
    /// Ported in a way that the JIT uses some pointer arithmetics like in the original version.
    /// </summary>
    internal class PatternScanBytePointerWithJIT : PatternScanAlgorithm
    {
        internal override string Creator => "M i c h a e l";
        internal PatternScanBytePointerWithJIT() { }

        /// <summary>
        /// Initializes a new 'PatternScanBytePointerWithJIT'.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <returns>An optional string to display next to benchmark results.</returns>
        internal override string Init(in byte[] cbMemory)
        {
            return "";
        }

        /// <summary>
        /// Returns address of pattern using 'BytePointerWithJIT' implementation by M i c h a e l. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns></returns>
        internal override long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            int maskLength = szMask.Length;
            int search_len = cbMemory.Length;
            int iRegion_it = 0;
            byte region_it;
            for (; iRegion_it < search_len; ++iRegion_it)
            {
                region_it = cbMemory[iRegion_it];
                if (region_it == cbPattern[0])
                {
                    int iPattern_it = 0;
                    byte pattern_it;
                    int iMask_it = 0;
                    char mask_it;
                    int iMemory_it = iRegion_it;
                    byte memory_it;
                    bool found = true;
                    
                    for (; iMask_it < maskLength && iMemory_it < search_len; ++iMask_it, ++iPattern_it, ++iMemory_it)
                    {
                        pattern_it = cbPattern[iPattern_it];
                        mask_it = szMask[iMask_it];
                        memory_it = cbMemory[iMemory_it];

                        if (mask_it != 'x') continue;
                        if (memory_it != pattern_it)
                        {
                            found = false;
                            break;
                        }
                    }

                    if (found)
                        return iRegion_it;
                }
            }

            return -1;
        }
    }
}
