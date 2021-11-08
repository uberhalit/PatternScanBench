using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Security.Cryptography;
using PatternScanBench.Implementations;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using System.Threading;

namespace PatternScanBench
{
    [SimpleJob(RunStrategy.Monitoring, warmupCount: 2, invocationCount: 2, targetCount: 5)]
    public class Benchmark
    {
        /**
         *  Add a benchmark for your implementation here like the following.
         *  If it runs multithreaded add an '(MT)' to the Description.
         *  
            [Benchmark(Description = "implementation by author")]
            public void Implementation()
            {
                PatternScanTemplate.Init(in CbMemory);
                foreach (MemoryPattern pattern in MemoryPatterns)
                {
                    long result = PatternScanTemplate.FindPattern(in CbMemory, in pattern.CbPattern, pattern.SzMask);
                    if (result != pattern.ExpectedAddress)
                        throw new Exception("Pattern not found...");
                }
            }
         */

        [Benchmark(Description = "DistanceMask by h4ppywastaken")]
        public void DistanceMask()
        {
            foreach (MemoryPattern pattern in MemoryPatterns)
            {
                long result = PatternScanDistanceMask.FindPattern(in CbMemory, in pattern.CbPattern, pattern.SzMask);
                if (result != pattern.ExpectedAddress)
                    throw new Exception("Pattern not found...");
            }
        }

        [Benchmark(Description = "SIMD by Forza")]
        public void ForzaSIMD()
        {
            PatternScanForzaSIMD.Init(in CbMemory);
            foreach (MemoryPattern pattern in MemoryPatterns)
            {
                long result = PatternScanForzaSIMD.FindPattern(in CbMemory, in pattern.CbPattern, pattern.SzMask);
                if (result != pattern.ExpectedAddress)
                    throw new Exception("Pattern not found...");
            }
        }

        // SLOW
        //[Benchmark(Description = "Boyer-Moore-Horspool by Forza")]
        //public void ForzaBMH()
        //{
        //    foreach (MemoryPattern pattern in MemoryPatterns)
        //    {
        //        long result = PatternScanForzaBMH.FindPattern(in CbMemory, in pattern.CbPattern, pattern.SzMask);
        //        if (result != pattern.ExpectedAddress)
        //            throw new Exception("Pattern not found...");
        //    }
        //}

        // Can not match 0s, bugged.
        //[Benchmark(Description = "Boyer-Moore-Horspool by mrexodia")]
        //public void ExodiaBMH()
        //{
        //    foreach (MemoryPattern pattern in MemoryPatterns)
        //    {
        //        long result = PatternScanExodiaBMH.FindPattern(in CbMemory, in pattern.CbPattern, pattern.SzMask);
        //        if (result != pattern.ExpectedAddress)
        //            throw new Exception("Pattern not found...");
        //    }
        //}

        [Benchmark(Description = "Exodia by mrexodia")]
        public void Exodia()
        {
            foreach (MemoryPattern pattern in MemoryPatterns)
            {
                long result = PatternScanExodia.FindPattern(in CbMemory, in pattern.CbPattern, pattern.SzMask);
                if (result != pattern.ExpectedAddress)
                    throw new Exception("Pattern not found...");
            }
        }

        [Benchmark(Description = "ALittleBitNaiveFor by DI20ID")]
        public void ALittleBitNaiveFor()
        {
            foreach (MemoryPattern pattern in MemoryPatterns)
            {
                long result = PatternScanALittleBitNaiveFor.FindPattern(in CbMemory, in pattern.CbPattern, pattern.SzMask);
                if (result != pattern.ExpectedAddress)
                    throw new Exception("Pattern not found...");
            }
        }

        [Benchmark(Description = "SIMD by DarthTon")]
        public void DarthTonSIMD()
        {
            PatternScanDarthTonSIMD.Init(in CbMemory);
            foreach (MemoryPattern pattern in MemoryPatterns)
            {
                long result = PatternScanDarthTonSIMD.FindPattern(in CbMemory, in pattern.CbPattern, pattern.SzMask);
                if (result != pattern.ExpectedAddress)
                    throw new Exception("Pattern not found...");
            }
        }
        
