
namespace PatternScanBench.Implementations
{
    /**
        1. Copy this class and replace 'Template' in its name with your implementation name
        2. Replace all 'Template' with your implementation name
        3. If your implementation needs some one-time initialization done add it to Init()
        4. Write your implementation into FindPattern(), must be able to match zeros too
        5. Remove all multi line comment blocks including this one
        6. Add your Implementation to Benchmark class

        * NO unsafe code
        * NO DLLs/libraries (must work cross-platform)
        * NO assembly references outside of default .NET 5 (except Microsofts "extended dotnetframework")
        * NO changes to build process
        * has to work on 64 bit Windows 10
        * has to work on **most** modern AMD64 processors
        * don't reuse functions from other implementations, keep this class self-contained
        * Dynamic SIMD support is available through Microsoft's System.Numerics.Vectors (can be embedded)
     */
    /// <summary>
    /// Pattern scan implementation 'Template' - by author
    /// </summary>
    internal class PatternScanTemplate
    {
        internal PatternScanTemplate() { }

        /// <summary>
        /// Initializes a new 'PatternScanTemplate'.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        internal static void Init(in byte[] cbMemory)
        {
            /**
             * Do whatever you need here
             * Will be called only once, also benchmarked
             */
        }

        /// <summary>
        /// Returns address of pattern using 'Template' implementation by author. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal static long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            /**
             * Add your implementation here
             * Whole function will be benchmarked
             */
            return -1;
        }
    }
}
