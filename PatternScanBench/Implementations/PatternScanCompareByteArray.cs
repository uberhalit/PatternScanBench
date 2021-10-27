using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'CompareByteArray' - by fdsasdf (ported to C# by uberhalit)
    /// https://www.unknowncheats.me/forum/members/645501.html
    /// https://github.com/learn-more/findpattern-bench/blob/master/patterns/fdsasdf.h
    ///
    /// Altered refs while porting to improve performance with JIT.
    /// </summary>
    internal class PatternScanCompareByteArray
    {
        /// <summary>
        /// Represents a '?' in a byte pattern, can not be matched...
        /// </summary>
        private const byte wildcard = 0xCC;

        /// <summary>
        /// Returns address of pattern using 'CompareByteArray' implementation by fdsasdf. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal static long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            byte First = cbPattern[0];
            long iBaseAddress = 0;
            int Max = cbMemory.Length - cbPattern.Length;

            byte[] newPattern = new byte[cbPattern.Length];
            GenerateWildcardPattern(in cbPattern, ref newPattern, szMask);
            int signatureLength = cbPattern.Length;

            ref byte BaseAddress = ref cbMemory[0];

            for (; iBaseAddress < Max; ++iBaseAddress, BaseAddress = ref Unsafe.Add(ref BaseAddress, 1))
            {
                if (BaseAddress != First) 
                    continue;
                if (CompareByteArray(ref BaseAddress, ref newPattern, signatureLength))
                    return iBaseAddress;
            }

            return -1;
        }

        private static void GenerateWildcardPattern(in byte[] cbPattern, ref byte[] newPattern, string szMask)
        {
            Buffer.BlockCopy(cbPattern, 0, newPattern, 0, cbPattern.Length);
            for (int i = 0; i < szMask.Length; i++)
            {
                if (szMask[i] != 'x') newPattern[i] = wildcard;
            }
        }

        private static bool CompareByteArray(ref byte Data, ref byte[] newPattern, int signatureLength)
        {
            ref byte Signature = ref Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(newPattern), 1);
            int iSignature = 1;
            Data = ref Unsafe.Add(ref Data, 1);
            for (; iSignature < signatureLength; ++iSignature, Data = ref Unsafe.Add(ref Data, 1), Signature = ref Unsafe.Add(ref Signature, 1))
            {
                if (Signature == wildcard)
                    continue;
                if (Data != Signature)
                    return false;
            }
            return true;
        }
    }
}
