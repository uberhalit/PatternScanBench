
namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'MemoryDLL' - by erfg12
    /// https://github.com/erfg12
    /// https://github.com/erfg12/memory.dll/blob/master/Memory/memory.cs
    /// </summary>
    internal class PatternScanMemoryDLL
    {
        /// <summary>
        /// Returns address of pattern using 'MemoryDLL' implementation by erfg12. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal static long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            int foundIndex = -1;

            if (cbMemory.Length <= 0 || cbPattern.Length <= 0 || cbPattern.Length > cbMemory.Length) return foundIndex;

            byte[] masks = GenerateAobMask(szMask);

            for (int index = 0; index <= cbMemory.Length - cbPattern.Length; index++)
            {
                if (((cbMemory[index] & masks[0]) == (cbPattern[0] & masks[0])))
                {
                    var match = true;
                    for (int index2 = 1; index2 <= cbPattern.Length - 1; index2++)
                    {
                        if ((cbMemory[index + index2] & masks[index2]) == (cbPattern[index2] & masks[index2])) continue;
                        match = false;
                        break;

                    }

                    if (!match) continue;

                    foundIndex = index;
                    break;
                }
            }

            return foundIndex;
        }

        private static byte[] GenerateAobMask(string stringMask)
        {
            byte[] mask = new byte[stringMask.Length];

            for (var i = 0; i < stringMask.Length; i++)
            {
                char ba = stringMask[i];

                if (ba == '?')
                    mask[i] = 0x00;
                else
                    mask[i] = 0xFF;
            }

            return mask;
        }
    }
}