        [Benchmark(Description = "SIMD by DarthTon (MT)")]
        public void DarthTonSIMD_MT()
        {
            PatternScanDarthTonSIMD.Init(in CbMemory);
            foreach (MemoryPattern pattern in MemoryPatterns)
            {
                long result = PatternScanDarthTonSIMD.FindPattern_MT(in CbMemory, in pattern.CbPattern, pattern.SzMask);
                if (result != pattern.ExpectedAddress)
                    throw new Exception("Pattern not found...");
            }
        }
        
        [Benchmark(Description = "Boyer-Moore-Horspool by DarthTon")]
        public void DarthTonBMH()
        {
            foreach (MemoryPattern pattern in MemoryPatterns)
            {
                long result = PatternScanDarthTonBMH.FindPattern(in CbMemory, in pattern.CbPattern, pattern.SzMask);
                if (result != pattern.ExpectedAddress)
                    throw new Exception("Pattern not found...");
            }
        }

        [Benchmark(Description = "BytePointerWithJIT by M i c h a e l")]
        public void BytePointerWithJIT()
        {
            foreach (MemoryPattern pattern in MemoryPatterns)
            {
                long result = PatternScanBytePointerWithJIT.FindPattern(in CbMemory, in pattern.CbPattern, pattern.SzMask);
                if (result != pattern.ExpectedAddress)
                    throw new Exception("Pattern not found...");
            }
        }
        
        [Benchmark(Description = "CompareByteArray by fdsasdf")]
        public void CompareByteArray()
        {
            foreach (MemoryPattern pattern in MemoryPatterns)
            {
                long result = PatternScanCompareByteArray.FindPattern(in CbMemory, in pattern.CbPattern, pattern.SzMask);
                if (result != pattern.ExpectedAddress)
                    throw new Exception("Pattern not found...");
            }
        }
        
        //[Benchmark(Description = "LearnMore by learn_more")]
        //public void LearnMore()
        //{
        //    foreach (MemoryPattern pattern in MemoryPatterns)
        //    {
        //        long result = PatternScanLearnMore.FindPattern(in CbMemory, in pattern.CbPattern, pattern.SzMask);
        //        if (result != pattern.ExpectedAddress)
        //            throw new Exception("Pattern not found...");
        //    }
        //}
        
        [Benchmark(Description = "LearnMore v2 by learn_more")]
        public void LearnMoreV2()
        {
            foreach (MemoryPattern pattern in MemoryPatterns)
            {
                long result = PatternScanLearnMore.FindPatternV2(in CbMemory, in pattern.CbPattern, pattern.SzMask);
                if (result != pattern.ExpectedAddress)
                    throw new Exception("Pattern not found...");
            }
        }
        
        [Benchmark(Description = "MemoryDLL by erfg12")]
        public void MemoryDLL()
        {
            foreach (MemoryPattern pattern in MemoryPatterns)
            {
                long result = PatternScanMemoryDLL.FindPattern(in CbMemory, in pattern.CbPattern, pattern.SzMask);
                if (result != pattern.ExpectedAddress)
                    throw new Exception("Pattern not found...");
            }
        }
        
        // BASELINE - Slow
        //[Benchmark(Description = "NaiveFor by uberhalit")]
        //public void NaiveFor()
        //{
        //    foreach (MemoryPattern pattern in MemoryPatterns)
        //    {
        //        long result = PatternScanNaiveFor.FindPattern(in CbMemory, in pattern.CbPattern, pattern.SzMask);
        //        if (result != pattern.ExpectedAddress)
        //            throw new Exception("Pattern not found...");
        //    }
        //}
        
        [Benchmark(Description = "LazySIMD by uberhalit")]
        public void LazySIMD()
        {
            PatternScanLazySIMD.Init(in CbMemory);
            foreach (MemoryPattern pattern in MemoryPatterns)
            {
                long result = PatternScanLazySIMD.FindPattern(in CbMemory, in pattern.CbPattern, pattern.SzMask);
                if (result != pattern.ExpectedAddress)
                    throw new Exception("Pattern not found...");
            }
        }
        
