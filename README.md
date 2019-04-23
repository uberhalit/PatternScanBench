# PatternScanBenchmark

A small and simple benchmark for pattern scan (findpattern) implementations written in C# inspired by [this great C++ comparison](https://github.com/learn-more/findpattern-bench). Aimed to compare real-world scanning performance without any frills.

[![PatternScanBenchmark](https://camo.githubusercontent.com/b9e0518be0a1615d593dbc06300c6d6517ac862b/68747470733a2f2f692e696d6775722e636f6d2f483077615732772e706e67)](#)

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
