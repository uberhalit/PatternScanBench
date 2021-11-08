using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'SIMD' - by Forza (ported to C# and fixed by uberhalit)
    /// https://github.com/MyriadSoft
    /// https://github.com/learn-more/findpattern-bench/blob/master/patterns/Forza.h
    /// 
    /// Original version counts from back to front and finds last occurence of pattern first which is not valid in general case.
    /// Additionally contained a bug where if a match in first 16 bytes is a mismatch then next 16 bytes get skipped.
    /// _mm_cmpestri() from original version isn't available in C# so I created custom GetIndexFromVector() and GetEqualityIndexFromVector().
    /// </summary>
    internal class PatternScanForzaSIMD
    {
        /// <summary>
        /// Contains data on pattern.
        /// </summary>
        internal struct PatternData
        {
            /// <summary>
            /// Total count of pattern vectors.
            /// </summary>
            internal int Count;

            /// <summary>
            /// Total length of pattern.
            /// </summary>
            internal int Size;

            /// <summary>
            /// Match lengths of vectors.
            /// </summary>
            internal int[] Length;

            /// <summary>
            /// Amount of bytes to skip on vectors.
            /// </summary>
            internal int[] Skip;

            /// <summary>
            /// Pattern parts as vector, '?' have been replaced by 0.
            /// </summary>
            internal Vector128<byte>[] Value;
        };

        /// <summary>
        /// Initializes a new 'PatternScanForzaSIMD'.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        internal static void Init(in byte[] cbMemory)
        {
            Vector128<byte> tmp = Vector128.Create((byte)0); // used to pre-load dependency if GC has over-optimized us out of existence already...
            Vector256<byte> tm2 = Vector256.Create((byte)0);
            if (!Sse2.IsSupported || !Avx.IsSupported)
                throw new NotSupportedException("SIMD not HW accelerated...");
        }

        /// <summary>
        /// Returns address of pattern using 'SIMD' implementation by Forza. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal static long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            ref byte Data = ref cbMemory[0];
            ref byte Signature = ref cbPattern[0];
            char[] msk = szMask.ToCharArray();
            ref char Mask = ref msk[0];
            GeneratePattern(ref Signature, szMask, out PatternData d);
            int end = cbMemory.Length - d.Size;
            int len = cbMemory.Length - 32;

            Vector128<byte> firstByte = Vector128.Create(cbPattern[0]);
            ref Vector128<byte> pFirstByte = ref firstByte;
            ref Vector128<byte> needleVec = ref d.Value[0];
            int firstLen = d.Length[0];
            int size = d.Size;
            int skip = d.Skip[0];

            ref PatternData pPatterns = ref d;

            for (int i = 0; i < len; i += 32)
            {
                int iP = i;
                ref byte pP = ref Unsafe.Add(ref Data, i);
                Vector256<byte> b = Unsafe.As<byte, Vector256<byte>>(ref pP);

                int f = GetIndexFromVector(ref pFirstByte, ref needleVec, Avx.ExtractVector128(b, 0), firstLen);
                if (f == 16)
                {
                    f += GetIndexFromVector(ref pFirstByte, ref needleVec, Avx.ExtractVector128(b, 1), firstLen);
                    if (f == 32)
                        continue;
                }

            PossibleMatch:;
                iP += f;
                pP = ref Unsafe.Add(ref pP, f);

                if (iP + size < end)
                {
                    for (int j = 0; j < size && j + iP < len; j++)
                    {
                        if (Unsafe.Add(ref Mask, j) == 'x' && Unsafe.Add(ref Signature, j) != Unsafe.Add(ref pP, j))
                            break;

                        if (j + 1 == size)
                            return iP;
                    }
                    goto SecondMatch;
                }

                if (Matches(ref pP, ref pPatterns, skip))
                    return iP;
                return -1;

            SecondMatch:;
                f = GetIndexFromVector(ref pFirstByte, ref needleVec, Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref pP, 1)), firstLen) + 1;
                if (f > 32)
                    continue;
                goto PossibleMatch;
            }

            return -1;
        }

        private static void GeneratePattern(ref byte Signature, string Mask, out PatternData Out)
        {
            int l = Mask.Length;

            Out = new();
            Out.Length = new int[l];
            Out.Skip = new int[l];
            Out.Value = new Vector128<byte>[l];
            Out.Count = 0;

            for (int i = 0; i < l; i++)
            {
                if (Mask[i] == '?')
                    continue;

                int ml = 0, sl = 0;

                for (int j = i; j < l; j++)
                {
                    if (Mask[j] == '?' || sl >= 16)
                        break;
                    sl++;
                }

                for (int j = i + sl; j < l; j++)
                {
                    if (Mask[j] != '?')
                        break;
                    ml++;
                }

                int c = Out.Count;

                Out.Length[c] = sl;
                Out.Skip[c] = sl + ml;
                Out.Value[c] = Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref Signature, i));
                Out.Count++;

                i += sl - 1;
            }

            Array.Resize(ref Out.Length, Out.Count);
            Array.Resize(ref Out.Skip, Out.Count);
            Array.Resize(ref Out.Value, Out.Count);

            Out.Size = l;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Matches(ref byte Data, ref PatternData Patterns, int skip)
        {
            unchecked
            {
                int k = skip;

                for (int i = 0; i < Patterns.Count; i++)
                {
                    int l = Patterns.Length[i];

                    if (GetEqualityIndexFromVector(ref Patterns.Value[i], ref Unsafe.As<byte, Vector128<byte>>(ref Unsafe.Add(ref Data, k)), l) != l)
                        break;

                    if (i + 1 == Patterns.Count)
                        return true;

                    k += Patterns.Skip[i];
                }
            }

            return false;
        }

        /// <summary>
        /// Compares two vectors and returns an index up to which position they equal eachother. Optionally only compare to a specified length.
        /// </summary>
        /// <param name="left">Left vector to compare.</param>
        /// <param name="right">Right vector to compare.</param>
        /// <param name="len">A length indicating the length to compare.</param>
        /// <returns>An index indicating until which the vectors equal.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetEqualityIndexFromVector(ref Vector128<byte> left, ref Vector128<byte> right, int len = 16)
        {
            int cmp = Sse2.MoveMask(Sse2.CompareEqual(left, right));
            for (int j = 0; j < len; j++)
            {
                if (((cmp >> j) & 1) != 1)
                    return j;
            }
            return len;
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
        private static int GetIndexFromVector(ref Vector128<byte> firstByte, ref Vector128<byte> needle, Vector128<byte> haystack, int needleLen = 16)
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
    }
}
