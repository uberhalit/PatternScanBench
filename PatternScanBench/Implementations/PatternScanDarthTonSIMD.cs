using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;

namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'DarthTonSIMD' - by DarthTon (ported to C# and fixed by uberhalit)
    /// https://github.com/DarthTon
    /// https://github.com/learn-more/findpattern-bench/blob/master/patterns/DarthTon.h
    /// 
    /// Original version contains multiple bugs (last byte of pattern does not get checked, if a match in first 16 bytes is a mismatch then next 16 bytes get skipped, possible match skips with MT).
    /// _mm_cmpestri() from original version isn't available in C# so I created a custom GetIndexFromVector().
    /// </summary>
    internal class PatternScanDarthTonSIMD
    {
        /// <summary>
        /// Contains data on pattern chunks.
        /// </summary>
        internal struct PartData
        {
            /// <summary>
            /// A bitmask created from the pattern part where 1 indicates an 'x' from szMask.
            /// </summary>
            internal int mask;

            /// <summary>
            /// Pattern part as vector, '?' have been replaced by 0.
            /// </summary>
            internal Vector128<byte> needle;
        };

        /// <summary>
        /// Initializes a new 'DarthTonSIMD'.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        internal static void Init(in byte[] cbMemory)
        {
            Vector128<byte> tmp = Vector128.Create((byte)0); // used to pre-load dependency if GC has over-optimized us out of existence already...
            if (!Sse2.IsSupported)
                throw new NotSupportedException("SIMD not HW accelerated...");
        }

        /// <summary>
        /// Returns address of pattern using 'DarthTonSIMD' implementation by DarthTon. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal static long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            ref byte pData = ref cbMemory[0];
            ref byte pPattern = ref cbPattern[0];
            long size = cbMemory.Length / 32 - 1;

            int len = szMask.Length;
            int first = szMask.IndexOf('?');
            int firstlen = first > -1 ? first : len;
            firstlen = firstlen > 16 ? 16 : firstlen;
            int num_parts = (len < 16 || len % 16 != 0) ? (len / 16 + 1) : (len / 16);
            PartData[] parts = new PartData[num_parts];
            for (int i = 0; i < num_parts; i++)
            {
                parts[i] = new PartData
                {
                    mask = 0,
                    needle = Vector128<byte>.Zero
                };
            }

            for (int i = 0; i < num_parts; ++i, len -= 16)
            {
                for (int j = 0; j < (len > 16 ? 16 : len); ++j)
                    if (szMask[16 * i + j] == 'x')
                        parts[i].mask = parts[i].mask | (1 << j);

                parts[i].needle = Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref pPattern, i * 16));
            }
            Vector128<byte> firstByte = Vector128.Create(cbPattern[0]);
            ref Vector128<byte> pFirstByte = ref firstByte;
            ref Vector128<byte> needleVec = ref parts[0].needle;
            ref PartData part = ref parts[0];

            for (int i = 0; i < size; ++i)
            {
                int offset = GetIndexFromVector(ref pFirstByte, ref needleVec, ref Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref pData, i * 32)), firstlen);
                if (offset == 16)
                {
                    offset += GetIndexFromVector(ref pFirstByte, ref needleVec, ref Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref pData, i * 32 + 16)), firstlen);
                    if (offset == 32)
                        continue;
                }

            checkHay:;
                for (int j = 0; j < num_parts; ++j)
                {
                    Vector128<byte> hay = Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref pData, (2 * i + j) * 16 + offset));
                    int bitmask = Sse2.MoveMask(Sse2.CompareEqual(hay, parts[j].needle));
                    if ((bitmask & Unsafe.Add(ref part, j).mask) != Unsafe.Add(ref part, j).mask)
                        goto secondMatch;
                }

                return 32 * i + offset;

            secondMatch:;
                offset += GetIndexFromVector(ref pFirstByte, ref needleVec, ref Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref pData, i * 32 + offset + 1)), firstlen) + 1;
                if (offset > 32)
                    continue;
                goto checkHay;
            }

            return -1;
        }

        /// <summary>
        /// Returns address of pattern using 'DarthTonSIMD (MT)' implementation by DarthTon. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        /// <remarks>Runs multithreaded heavily CPU-bound.</remarks>
        internal static long FindPattern_MT(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            byte[] cxMemory = cbMemory;
            ref byte pPattern = ref cbPattern[0];
            int size = cbMemory.Length / 32 - 1;

            int len = szMask.Length;
            int first = szMask.IndexOf('?');
            int firstlen = first > -1 ? first : len;
            firstlen = firstlen > 16 ? 16 : firstlen;
            int num_parts = (len < 16 || len % 16 != 0) ? (len / 16 + 1) : (len / 16);
            PartData[] parts = new PartData[num_parts];
            for (int i = 0; i < num_parts; i++)
            {
                parts[i] = new PartData
                {
                    mask = 0,
                    needle = Vector128<byte>.Zero
                };
            }

            for (int i = 0; i < num_parts; ++i, len -= 16)
            {
                for (int j = 0; j < (len > 16 ? 16 : len); ++j)
                    if (szMask[16 * i + j] == 'x')
                        parts[i].mask = parts[i].mask | (1 << j);

                parts[i].needle = Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref pPattern, i * 16));
            }
            Vector128<byte> firstByte = Vector128.Create(cbPattern[0]);
            long result = -1;

            Parallel.For(0, size, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, (i, state) =>
            {
                ref Vector128<byte> pFirstByte = ref firstByte;
                ref PartData part = ref MemoryMarshal.GetArrayDataReference(parts);
                ref Vector128<byte> needleVec = ref part.needle;
                ref byte pDataInternal = ref MemoryMarshal.GetArrayDataReference(cxMemory);

                int offset = GetIndexFromVector(ref pFirstByte, ref needleVec, ref Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref pDataInternal, i * 32)), firstlen);
                if (offset == 16)
                {
                    offset += GetIndexFromVector(ref pFirstByte, ref needleVec, ref Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref pDataInternal, i * 32 + 16)), firstlen);
                    if (offset == 32)
                        goto ends;
                }

            checkHay:;
                for (int j = 0; j < num_parts; ++j)
                {
                    ref PartData jPart = ref Unsafe.Add(ref part, j);
                    Vector128<byte> hay = Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref pDataInternal, (2 * i + j) * 16 + offset));
                    int bitmask = Sse2.MoveMask(Sse2.CompareEqual(hay, jPart.needle));
                    if ((bitmask & jPart.mask) != jPart.mask)
                        goto secondMatch;
                }
                if (result == -1)
                    result = 32 * i + offset;
                else if (result > 32 * i + offset)  // we could end up with a thread further up in memory finishing earlier than the thread working on the first occurrence of the pattern
                    result = 32 * i + offset;
                state.Break();
                goto ends;

            secondMatch:;
                offset += GetIndexFromVector(ref pFirstByte, ref needleVec, ref Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref pDataInternal, i * 32 + offset + 1)), firstlen) + 1;
                if (offset > 32)
                    goto ends;
                goto checkHay;

            ends:;
            });

            return result;
        }

        /// <summary>
        /// Looks for needle vector in haystack vector and returns its offset, optionally check only a portion of needle.
        /// If needdle matches partially at end, it will return this offset.
        /// </summary>
        /// <param name="firstByte">The vector filled with the first byte of the pattern.</param>
        /// <param name="needle">The vector to look for.</param>
        /// <param name="haystack">The vector to search in for.</param>
        /// <param name="needleLen">The length of needle vector that should be matched.</param>
        /// <returns>16 if not found.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetIndexFromVector(ref Vector128<byte> firstByte, ref Vector128<byte> needle, ref Vector128<byte> haystack, int needleLen = 16)
        {
            // Sse2.CompareEqual (_mm_cmpeq_epi8) compares every byte of two vectors and sets resulting vector byte to 255 if match.
            // Sse2.MoveMask (_mm_movemask_epi8) creates an integer representation of a vector, setting first 16 lowbits according to 255 or 0 in vector.
            // Sse2.ShiftLeftLogical128BitLane (_mm_bslli_si128) shifts all bytes in a vector x bytes TO THE RIGHT adding in 0 on the left side
            // Int32 first 16 lowbits set = 0xFFFF
            if (needleLen == 16)
                return Sse2.MoveMask(Sse2.CompareEqual(needle, haystack)) == 0xFFFF ? 0 : 16;
            else
            {
                int firstByteCmp = Sse2.MoveMask(Sse2.CompareEqual(firstByte, haystack));
                int fullOffset = 0;
                var shiftNeedle = needle;
                int i = 0;
                int n = 0;
                while (firstByteCmp != 0)
                {
                    int offset = BitOperations.TrailingZeroCount((uint)firstByteCmp);
                    fullOffset += offset + n;

                    // WARNING: ShiftLogical128BitLane should only be called with a compile-time constant second parameter (1 in this case) as having a variable hurts performance massively
                    for (int j = 0 - n; j < offset; j++)
                        shiftNeedle = Sse2.ShiftLeftLogical128BitLane(shiftNeedle, 1);

                    int oCmp = Sse2.MoveMask(Sse2.CompareEqual(shiftNeedle, haystack));
                    bool found = true;
                    for (int j = 0; j < needleLen && (j + fullOffset) < 16; j++)
                    {
                        if (((oCmp >> j + fullOffset) & 1) == 1)
                            continue;
                        found = false;
                        break;
                    }
                    if (found)
                        return fullOffset;
                    else
                    {
                        if ((BitOperations.LeadingZeroCount((uint)firstByteCmp) - 16) < 16 - offset)
                        {
                            // another match possible
                            firstByteCmp >>= offset + 1;
                            i++;
                            n = 1;
                        }
                    }
                }
            }
            return 16;
        }

        /// <summary>
        /// A shift mask used to create vector masks with n number of set bytes.
        /// </summary>
        private static readonly byte[] vectorShiftmask = new byte[32] {
            (byte)0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                  0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };
        /// <summary>
        /// Looks for needle vector in haystack vector and returns its offset, optionally check only a portion of needle.
        /// If needdle matches partially at end, it will return this offset.
        /// </summary>
        /// <param name="needle">The vector to look for.</param>
        /// <param name="haystack">The vector to search in for.</param>
        /// <param name="pVectorMsk">A ref to vectorShiftmask[0].</param>
        /// <param name="needleLen">The length of needle vector that should be matched.</param>
        /// <returns>16 if not found.</returns>
        /// <remarks>
        /// By KingRikkie
        /// https://github.com/KingRikkie
        /// https://github.com/dotnet/runtime/issues/957#issuecomment-962005493
        /// </remarks>
        private static int GetIndexFromVectorB(ref Vector128<byte> needle, ref Vector128<byte> haystack, ref byte pVectorMsk, int needleLen = 16)
        {
            // Sse2.CompareEqual (_mm_cmpeq_epi8) compares every byte of two vectors and sets resulting vector byte to 255 if match.
            // Sse2.MoveMask (_mm_movemask_epi8) creates an integer representation of a vector, setting first 16 lowbits according to 255 or 0 in vector.
            // Sse41.BlendVariable (_mm_blendv_epi8) puts a vector ontop of another one using a blend mask where 255 indicates overwrite and 0 indicates no copy.
            // Sse2.ShiftLeftLogical128BitLane (_mm_bslli_si128) shifts all bytes in a vector x bytes TO THE RIGHT adding in 0 on the left side
            // Int32 first 16 lowbits set = 0xFFFF
            if (needleLen == 16)
                return Sse2.MoveMask(Sse2.CompareEqual(needle, haystack)) == 0xFFFF ? 0 : 16;
            else
            {
                // get a mask where every byte that we need to match is 255 to later blend it over the haystack
                var blendMsk = Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref pVectorMsk, 16 - needleLen));
                var shiftNeedle = needle;
                for (int i = 0; i < 16; i++)
                {
                    if (Sse2.MoveMask(Sse2.CompareEqual(haystack, Sse41.BlendVariable(haystack, shiftNeedle, blendMsk))) == 0xFFFF)
                        return i;
                    // WARNING: ShiftLogical128BitLane should only be called with a compile-time constant second parameter (1 in this case) as having a variable hurts performance massively
                    blendMsk = Sse2.ShiftLeftLogical128BitLane(blendMsk, 1);
                    shiftNeedle = Sse2.ShiftLeftLogical128BitLane(shiftNeedle, 1);
                }
            }
            return 16;
        }
    }
}
