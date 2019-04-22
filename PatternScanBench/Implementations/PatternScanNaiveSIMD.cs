using System;
using System.Numerics;

namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'NaiveSIMD' - by uberhalit
    /// https://github.com/uberhalit
    /// 
    /// Uses SIMD instructions on AVX-supporting processors, the longer the pattern the more efficient this should get.
    /// Requires RyuJIT compiler for hardware acceleration which **should** be enabled by default on newer VS versions.
    /// Ideally a pattern would be a multiple of (xmm0 register size) / 8 so all available space gets used in calculations.
    /// Can be optimized further quiet dramatically as currently the compiler adds a lot of unnecessary array bounds checks.
    /// </summary>
    internal class PatternScanNaiveSIMD : PatternScanAlgorithm
    {
        internal override string Creator => "uberhalit";
        internal PatternScanNaiveSIMD() { }

        private static readonly int _simdLength = Vector<byte>.Count;

        /// <summary>
        /// Initializes a new 'PatternScanNaiveSIMD'.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <returns>An optional string to display next to benchmark results.</returns>
        internal override string Init(in byte[] cbMemory)
        {
            JitVersion jitVersion = new JitVersionInfo().GetJitVersion();
            if (jitVersion == JitVersion.Unknown)
                return "SIMD support not determined";
            if (!Vector.IsHardwareAccelerated || jitVersion != JitVersion.RyuJit)
                return "SIMD not HW accelerated...";
            return "";
        }

        /// <summary>
        /// Returns address of pattern using 'NaiveSIMD' implementation by uberhalit. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal override long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            int maskLength = szMask.Length;
            ushort[] matchTable = BuildMatchIndexes(szMask, maskLength);
            int matchTableLength = matchTable.Length;
            Vector<byte>[] patternVectors = PadPatternToVector(cbPattern);

            int searchLength = cbMemory.Length - (_simdLength > cbPattern.Length ? _simdLength : cbPattern.Length);
            for (int position = 0; position < searchLength; position++)
            {
                if (cbMemory[position] == cbPattern[0])
                {
                    int iMatchTableIndex = 0;
                    bool found = true;
                    for (int i = 0; i < patternVectors.Length; i++)
                    {
                        Vector<byte> compareResult = patternVectors[i] - new Vector<byte>(cbMemory, position + 1 + (i * _simdLength));
                        for (; iMatchTableIndex < matchTableLength; iMatchTableIndex++)
                        {
                            int matchIndex = matchTable[iMatchTableIndex];
                            if (i > 0) matchIndex -= i * _simdLength;
                            if (matchIndex >= _simdLength)
                                break;
                            if (compareResult[matchIndex] == 0x00)
                                continue;
                            found = false;
                            break;
                        }

                        if (!found)
                            break;
                    }

                    if (found)
                        return position;
                }
            }

            return -1;
        }

        /// <summary>
        /// Builds a table that indicates which positions in pattern should be matched, the first match is skipped and all indexes are shifted to the left by 1.
        /// </summary>
        /// <param name="szMask">The mask.</param>
        /// <param name="maskLength">The length of the mask.</param>
        /// <returns></returns>
        private static ushort[] BuildMatchIndexes(string szMask, int maskLength)
        {
            ushort[] fullMatchTable = new ushort[maskLength];
            int matchCount = 0;
            for (ushort i = 1; i < maskLength; i++)
            {
                if (szMask[i] != 'x') continue;
                fullMatchTable[matchCount] = (ushort)(i - 1);
                matchCount++;
            }
            ushort[] matchTable = new ushort[matchCount];
            Array.Copy(fullMatchTable, matchTable, matchCount);
            return matchTable;
        }

        /// <summary>
        /// Generates byte-Vectors that are right-padded with 0 from a pattern. The first byte is skipped.
        /// </summary>
        /// <param name="cbPattern">The pattern.</param>
        /// <returns></returns>
        private static Vector<byte>[] PadPatternToVector(byte[] cbPattern)
        {
            int vectorCount = (int)Math.Ceiling((cbPattern.Length - 1) / (float)_simdLength);
            byte[] paddedPattern = new byte[vectorCount * _simdLength];
            Buffer.BlockCopy(cbPattern, 1, paddedPattern, 0, cbPattern.Length - 1);
            Vector<byte>[] patternVectors = new Vector<byte>[vectorCount];
            for (int i = 0; i < vectorCount; i++)
                patternVectors[i] = new Vector<byte>(paddedPattern, _simdLength * i);
            return patternVectors;
        }

        private enum JitVersion
        {
            MsX64, RyuJit, Unknown
        }

        private class JitVersionInfo
        {
            internal JitVersion GetJitVersion()
            {
                #if DEBUG
                return JitVersion.Unknown;
                #endif
                if (IsMsX64())
                    return JitVersion.MsX64;
                return JitVersion.RyuJit;
            }

            private int bar;

            /// <summary>
            /// Uses the JIT-x64 sub-expression elimination optimizer _bug to check if we use MsX64 JIT.
            /// Only works with code optimizations enabled so we can't determine JIT in DEBUG.
            /// </summary>
            /// https://aakinshin.net/posts/subexpression-elimination-bug-in-jit-x64/
            /// <returns>True if _bug is present; default x64 JIT is used.</returns>
            private bool IsMsX64(int step = 1)
            {
                var value = 0;
                for (int i = 0; i < step; i++)
                {
                    bar = i + 10;
                    for (int j = 0; j < 2 * step; j += step)
                        value = j + 10;
                }
                return value == 20 + step;
            }
        }
    }
}
