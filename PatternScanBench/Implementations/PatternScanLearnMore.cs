
namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'LearnMore' - by learn_more (ported to C# by uberhalit)
    /// https://github.com/learn-more
    /// https://www.unknowncheats.me/forum/members/124636.html
    /// https://github.com/learn-more/findpattern-bench/blob/master/patterns/learn_more.h
    ///
    /// Could be improved by using fast Span<T> from System.Memory in .NET Core...
    /// </summary>
    internal class PatternScanLearnMore : PatternScanAlgorithm
    {
        internal override string Creator => "learn_more";
        internal PatternScanLearnMore() { }

        /// <summary>
        /// Initializes a new 'PatternScanLearnMore'.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <returns>An optional string to display next to benchmark results.</returns>
        internal override string Init(in byte[] cbMemory)
        {
            return "";
        }

        /// <summary>
        /// Returns address of pattern using 'LearnMore' implementation by learn_more. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal override long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            int rangeEnd = cbMemory.Length;
            char[] mas = szMask.ToCharArray();

            int iFirstMatch = -1;
            int iPat = 0;
            for (int iPcur = 0; iPcur < rangeEnd; ++iPcur)
            {
                byte pCur = cbMemory[iPcur];
                if (mas[iPat] == '?' || pCur == cbPattern[iPat])
                {
                    if (iFirstMatch < 0 || iFirstMatch >= rangeEnd)
                        iFirstMatch = iPcur;
                    if (iPat < mas.Length - 2 && (mas[iPat] == '?' && mas[iPat + 1] == '?'))
                    {
                        iPat += 2;
                        iPcur++;
                    }
                    else
                        iPat++;
                    if (iPat >= cbPattern.Length)
                        return iFirstMatch;
                }
                else if (iFirstMatch > -1 && iFirstMatch < rangeEnd)
                {
                    iPcur = iFirstMatch;
                    iPat = 0;
                    iFirstMatch = -1;
                }
            }

            return -1;
        }
    }
}
