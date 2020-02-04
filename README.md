# PatternScanBenchmark

A small and simple benchmark for pattern scan (findpattern) implementations written in C# inspired by [this great C++ comparison](https://github.com/learn-more/findpattern-bench). Aimed to compare real-world scanning performance without any frills.

[![PatternScanBenchmark](https://camo.githubusercontent.com/c88ae59cc829b6a44b11f81358a272aeab2661f4/68747470733a2f2f692e696d6775722e636f6d2f6332766a7445442e706e67)](#)

## Usage

Copy [memorydump.dat](PatternScanBench/Memorydump/memorydump.dat) into the same folder as the compiled benchmark and run the application.

## Contributing

Feel free to create a pull request with your own patter scan implementation at any time. Use [PatternScanTemplate.cs](PatternScanBench/Implementations/PatternScanTemplate.cs) as a template for your one while **following the rules**. Afterwards add it as the first element of `PATTERN_SCAN_ALGORITHMS` in [Program.cs](PatternScanBench/Program.cs) and create a pull request. Alternatively you can also try to improve an already existing one.

## Prerequisites

* .NET Framework 4.7.2
* Windows 10 64bit

## Building

Use Visual Studio 2019 or newer to build

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
