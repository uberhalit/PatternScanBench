using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Security.Cryptography;
using PatternScanBench.Implementations;

namespace PatternScanBench
{
    class Program
    {
        /**
            Add your implementation here
         */
        static readonly Dictionary<string, PatternScanAlgorithm> PATTERN_SCAN_ALGORITHMS = new Dictionary<string, PatternScanAlgorithm>
        {
            { "NaiveFor", new PatternScanNaiveFor() }, // by uberhalit
            { "BoyerMooreHorspool", new PatternScanBoyerMooreHorspool() }, // by DarthTon

            #if (!DEBUG)
            { "NaiveLINQ", new PatternScanNaiveLINQ() }, // by lolp1
            #endif
        };

        private const string TARGET_HASH = "2D74CAE219085257C97AE14B72E5F0E773D243E0";
        static readonly Dictionary<long, string> TARGET_PATTERNS = new Dictionary<long, string>
        {
            { 0x198A9A, "0F 84 ?? ?? ?? ?? 48 8D 4C 24 20 C6 84 24 ?? ?? ?? ?? 0B 4C 89 A4 24" }, // "blender.exe"+0x198A9A
            { 0x40C9FF, "57 48 81 EC ?? ?? ?? ?? 48 8B 69 ?? 8B 9C 24" }, // "blender.exe"+0x40C9FF
            { 0x913C69, "E3 2E 4D 02" }, // "blender.exe"+0x913C69
            { 0xA4B71F, "75 ?? ?? ?? ?? EB 21 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 48 8B 9C" }, // "blender.exe"+A4B71F
            { 0xD7CA80, "C6 84 24 ?? ?? ?? ?? ?? C6 44 24 ?? 00" }, // "blender.exe"+D7CA80
            { 0xEE40DB, "48 83 BC 24 ?? ?? ?? ?? ?? 74 ?? 48 8B 84 24 ?? ?? ?? ?? 48 83 E8 ?? 48 89 84 24 ?? ?? ?? ?? EB ?? 48 C7 84 24 ?? ?? ?? ?? ?? ?? ?? ?? 48 8B 84 24 ?? ?? ?? ?? 48 89 84 24 ?? ?? ?? ?? B8 06" }, // "blender.exe"+EE40DB
            { 0x193372F, "4C 8D 1D BA 55 4F 00" }, // "blender.exe"+193372F
            // DATA SECTION
            { 0x1E8CC68 , "48 61 ?? ?? ?? 61 79" }, // "blender.exe"+1E8CC68
            { 0x21A87B8 , "47 4C 53 4C 5F 5F 74 65 63 68 6E 69 71 75 65 5F 5F 70 61 73 73 5F 5F 6D 61 74 65 72 69 61 6C 5F 73 68 69 6E 69 6E 65 73 73" }, // "blender.exe"+21A87B8
            { 0x2572408 , "50 79 45 78 63 5F 52 75 6E 74 69 6D 65 45 72 72 6F 72" } // "blender.exe"+2572408
        };


        /// <summary>
        /// Compares C# patterscan implementations.
        /// Uses a full memory dump from blender 2.64a 64bit as a target.
        /// </summary>
        /// <remarks>https://download.blender.org/release/Blender2.64/blender-2.64a-release-windows64.zip</remarks>
        static void Main(string[] args)
        {
            Console.Title = "Patternscan Benchmark";
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(@"    ____        __  __                ");
            Console.WriteLine(@"   / __ \____ _/ /_/ /____  _________ ");
            Console.WriteLine(@"  / /_/ / __ `/ __/ __/ _ \/ ___/ __ \");
            Console.WriteLine(@" / ____/ /_/ / /_/ /_/  __/ /  / / / /");
            Console.WriteLine(@"/_/    \__,_/\__/\__/\___/_/  /_/ /_/ ");
            Console.WriteLine(@"                                      ");
            Console.WriteLine("            scan benchmark");
            Console.WriteLine("            - C# version -");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("To start press ENTER...");
            Console.ReadLine();
            Console.WriteLine("");

            if (IntPtr.Size != 8)
                throw new PlatformNotSupportedException("Supports x64 only");

            // get dump
            string memoryDumpPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\memorydump.dat";
            if (!File.Exists(memoryDumpPath))
            {
                memoryDumpPath = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "..", "..", ".."), "Memorydump") + @"\memorydump.dat";
                if (!File.Exists(memoryDumpPath))
                    throw new FileNotFoundException("Memory dump not found");
            }

