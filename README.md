# PatternScanBenchmark

A small and simple benchmark for pattern scan (findpattern) implementations written in C# inspired by [this great C++ comparison](https://github.com/learn-more/findpattern-bench). Aimed to compare real-world scanning performance without any frills.

[![PatternScanBenchmark](https://camo.githubusercontent.com/8508cb6dd86f0529aeb049d0978577b8bc7704ab1db5a54ab3c175fe8406d2d6/68747470733a2f2f692e696d6775722e636f6d2f4742364b5854712e706e67)](#)

## Contributing

Feel free to create a pull request with your own patter scan implementation at any time. Use [PatternScanTemplate.cs](PatternScanBench/Implementations/PatternScanTemplate.cs) as a template for your one while **following the rules**. Afterwards add a `[Benchmark]` block in [Program.cs](PatternScanBench/Program.cs) and create a pull request. Alternatively try to improve an already existing one.

## Rules

* NO unsafe code
* NO DLLs/libraries (must work cross-platform)
* NO assembly references outside of default .NET 5 (except Microsofts "extended dotnetframework")
* NO changes to build process
* has to work on 64 bit Windows/Linux
* has to work on **most** modern AMD64 processors
* don't reuse functions from other implementations, keep your class self-contained

## Prerequisites

* .NET Framework 5.0
* 64bit OS

## Building

Use Visual Studio 2019 or VSCode to build

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
