
namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'NaiveFor' - by uberhalit
    /// https://github.com/uberhalit
    /// </summary>
    internal class PatternScanNaiveFor : PatternScanAlgorithm
    {
        /// <summary>
        /// Authors name.
        /// </summary>
        internal override string Creator => "uberhalit";

        internal PatternScanNaiveFor() { }

        /// <summary>
        /// Initializes a new 'PatternScanNaiveFor'.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <returns>An optional string to display next to benchmark results.</returns>
        internal override string Init()
        {
            return "";
        }

        /// <summary>
        /// Returns address of pattern using 'NaiveFor' implementation by uberhalit. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns></returns>
        internal override long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            long ix;
            int iy;
            bool bFound = false;
            int patternLength = cbPattern.Length;
            int dataLength = cbMemory.Length - patternLength;

            for (ix = 0; ix < dataLength; ix++)
            {
                bFound = true;
                for (iy = 0; iy < patternLength; iy++)
                {
                    if (szMask[iy] != 'x' || cbPattern[iy] == cbMemory[ix + iy])
                        continue;
                    bFound = false;
                    break;
                }

                if (bFound)
                    return ix;
            }

            return -1;
        }
    }
}