            // read bytes and check hash
            byte[] moduleMemory = File.ReadAllBytes(memoryDumpPath);
            using (SHA1Managed sha1Managed = new SHA1Managed())
            {
                byte[] hash = sha1Managed.ComputeHash(moduleMemory);
                string sha1 = string.Concat(hash.Select(b => b.ToString("x2")));
                if (!sha1.Equals(TARGET_HASH, StringComparison.OrdinalIgnoreCase))
                    throw new BadImageFormatException("Memory dump corrupted");
            }

            // generate byte patterns and masks from string patterns
            List<MemoryPattern> memoryPatterns = new List<MemoryPattern>();
            foreach (KeyValuePair<long, string> entry in TARGET_PATTERNS)
            {
                MemoryPattern memoryPattern = new MemoryPattern(entry.Key, entry.Value);
                memoryPatterns.Add(memoryPattern);
            }

            // bench all algorithms
            Stopwatch stopWatch = new Stopwatch();
            foreach (KeyValuePair<string, PatternScanAlgorithm> patternScanAlgorithm in PATTERN_SCAN_ALGORITHMS)
            {
                PrintInfo(patternScanAlgorithm.Key + " - by " + patternScanAlgorithm.Value.Creator);
                string message = patternScanAlgorithm.Value.Init(in moduleMemory);
                bool algoSuccess = true;
                stopWatch.Restart();
                foreach (MemoryPattern memoryPattern in memoryPatterns)
                {
                    if (patternScanAlgorithm.Value.FindPattern(in moduleMemory, in memoryPattern.CbPattern, memoryPattern.SzMask) == memoryPattern.ExpectedAddress) continue;
                    algoSuccess = false;
                    break;
                }
                stopWatch.Stop();
                if (!algoSuccess)
                    PrintError("failed..." + (message != "" ? " (" + message + ")" : ""), 1);
                else
                    PrintResult(stopWatch.ElapsedMilliseconds + "ms" + (message != "" ? " (" + message + ")" : ""), 1);
            }

            Console.WriteLine("");
            Console.WriteLine("finished...");
            Console.ReadLine();
        }

        /// <summary>
        /// Prints an error message.
        /// </summary>
        /// <param name="msg">The error.</param>
        /// <param name="tabStops">Number of tab stops.</param>
        internal static void PrintError(string msg, int tabStops = 0)
        {
            string prefix = string.Concat(Enumerable.Repeat("\t", tabStops)) + "[!] ";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(prefix);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("ERROR: " + msg);
        }

        /// <summary>
        /// Prints an info message.
        /// </summary>
        /// <param name="msg">The info.</param>
        /// <param name="tabStops">Number of tab stops.</param>
        internal static void PrintInfo(string msg, int tabStops = 0)
        {
            string prefix = string.Concat(Enumerable.Repeat("\t", tabStops)) + "[+] ";
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(prefix);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(msg);
        }

        /// <summary>
        /// Prints an info message.
        /// </summary>
        /// <param name="msg">The info.</param>
        /// <param name="tabStops">Number of tab stops.</param>
        internal static void PrintResult(string msg, int tabStops = 0)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            string prefix = string.Concat(Enumerable.Repeat("\t", tabStops));
            Console.WriteLine(prefix + msg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }

    internal class MemoryPattern
    {
        internal readonly long ExpectedAddress;
        internal readonly byte[] CbPattern;
        internal readonly string SzMask;

        internal MemoryPattern(long expectedAddress, string pattern)
        {
            string[] saPattern = pattern.Split(' ');
            string szMask = "";
            for (int i = 0; i < saPattern.Length; i++)
            {
                if (saPattern[i] == "??")
                {
                    szMask += "?";
                    saPattern[i] = "0";
                }
                else szMask += "x";
            }
            byte[] bPattern = new byte[saPattern.Length];
            for (int i = 0; i < saPattern.Length; i++)
                bPattern[i] = Convert.ToByte(saPattern[i], 0x10);

            ExpectedAddress = expectedAddress;
            SzMask = szMask;
            CbPattern = bPattern;
            if (CbPattern == null || CbPattern.Length == 0)
                throw new ArgumentException("Pattern's length is zero!");
            if (CbPattern.Length != SzMask.Length)
                throw new ArgumentException("Pattern's bytes and szMask must be of the same size!");
        }
    }

    internal abstract class PatternScanAlgorithm
    {
        internal abstract string Creator { get; }

        internal abstract string Init(in byte[] cbMemory);

        internal abstract long FindPattern(in byte[] cbMemory, in byte[] cbPattern, string szMask);
    }
}
