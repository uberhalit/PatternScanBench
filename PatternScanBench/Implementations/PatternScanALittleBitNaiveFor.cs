using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PatternScanBench.Implementations
{ 
    /// <summary>
    /// Pattern scan implementation 'ALittleBitNaiveFor' - by DI20ID
    /// https://github.com/DI20ID
    /// </summary>
    internal class PatternScanALittleBitNaiveFor
    {
        /// <summary>
        /// Returns address of pattern using 'ALittleBitNaiveFor' implementation by DI20ID. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal static long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            int cbMemoryL = cbMemory.Length;
            ref byte PcbMemory = ref cbMemory[0];

            int cbPatternL = cbPattern.Length - 1;
            ref byte PcbPattern = ref cbPattern[0];
            int[] cbPatternIndexes = new int[cbPatternL + 1];
            ref int PcbPatternIndexes = ref cbPatternIndexes[0];
            ref int iPcbPatternIndexes = ref cbPatternIndexes[0];

            char[] bMask = szMask.ToCharArray();
            byte[] bbMask = new byte[cbPatternL];
            ref char PbMask = ref bMask[0];
            ref byte PbbMask = ref bbMask[0];

            for (int i = 0; i <= cbPatternL; i++)
            {
                PbbMask = (byte)PbMask;
                iPcbPatternIndexes++;
                if (PbbMask == 120)
                {
                    iPcbPatternIndexes = ref Unsafe.Add(ref iPcbPatternIndexes, 1);
                    iPcbPatternIndexes = 0;
                }
                PbMask = ref Unsafe.Add(ref PbMask, 1);
                PbbMask = ref Unsafe.Add(ref PbbMask, 1);
            }

            for (int i = 0; i < cbMemoryL; i++, PcbMemory = ref Unsafe.Add(ref PcbMemory, 1))
            {
                if (PcbMemory == PcbPattern)
                {
                    if (Unsafe.Add(ref PcbMemory, cbPatternL) == Unsafe.Add(ref PcbPattern, cbPatternL))
                    {
                    ref byte xPcbMemory = ref PcbMemory;
                    ref byte xPcbPattern = ref PcbPattern;
                    ref int xPcbPatternIndexes = ref PcbPatternIndexes;

                        while (true)
                        {
                            xPcbPatternIndexes = ref Unsafe.Add(ref xPcbPatternIndexes, 1);
                            xPcbMemory = ref Unsafe.Add(ref xPcbMemory, xPcbPatternIndexes);
                            xPcbPattern = ref Unsafe.Add(ref xPcbPattern, xPcbPatternIndexes);
                            if (xPcbMemory != xPcbPattern)
                                break;
                            else if ((int)Unsafe.ByteOffset(ref PcbMemory, ref xPcbMemory) == cbPatternL)
                                return i;
                        }
                    }
                }
            }
            return -1;
        }
    }
}