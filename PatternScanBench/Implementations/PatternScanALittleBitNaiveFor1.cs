﻿using System.Collections.Generic;

namespace PatternScanBench.Implementations
{ 
    /// <summary>
    /// Pattern scan implementation 'ALittleBitNaiveFor' - by DI20ID
    /// https://github.com/DI20ID
    /// </summary>
    internal class PatternScanALittleBitNaiveFor1
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
            long ix;
            int iy;
            Dictionary<byte,byte> not0PatternBytesDic = new Dictionary<byte, byte>();
            List<byte> not0PatternBytesList = new List<byte>();
            List<int> not0PatternBytesIndexList = new List<int>();
            
            int dataLength = cbMemory.Length - cbPattern.Length;

            for (iy = cbPattern.Length - 1; iy > -1; iy--)
            {
                if (szMask[iy] == 'x')
                {
                    not0PatternBytesList.Add(cbPattern[iy]);
                    not0PatternBytesIndexList.Add(iy);
                }
            }

            byte[] not0PatternBytesArray = not0PatternBytesList.ToArray();
            int not0PatternBytesL = not0PatternBytesArray.Length;
            int[] not0PatternBytesIndexArray = not0PatternBytesIndexList.ToArray();

            for (ix = 0; ix < dataLength; ix++)
            {
                if (not0PatternBytesArray[not0PatternBytesL - 1] != cbMemory[ix]) continue;
                bool check = true;
            
                for (iy = not0PatternBytesL - 1; iy > -1; iy--)
                {
                    if (not0PatternBytesArray[iy] == cbMemory[ix + not0PatternBytesIndexArray[iy]])
                        continue;
                    check = false;
                    break;
                }
            
                if (check)
                {
                    return ix;
                }
            }

            return -1;
        }
    }
}