        [Benchmark(Description = "SpanEquals by uberhalit")]
        public void SpanEquals()
        {
            foreach (MemoryPattern pattern in MemoryPatterns)
            {
                long result = PatternScanSpanEquals.FindPattern(in CbMemory, in pattern.CbPattern, pattern.SzMask);
                if (result != pattern.ExpectedAddress)
                    throw new Exception("Pattern not found...");
            }
        }

        /// <summary>
        /// The entire memory to scan patterns in.
        /// </summary>
        private readonly byte[] CbMemory;

        /// <summary>
        /// A memory pattern including its expected location.
        /// </summary>
        private readonly List<MemoryPattern> MemoryPatterns;

        #region SETUP

        public Benchmark()
        {
            // get dump
            string memoryDumpPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\memorydump.dat";
            if (!File.Exists(memoryDumpPath))
            {
                memoryDumpPath = Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "..", "..", ".."), "Memorydump") + @"\memorydump.dat";
                if (!File.Exists(memoryDumpPath))
                    throw new FileNotFoundException("Memory dump not found: " + memoryDumpPath);
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
            this.CbMemory = moduleMemory;

            // generate byte patterns
            this.MemoryPatterns = new List<MemoryPattern>();
            foreach (KeyValuePair<long, string> entry in TARGET_PATTERNS)
            {
                MemoryPattern memoryPattern = new MemoryPattern(entry.Key, entry.Value);
                this.MemoryPatterns.Add(memoryPattern);
            }
        }

        private const string TARGET_HASH = "2D74CAE219085257C97AE14B72E5F0E773D243E0";

        private readonly Dictionary<long, string> TARGET_PATTERNS = new Dictionary<long, string>
        {
            { 0x198A9A, "0F 84 ?? ?? ?? ?? 48 8D 4C 24 20 C6 84 24 ?? ?? ?? ?? 0B 4C 89 A4 24" }, // "blender.exe"+0x198A9A
            { 0x40C9FF, "57 48 81 EC ?? ?? ?? ?? 48 8B 69 ?? 8B 9C 24" }, // "blender.exe"+0x40C9FF
            { 0x913C69, "E3 2E 4D 02" }, // "blender.exe"+0x913C69
            { 0xA4B71F, "75 ?? ?? ?? ?? EB 21 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 48 8B 9C" }, // "blender.exe"+A4B71F
            { 0xD7CA80, "C6 84 24 ?? ?? ?? ?? ?? C6 44 24 ?? 00" }, // "blender.exe"+D7CA80
            { 0xEE40DB, "48 83 BC 24 ?? ?? ?? ?? ?? 74 ?? 48 8B 84 24 ?? ?? ?? ?? 48 83 E8 ?? 48 89 84 24 ?? ?? ?? ?? EB ?? 48 C7 84 24 ?? ?? ?? ?? ?? ?? ?? ?? 48 8B 84 24 ?? ?? ?? ?? 48 89 84 24 ?? ?? ?? ?? B8 06" }, // "blender.exe"+EE40DB
            { 0x193372F, "4C 8D 1D BA 55 4F 00" }, // "blender.exe"+193372F
            { 0x199B12D, "83 ?? ?? ?? ?? ?? ?? ?? ?? ?? 73 ?? 33 ?? 48 8D 54 24 ?? 48 ?? ?? ?? ?? ?? ?? ?? ?? ?? E8 ?? E6 C5" }, // "blender.exe"+199B12D
            { 0x199B12F, "FF ?? ?? ?? ?? ?? ?? ?? 73 ?? 33 ?? 48 8D 54 24 ?? 48 ?? ?? ?? ?? ?? ?? ?? ?? ?? E8 ?? E6 C5 FF 4C" }, // "blender.exe"+199B12F
            // DATA SECTION
            { 0x1E8CC68 , "48 61 ?? ?? ?? 61 79" }, // "blender.exe"+1E8CC68
            // Should never be found, check for memory exceptions
            { -1 , "CF 99 DA DF EA EF FF FF BB BB" }
        };

