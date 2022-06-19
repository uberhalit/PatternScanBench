
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace PatternScanBench.Implementations
{
    /*
     *  1:1 port of `Compiled` non-SIMD implementation from
     *  Reloaded.Memory.SigScan (3.0.1).
     *
     *  Modified to use `Unsafe` class instead of unsafe code/compiler flag
     *  like the other implementations in this repository.
     *
     *  However, must be noted that this compiles down to the same code in the end anyway.
     */

    /// <summary>
    /// Reloaded.Memory.SigScan (3.0.1) by Sewer56
    /// Non-SIMD Variant
    /// </summary>
    internal class PatternScanReloadedMemorySigscan
    {
        internal PatternScanReloadedMemorySigscan() { }

        /// <summary>
        /// Initializes a new 'PatternScanTemplate'.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        internal static void Init(in byte[] cbMemory) { }

        /// <summary>
        /// Returns address of pattern using 'Template' implementation by author. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal static long FindPattern(in byte[] cbMemory, in MemoryPattern cbPattern)
        {
            return FindPatternCompiled(ref Unsafe.AsRef(cbMemory[0]), cbMemory.Length, cbPattern.SourcePattern).Offset;
        }

        /// <summary>
        /// Attempts to find a given pattern inside the memory region this class was created with.
        /// This method generally works better than a simple byte search when the expected offset is bigger than 4096.
        /// </summary>
        /// <param name="data">Address of the data to be scanned.</param>
        /// <param name="dataLength">Length of the data to be scanned.</param>
        /// <param name="pattern">
        ///     The compiled pattern to look for inside the given region.
        /// </param>
        /// <returns>A result indicating an offset (if found) of the pattern.</returns>
#if NETCOREAPP3_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
        public static PatternScanResult FindPatternCompiled(ref byte data, int dataLength, CompiledScanPattern pattern)
        {
            const int numberOfUnrolls = 8;

            int numberOfInstructions = pattern.NumberOfInstructions;
            int lastIndex = dataLength - Math.Max(pattern.Length, Unsafe.SizeOf<nint>()) - numberOfUnrolls;

            if (lastIndex < 0)
                return FindPatternSimple(ref data, dataLength, new SimplePatternScanData(pattern.Pattern));

            // Note: All of this has to be manually inlined otherwise performance suffers, this is a bit ugly though :/
            ref var firstInstruction = ref Unsafe.AsRef(pattern.Instructions[0]);

            ref var dataCurPointer = ref data;
            ref var dataMaxPointer = ref Unsafe.Add(ref data, lastIndex);

            while (Unsafe.IsAddressLessThan(ref dataCurPointer, ref dataMaxPointer))
            {
                if ((Unsafe.As<byte, nuint>(ref dataCurPointer) & firstInstruction.Mask) != firstInstruction.LongValue)
                {
                    if ((Unsafe.As<byte, nuint>(ref Unsafe.Add(ref dataCurPointer, 1)) & firstInstruction.Mask) != firstInstruction.LongValue)
                    {
                        if ((Unsafe.As<byte, nuint>(ref Unsafe.Add(ref dataCurPointer, 2)) & firstInstruction.Mask) != firstInstruction.LongValue)
                        {
                            if ((Unsafe.As<byte, nuint>(ref Unsafe.Add(ref dataCurPointer, 3)) & firstInstruction.Mask) != firstInstruction.LongValue)
                            {
                                if ((Unsafe.As<byte, nuint>(ref Unsafe.Add(ref dataCurPointer, 4)) & firstInstruction.Mask) != firstInstruction.LongValue)
                                {
                                    if ((Unsafe.As<byte, nuint>(ref Unsafe.Add(ref dataCurPointer, 5)) & firstInstruction.Mask) != firstInstruction.LongValue)
                                    {
                                        if ((Unsafe.As<byte, nuint>(ref Unsafe.Add(ref dataCurPointer, 6)) & firstInstruction.Mask) != firstInstruction.LongValue)
                                        {
                                            if ((Unsafe.As<byte, nuint>(ref Unsafe.Add(ref dataCurPointer, 7)) & firstInstruction.Mask) != firstInstruction.LongValue)
                                            {
                                                dataCurPointer = ref Unsafe.Add(ref dataCurPointer, 8);
                                                goto end;
                                            }
                                            else
                                            {
                                                dataCurPointer = ref Unsafe.Add(ref dataCurPointer, 7);
                                            }
                                        }
                                        else
                                        {
                                            dataCurPointer = ref Unsafe.Add(ref dataCurPointer, 6);
                                        }
                                    }
                                    else
                                    {
                                        dataCurPointer = ref Unsafe.Add(ref dataCurPointer, 5);
                                    }
                                }
                                else
                                {
                                    dataCurPointer = ref Unsafe.Add(ref dataCurPointer, 4);
                                }
                            }
                            else
                            {
                                dataCurPointer = ref Unsafe.Add(ref dataCurPointer, 3);
                            }
                        }
                        else
                        {
                            dataCurPointer = ref Unsafe.Add(ref dataCurPointer, 2);
                        }
                    }
                    else
                    {
                        dataCurPointer = ref Unsafe.Add(ref dataCurPointer, 1);
                    }
                }

                if (numberOfInstructions <= 1 || TestRemainingMasks(numberOfInstructions, ref Unsafe.As<byte, nuint>(ref dataCurPointer), ref firstInstruction))
                    return new PatternScanResult((int)(Unsafe.ByteOffset(ref data, ref dataCurPointer)));

                dataCurPointer = ref Unsafe.Add(ref dataCurPointer, 1);
                end:;
            }

            // Check last few bytes in cases pattern was not found and long overflows into possibly unallocated memory.
            return FindPatternSimple(ref Unsafe.Add(ref data, lastIndex), dataLength - lastIndex, pattern.Pattern).AddOffset(lastIndex);

            // PS. This function is a prime example why the `goto` statement is frowned upon.
            // I have to use it here for performance though.
        }

#if NET5_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
        private static bool TestRemainingMasks(int numberOfInstructions, ref nuint currentDataPointer, ref GenericInstruction instructions)
        {
            /* When NumberOfInstructions > 1 */
            currentDataPointer = ref Unsafe.Add(ref currentDataPointer, 1);

            int y = 1;
            do
            { 
                instructions = ref Unsafe.Add(ref instructions, 1);
                var compareValue = currentDataPointer & instructions.Mask;
                if (compareValue != instructions.LongValue)
                    return false;

                currentDataPointer = ref Unsafe.Add(ref currentDataPointer, 1);
                y++;
            }
            while (y < numberOfInstructions);

            return true;
        }

        /// <summary>
        /// Attempts to find a given pattern inside the memory region this class was created with.
        /// This method uses the simple search, which simply iterates over all bytes, reading max 1 byte at once.
        /// This method generally works better when the expected offset is smaller than 4096.
        /// </summary>
        /// <param name="data">Address of the data to be scanned.</param>
        /// <param name="dataLength">Length of the data to be scanned.</param>
        /// <param name="pattern">
        ///     The pattern to look for inside the given region.
        ///     Example: "11 22 33 ?? 55".
        ///     Key: ?? represents a byte that should be ignored, anything else if a hex byte. i.e. 11 represents 0x11, 1F represents 0x1F
        /// </param>
        /// <returns>A result indicating an offset (if found) of the pattern.</returns>
#if NETCOREAPP3_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
        public static PatternScanResult FindPatternSimple(ref byte data, int dataLength, SimplePatternScanData pattern)
        {
            var patternData = pattern.Bytes;
            var patternMask = pattern.Mask;

            int lastIndex = (dataLength - patternMask.Length) + 1;

            ref byte patternDataPtr = ref Unsafe.AsRef(patternData[0]);
            for (int x = 0; x < lastIndex; x++)
            {
                int patternDataOffset = 0;
                int currentIndex = x;

                int y = 0;
                do
                {
                    // Some performance is saved by making the mask a non-string, since a string comparison is a bit more involved with e.g. null checks.
                    if (patternMask[y] == 0x0)
                    {
                        currentIndex += 1;
                        y++;
                        continue;
                    }

                    // Performance: No need to check if Mask is `x`. The only supported wildcard is '?'.
                    if (Unsafe.Add(ref data, currentIndex) != Unsafe.Add(ref patternDataPtr, patternDataOffset))
                        goto loopexit;

                    currentIndex += 1;
                    patternDataOffset += 1;
                    y++;
                }
                while (y < patternMask.Length);

                return new PatternScanResult(x);
                loopexit:;
            }

            return new PatternScanResult(-1);
        }
    }

    /// <summary>
    /// Represents the pattern to be searched by the scanner.
    /// </summary>
    public ref struct CompiledScanPattern
    {
        private const string MaskIgnore = "??";

        /// <summary>
        /// The pattern the instruction set was created from.
        /// </summary>
        public readonly string Pattern;

        /// <summary>
        /// The length of the original given pattern.
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// Contains the functions that will be executed in order to validate a given block of memory to equal
        /// the pattern this class was instantiated with.
        /// </summary>
        internal GenericInstruction[] Instructions;

        /// <summary>
        /// Contains the number of instructions in the <see cref="Instructions"/> object.
        /// </summary>
        internal int NumberOfInstructions;

        /// <summary>
        /// Creates a new pattern scan target given a string representation of a pattern.
        /// </summary>
        /// <param name="stringPattern">
        ///     The pattern to look for inside the given region.
        ///     Example: "11 22 33 ?? 55".
        ///     Key: ?? represents a byte that should be ignored, anything else if a hex byte. i.e. 11 represents 0x11, 1F represents 0x1F.
        /// </param>
        public CompiledScanPattern(string stringPattern)
        {
            Pattern = stringPattern;
            string[] entries = stringPattern.Split(' ');
            Length = entries.Length;

            // Get bytes to make instructions with.
            Instructions = new GenericInstruction[Length];
            NumberOfInstructions = 0;

            // Optimization for short-medium patterns with masks.
            // Check if our pattern is 1-8 bytes and contains any skips.
            var spanEntries = new Span<string>(entries, 0, entries.Length);
            while (spanEntries.Length > 0)
            {
                int nextSliceLength = Math.Min(Unsafe.SizeOf<nint>(), spanEntries.Length);
                GenerateMaskAndValue(spanEntries.Slice(0, nextSliceLength), out nuint mask, out nuint value);
                AddInstruction(new GenericInstruction(value, mask));
                spanEntries = spanEntries.Slice(nextSliceLength);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddInstruction(GenericInstruction instruction)
        {
            Instructions[NumberOfInstructions] = instruction;
            NumberOfInstructions++;
        }

        /// <summary>
        /// Generates a mask given a pattern between size 0-8.
        /// </summary>
        private void GenerateMaskAndValue(Span<string> entries, out nuint mask, out nuint value)
        {
            mask = 0;
            value = 0;
            for (int x = 0; x < entries.Length; x++)
            {
                mask = mask << 8;
                value = value << 8;
                if (entries[x] != MaskIgnore)
                {
                    mask = mask | 0xFF;
                    value = value | byte.Parse(entries[x], NumberStyles.HexNumber);
                }
            }

            // Reverse order of value.
            if (BitConverter.IsLittleEndian)
            {
                value = (nuint)BinaryPrimitives.ReverseEndianness(value);
                mask = (nuint)BinaryPrimitives.ReverseEndianness(mask);

                // Trim excess zeroes.
                int extraPadding = Unsafe.SizeOf<nuint>() - entries.Length;
                for (int x = 0; x < extraPadding; x++)
                {
                    mask = mask >> 8;
                    value = value >> 8;
                }
            }
        }

        /// <summary>
        /// Implicitly converts a string to a scan pattern.
        /// </summary>
        public static implicit operator CompiledScanPattern(string pattern) => new(pattern);
    }

    /// <summary>
    /// Represents a generic instruction to match an 8 byte masked value at a given address.
    /// </summary>
    public struct GenericInstruction
    {
        /// <summary>
        /// The value to match.
        /// </summary>
        public nuint LongValue;

        /// <summary>
        /// The mask to apply before comparing with the value.
        /// </summary>
        public nuint Mask;

        /// <summary>
        /// Creates an instruction to match an 8 byte masked value at a given address.
        /// </summary>
        /// <param name="longValue">The value to be matched.</param>
        /// <param name="mask">The mask to match.</param>
        public GenericInstruction(nuint longValue, nuint mask)
        {
            LongValue = longValue;
            Mask = mask;
        }
    }

    /// <summary />
    public struct PatternScanResult
    {
        /// <summary>
        /// The offset of the pattern if found, else -1.
        /// </summary>
        public int Offset { get; internal set; }

        /// <summary>
        /// True if the pattern has been found, else false.
        /// </summary>
        public bool Found => Offset != -1;

        /// <summary>
        /// Creates a pattern scan result given the offset of the pattern.
        /// </summary>
        /// <param name="offset">The offset of the pattern if found. -1 if not found.</param>
        public PatternScanResult(int offset)
        {
            Offset = offset;
        }

        /// <summary>
        /// Appends to the existing offset if the offset is valid.
        /// </summary>
#if NETCOREAPP3_0_OR_GREATER
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public PatternScanResult AddOffset(int offset)
        {
            return Offset != -1 ? new PatternScanResult(Offset + offset) : this;
        }

        /* Autogenerated by R# */
        /// <summary/>
        public bool Equals(PatternScanResult other)
        {
            return Offset == other.Offset;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is PatternScanResult other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Offset;
        }
    }

    /// <summary>
    /// [Internal and Test Use]
    /// Represents the pattern to be searched by the scanner.
    /// </summary>
    public ref struct SimplePatternScanData
    {
        private static char[] _maskIgnore = { '?', '?' };
        private static List<byte> _bytes = new(1024);
        private static List<byte> _maskBuilder = new(1024);
        private static object _buildLock = new object();

        /// <summary>
        /// The pattern of bytes to check for.
        /// </summary>
        public byte[] Bytes;

        /// <summary>
        /// The mask string to compare against. `x` represents check while `?` ignores.
        /// Each `x` and `?` represent 1 byte.
        /// </summary>
        public byte[] Mask;

        /// <summary>
        /// The original string from which this pattern was created.
        /// </summary>
        public string Pattern;

        /// <summary>
        /// Creates a new pattern scan target given a string representation of a pattern.
        /// </summary>
        /// <param name="stringPattern">
        ///     The pattern to look for inside the given region.
        ///     Example: "11 22 33 ?? 55".
        ///     Key: ?? represents a byte that should be ignored, anything else if a hex byte. i.e. 11 represents 0x11, 1F represents 0x1F.
        /// </param>
        public SimplePatternScanData(string stringPattern)
        {
            Pattern = stringPattern;
            
            var enumerator       = new SpanSplitEnumerator<char>(stringPattern, ' ');
            var questionMarkFlag = new ReadOnlySpan<char>(_maskIgnore);

            lock (_buildLock)
            {
                _maskBuilder.Clear();
                _bytes.Clear();

                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Equals(questionMarkFlag, StringComparison.Ordinal))
                    {
                        _maskBuilder.Add(0x0);
                    }
                    else
                    {
                        _bytes.Add(byte.Parse(enumerator.Current, NumberStyles.AllowHexSpecifier));
                        _maskBuilder.Add(0x1);
                    }

                }

                Mask = _maskBuilder.ToArray();
                Bytes = _bytes.ToArray();
            }
        }

        /// <summary>
        /// Implicitly converts a string to a scan pattern.
        /// </summary>
        public static implicit operator SimplePatternScanData(string pattern) => new(pattern);
    }

    /// <summary>
    /// Creates a <see cref="SpanSplitEnumerator{TSpanType}"/> that allows for the efficient enumeration of a string
    /// to be split.
    /// </summary>
    /// <typeparam name="TSpanType">The item type held by the span..</typeparam>
    public ref struct SpanSplitEnumerator<TSpanType> where TSpanType : IEquatable<TSpanType>
    {
        /// <summary>
        /// The item to split on.
        /// </summary>
        public TSpanType SplitItem { get; private set; }

        /// <summary>
        /// The current state of the span.
        /// </summary>
        public ReadOnlySpan<TSpanType> Current { get; private set; }

        /// <summary>
        /// The original span this struct was instantiated with.
        /// </summary>
        private ReadOnlySpan<TSpanType> _original;
        private bool _reachedEnd;

        /// <summary>
        /// Moves the span to the next element delimited by the item to split by.
        /// </summary>
        /// <returns>True if the item has moved. False if there is no item to move to.</returns>
        public bool MoveNext()
        {
            var index = _original.IndexOf(SplitItem);
            if (index == -1)
            {
                if (_reachedEnd)
                    return false;

                Current = _original;
                _reachedEnd = true;
                return true;
            }

            // Move to next token.
            Current = _original.Slice(0, index);
            _original = _original.Slice(index + 1);

            return true;
        }

        /// <summary>
        /// Creates an enumerator used to split spans by a specific item.
        /// </summary>
        /// <param name="item">The span to split items within.</param>
        /// <param name="splitItem">The item to split on.</param>
        public SpanSplitEnumerator(ReadOnlySpan<TSpanType> item, TSpanType splitItem)
        {
            _original = item;
            Current = _original;
            SplitItem = splitItem;
            _reachedEnd = false;
        }
    }
}
