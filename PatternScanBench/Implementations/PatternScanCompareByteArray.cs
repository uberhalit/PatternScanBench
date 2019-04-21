
namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'CompareByteArray' - by fdsasdf (ported to C# by uberhalit)
    /// https://www.unknowncheats.me/forum/members/645501.html
    /// https://github.com/learn-more/findpattern-bench/blob/master/patterns/fdsasdf.h
    /// </summary>
    internal class PatternScanCompareByteArray : PatternScanAlgorithm
    {
        internal override string Creator => "fdsasdf";
        internal PatternScanCompareByteArray() { }

        private const byte wildcard = 0xCC;
        private bool CompareByteArray(in byte[] Data, ref int iData, in byte[] Signature, int signatureLength)
        {
            int iSignature = 0;
            for (; iSignature < signatureLength; ++iSignature, ++iData)
            {
                if (Signature[iSignature] == wildcard)
                    continue;
                if (Data[iData] != Signature[iSignature])
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Initializes a new 'PatternScanCompareByteArray'.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <returns>An optional string to display next to benchmark results.</returns>
        internal override string Init(in byte[] cbMemory)
        {
            return "";
        }

        /// <summary>
        /// Returns address of pattern using 'CompareByteArray' implementation by fdsasdf. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns></returns>
        internal override long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            byte First = cbPattern[0];
            int Max = cbMemory.Length - cbPattern.Length;
            int BaseAddress = 0;

            byte[] newPattern = GenerateWildcardPattern(cbPattern, szMask);
            int signatureLength = cbPattern.Length;

            for (; BaseAddress < Max; ++BaseAddress)
            {
                if (cbMemory[BaseAddress] != First) 
                    continue;
                if (CompareByteArray(in cbMemory, ref BaseAddress, in newPattern, signatureLength))
                    return BaseAddress - cbPattern.Length;
            }

            return -1;
        }

        private byte[] GenerateWildcardPattern(byte[] cbPattern, string szMask)
        {
            byte[] newPattern = new byte[cbPattern.Length];
            for (int i = 0; i < szMask.Length; i++)
            {
                if (szMask[i] != 'x') newPattern[i] = wildcard;
                else newPattern[i] = cbPattern[i];
            }

            return newPattern;
        }
    }
}