        #endregion
    }

    internal class MemoryPattern
    {
        /// <summary>
        /// The expected address where the pattern is located at.
        /// </summary>
        internal readonly long ExpectedAddress;

        /// <summary>
        /// A byte representatuon of the pattern, wildcard positions are replaced by 0.
        /// </summary>
        internal readonly byte[] CbPattern;

        /// <summary>
        /// A string that determines how pattern should be matched, 'x' is match, '?' acts as wildcard.
        /// </summary>
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

    class Program
    {
        /// <summary>
        /// Compares C# patterscan implementations.
        /// Uses a memory dump from main module of blender 2.64a 64bit as a target.
        /// </summary>
        /// <remarks>https://download.blender.org/release/Blender2.64/blender-2.64a-release-windows64.zip</remarks>
        internal static void Main(string[] args)
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

            PrintInfo("11 patterns | 10 iterations | 12 implementations");
            Console.Write("To start press ENTER...");
            Console.ReadLine();
            Console.Write("Running ");

            if (IntPtr.Size != 8)
                throw new PlatformNotSupportedException("Supports x64 only");

            Spinner spinner = new();
            spinner.Start();
            
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            
            var config = new ManualConfig();
            config.
                AddColumnProvider().
                AddExporter().
                AddDiagnoser().
                AddAnalyser().
                AddJob().
                AddValidator().
                AddLogger(NullLogger.Instance).
                WithOption(ConfigOptions.JoinSummary | ConfigOptions.DisableLogFile | ConfigOptions.StopOnFirstError, true).
                UnionRule = ConfigUnionRule.AlwaysUseGlobal;
            Summary summary = BenchmarkRunner.Run<Benchmark>(config);
            var ranking = summary.Reports.Where(x => x.Success).OrderBy(x => x.ResultStatistics.Mean);
            
            spinner.Stop();
            Console.SetCursorPosition(Console.CursorLeft - 8, Console.CursorTop);
            Console.WriteLine("         ");
            
            foreach (BenchmarkReport report in ranking)
                PrintResult(report);
            
            Console.WriteLine("");
            Console.WriteLine("finished...");
            Console.SetCursorPosition(0, 0);
            Console.ReadLine();
        }

        /// <summary>
        /// Prints an info message.
        /// </summary>
        /// <param name="msg">The info.</param>
        internal static void PrintInfo(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("[+] ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(msg);
        }

        /// <summary>
        /// Prints a benchmark result.
        /// </summary>
        /// <param name="report">A report of a succseeded BenchmarkReport.</param>
        internal static void PrintResult(BenchmarkReport report)
        {
            string name = report.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo.Replace("'", "");
            if (name.Contains("(MT)"))
                name = name.Replace("(MT)", $"(MT: {Environment.ProcessorCount} Threads)");
            PrintInfo(name);
            Console.Write("\t");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($"Avg: {(int)(report.ResultStatistics.Mean / 1000000)} ms | Med: {(int)(report.ResultStatistics.Median / 1000000)} ms | Dev: {(report.ResultStatistics.StandardDeviation / 1000000):N2} ms");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
        }
    }

    internal class Spinner
    {
        private readonly int Delay;
        //private static readonly string[] Sequence = new string[] { "V", "<", "^", ">" };
        private static readonly string[] Sequence = new string[] { "/", "—", "\\", "|" };
        private int Counter = 0;
        private Timer Timer = null;

        /// <summary>
        /// Initialize a console spinner animation.
        /// </summary>
        /// <param name="delay">The speed in milliseconds.</param>
        internal Spinner(int delay = 100)
        {
            Delay = delay;
        }

        /// <summary>
        /// Start the spinner at current cursor position.
        /// </summary>
        internal void Start()
        {
            if (Timer == null)
            {
                Console.CursorVisible = false;
                Timer = new Timer(TimerCallback, null, 0, Delay);
            }
        }

        /// <summary>
        /// Stops the spinner.
        /// </summary>
        internal void Stop()
        {
            if (Timer != null)
            {
                Timer.Dispose();
                Timer = null;
                Console.Write(' ');
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
                Console.CursorVisible = true;
            }
        }

        private void TimerCallback(Object o)
        {
            Console.Write(Sequence[Counter]);
            Counter++;
            if (Counter > 3)
                Counter = 0;
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        }
    }
}
