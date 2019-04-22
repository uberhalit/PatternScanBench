using System.Linq;

namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'NaiveLINQ' - by lolp1
    /// https://github.com/lolp1
    /// </summary>
    internal class PatternScanNaiveLINQ : PatternScanAlgorithm
    {
        internal override string Creator => "lolp1";
        internal PatternScanNaiveLINQ() { }

        /// <summary>
        /// Initializes a new 'PatternScanNaiveLINQ'.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <returns>An optional string to display next to benchmark results.</returns>
        internal override string Init(in byte[] cbMemory)
        {
            return "";
        }

        /// <summary>
        /// Returns address of pattern using 'NaiveLINQ' implementation by lolp1. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal override long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            byte[] pattern = cbPattern;
            byte[] patternData = cbMemory;

            for (var offset = 0; offset < patternData.Length; offset++)
            {
                if (szMask.Where((m, b) => m == 'x' && pattern[b] != patternData[b + offset]).Any())
                    continue;

                return offset;
            }
            return -1;
        }
    }
}
