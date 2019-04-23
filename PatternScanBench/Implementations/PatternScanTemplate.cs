
namespace PatternScanBench.Implementations
{
    /**
        1. Copy this class and replace 'Template' in its name with your implementation name
        2. Replace all 'Template' with your implementation name
        3. Replace all 'author' with your name
        4. If your implementation needs some one-time initialization done add it to Init()
        5. Write your implementation into FindPattern(), must be able to match zeros too
        6. Remove all multi line comment blocks including this one
        * NO DLLs/libraries (invoking native Windows DLLs is allowed)
        * NO assembly references outside of default .NET 4.6 (except from Microsoft themselves as part of their "extended dotnetframework")
        * NO changes to build process
        * NO unsafe code
        * has to work on 64 bit Windows 10
        * has to work on **most** modern AMD64 processors
        * don't reuse functions from other implementations, keep this class self-contained
        * Dynamic SIMD support is available through Microsoft's System.Numerics.Vectors (can be embedded)
     */
    /// <summary>
    /// Pattern scan implementation 'Template' - by author
    /// </summary>
    internal class PatternScanTemplate : PatternScanAlgorithm
    {
        internal override string Creator => "author";
        internal PatternScanTemplate() { }

        /// <summary>
        /// Initializes a new 'PatternScanTemplate'.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <returns>An optional string to display next to benchmark results.</returns>
        internal override string Init(in byte[] cbMemory)
        {
            /**
             * Do whatever you need here
             * Will be called only once, also benchmarked
             */
            return "";
        }

        /// <summary>
        /// Returns address of pattern using 'Template' implementation by author. Can match 0.
        /// </summary>
        /// <param name="cbMemory">The byte array to scan.</param>
        /// <param name="cbPattern">The byte pattern to look for, wildcard positions are replaced by 0.</param>
        /// <param name="szMask">A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.</param>
        /// <returns>-1 if pattern is not found.</returns>
        internal override long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask)
        {
            /**
                Add your implementation here
                Whole function will be benchmarked
             */
            return -1;
        }
    }
}
