using System.Collections.Generic;

namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'ALittleBitNaiveFor' - by DI20ID
    /// https://github.com/DI20ID
    /// </summary>
    internal class PatternScanALittleBitNaiveFor : PatternScanAlgorithm
    {
        /// <summary>
        /// Authors name.
        /// </summary>
        internal override string Creator => "DI20ID";
        internal PatternScanALittleBitNaiveFor() { }

        /// <summary>
        /// Initializes a new 'PatternScanTest'.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <returns>An optional string to display next to benchmark results.</returns>
        internal override string Init(in byte[] cbMemory)
        {
            return "";
        }

        /// <summary>
        /// Returns address of pattern using 'ALittleBitNaiveFor' implementation by DI20ID. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal override long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            long ix;
            int iy;

            List<byte> ls = new List<byte>();
            List<int> ini = new List<int>();

            int dataLength = cbMemory.Length - cbPattern.Length;

            for (iy = cbPattern.Length - 1; iy > -1; iy--)
            {
                if (szMask[iy] == 'x')
                {

                    ls.Add(cbPattern[iy]);
                    ini.Add(iy);
                }
            }

            byte[] arr = ls.ToArray();
            byte[] arr1 = new byte[arr.Length];

            for (ix = 0; ix < dataLength; ix++)
            {
                if (arr[arr.Length - 1] == cbMemory[ix])
                {
                    bool check = true;

                    for (iy = arr.Length - 1; iy > -1; iy--)
                    {
                        if (arr[iy] == cbMemory[ix + ini[iy]])
                            continue;
                        check = false;
                        break;
                    }

                    if (check)
                    {
                        return ix;
                    }
                }
            }

            return -1;
        }
    }
}
