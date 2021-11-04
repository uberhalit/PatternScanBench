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
            int cbPatternL = cbPattern.Length;
            int[] cbPatternIndexes = new int[cbPatternL+1];
            
            char[] bMask = szMask.ToCharArray();
            int tcbPatternL = 0;
            int l = 0;

            ref byte PcbMemory = ref cbMemory[0];
            ref byte PcbPattern = ref cbPattern[0];
            ref int PcbPatternIndexes = ref cbPatternIndexes[0];
            
            ref char PbMask = ref bMask[0];

            for (int i = 0; i < cbPatternL; i++)
            {
                l++;
                if(PbMask == 120)
                {
                   tcbPatternL++;
                   PcbPatternIndexes = l;
                   PcbPatternIndexes = ref Unsafe.Add(ref PcbPatternIndexes, 1);
                   l = 0;
                }
                PbMask = ref Unsafe.Add(ref PbMask, 1);
            }

            PcbPatternIndexes = ref cbPatternIndexes[0];

            for (int i = 0; i < cbMemoryL; i++, PcbMemory = ref Unsafe.Add(ref PcbMemory, 1))
            {
               //if(i == 0x198A9A)
               //{
               //    int k = 10;
               //}
                if(PcbMemory == PcbPattern)
                {
                    ref byte xPcbMemory = ref PcbMemory;
                    ref byte xPcbPattern = ref PcbPattern;
                    ref int xPcbPatternIndexes = ref PcbPatternIndexes;
                    bool check = true;
            
                    for (int j = 0; j < tcbPatternL; j++, xPcbPatternIndexes = ref Unsafe.Add(ref xPcbPatternIndexes, 1), xPcbMemory = ref Unsafe.Add(ref xPcbMemory, xPcbPatternIndexes), xPcbPattern = ref Unsafe.Add(ref xPcbPattern, xPcbPatternIndexes))
                    {
                        if(xPcbMemory != xPcbPattern)
                        {
                            check = false;
                            break;
                        }
                        else if(j == tcbPatternL -1)
                        {
                            if (check) return i;
                        }
                    }
                }
            }

            return -1;
        }
    }
}
//while (true)
//{
//    if (PcbMemory == PcbPattern)
//    {
//        ref byte xPcbMemory = ref PcbMemory;
//        ref byte xPcbPattern = ref PcbPattern;
//        ref int xPcbPatternIndexes = ref PcbPatternIndexes;
//        while (xPcbMemory == xPcbPattern)
//        {
//            if ((int)Unsafe.ByteOffset(ref PcbMemory, ref xPcbMemory) == cbPatternL - 1)
//            {
//                int n = (int)Unsafe.ByteOffset(ref cbMemory[0], ref PcbMemory);
//                return n;
//            }
//            xPcbPatternIndexes = ref Unsafe.Add(ref xPcbPatternIndexes, 1);
//            xPcbMemory = ref Unsafe.Add(ref xPcbMemory, xPcbPatternIndexes); xPcbPattern = ref Unsafe.Add(ref xPcbPattern, xPcbPatternIndexes);
//        }
//    }
//    PcbMemory = ref Unsafe.Add(ref PcbMemory, 1);
//}
