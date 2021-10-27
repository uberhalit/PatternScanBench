# PatternScanBenchmark

A small and simple benchmark for pattern scan (findpattern) implementations written in C# inspired by [this great C++ comparison](https://github.com/learn-more/findpattern-bench). Aimed to compare real-world scanning performance without any frills.

[![PatternScanBenchmark](https://camo.githubusercontent.com/6f03fa57d34457510a6a6141d7c24362ed792da4a2ec05d89cf79a67b5974639/68747470733a2f2f692e696d6775722e636f6d2f763852717458752e706e67)](#)

## Contributing

Feel free to create a pull request with your own patter scan implementation at any time. Use [PatternScanTemplate.cs](PatternScanBench/Implementations/PatternScanTemplate.cs) as a template for your one while **following the rules**. Afterwards add it as the first element of `PATTERN_SCAN_ALGORITHMS` in [Program.cs](PatternScanBench/Program.cs) and create a pull request. Alternatively you can also try to improve an already existing one.

## Prerequisites

* .NET Framework 5.0

## Building

Use Visual Studio 2019 or VSCode to build

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
