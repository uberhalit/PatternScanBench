using System.Runtime.CompilerServices;

namespace PatternScanBench.Implementations
{
    /// <summary>
    /// Pattern scan implementation 'DistanceMask' - by h4ppywastaken
    /// https://github.com/h4ppywastaken
    /// </summary>
    internal class PatternScanDistanceMask
    {
        /// <summary>
        /// Returns address of pattern using 'DistanceMask' implementation by h4ppywastaken. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal static long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            //iteration variables
            long il;
            nint j;

            //get distances between 'x' markers in szMask 
            char[] caSzMask = szMask.ToCharArray();
            ref char pSzMask = ref caSzMask[1];

            nint patternLength = cbPattern.Length;
            nint[] distanceMask = new nint[patternLength];
            ref nint pDistanceMask = ref distanceMask[0];
            nint span = 1;

            for (il = 0; il < patternLength; il++, pSzMask = ref Unsafe.Add(ref pSzMask, 1))
            {
                if(pSzMask == 'x')
                {
                    pDistanceMask = span;
                    pDistanceMask = ref Unsafe.Add(ref pDistanceMask, 1);
                    span = 1;
                }
                else
                {
                    ++span;
                }
            }

            //search for first byte of pattern
            ref byte pMemory = ref cbMemory[0];
            ref byte pPattern = ref cbPattern[0];
            pDistanceMask = ref distanceMask[0];
            long dataLength = cbMemory.Length - patternLength;

            for (il = 0; il < dataLength; ++il, pMemory = ref Unsafe.Add(ref pMemory, 1))
            {
                if(pPattern == pMemory)
                {
                    //check for remaining pattern bytes
                    for (j = pDistanceMask; ; pDistanceMask = ref Unsafe.Add(ref pDistanceMask, 1), j += pDistanceMask)
                    {
                        pPattern = ref Unsafe.Add(ref pPattern, pDistanceMask);
                        pMemory = ref Unsafe.Add(ref pMemory, pDistanceMask);

                        if (pDistanceMask == 0)
                        {
                            return il;
                        }
                        else if (pMemory != pPattern)
                        {
                            pMemory = ref cbMemory[il];
                            pPattern = ref cbPattern[0];
                            pDistanceMask = ref distanceMask[0];
                            break;
                        }
                    }
                }
                
            }

            return -1;
        }
    }
}
