# PatternScanBenchmark

A small and simple benchmark for pattern scan (findpattern) implementations written in C# inspired by [this great C++ comparison](https://github.com/learn-more/findpattern-bench). Aimed to compare real-world scanning performance without any frills. Real memory dump, every pattern gets scanned just once so that JIT isn't silently optimizing some implementations between runs.

[![PatternScanBenchmark](https://camo.githubusercontent.com/3172104a0354659f913dbcb47271fddf9b19a85e/68747470733a2f2f692e696d6775722e636f6d2f6143706b7048442e706e67)](#)

## Usage

Copy [memorydump.dat](PatternScanBench/Memorydump/memorydump.dat) into the same folder as the compiled benchmark and run the application.

## Contributing

Feel free to create a pull request with your own patter scan implementation at any time. Use [PatternScanTemplate.cs](PatternScanBench/Implementations/PatternScanTemplate.cs) as a template for your one, afterwards add it as the first element of `PATTERN_SCAN_ALGORITHMS` in [Program.cs](PatternScanBench/Program.cs) and create a pull request. Alternatively you can also try to improve an already existing one.

## Prerequisites

* .NET Framework 4.6
* Windows 10 64bit

## Building

Use Visual Studio 2017 or newer to build

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
