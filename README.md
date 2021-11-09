# PatternScanBenchmark

A small and simple benchmark for pattern (AOB) scan (findpattern) implementations written in C# inspired by [this great C++ comparison](https://github.com/learn-more/findpattern-bench). Aimed to compare real-world scanning performance without any frills. Currently 13 different implementations from 11 authors are included.

[![PatternScanBenchmark](https://camo.githubusercontent.com/5701cec8603dacb119a5f6e91e2be01b4e7ba0bbaffa6561e09869767af1aebc/68747470733a2f2f692e696d6775722e636f6d2f4f795a444162682e706e67)](#)

## Contributing

Feel free to create a pull request with your own patter scan implementation at any time. Use [PatternScanTemplate.cs](PatternScanBench/Implementations/PatternScanTemplate.cs) as a template for your one while **following the rules**. Afterwards add a `[Benchmark]` block in [Program.cs](PatternScanBench/Program.cs) and create a pull request. Alternatively try to improve an already existing one.

## Rules

* NO `-unsafe` compiler option
* NO DLLs/libraries/WinAPIs (must work cross-platform)
* NO changes to build process
* has to work on 64 bit Windows/Linux
* has to work on **most** modern AMD64 processors
* don't reuse functions from other implementations, keep your class self-contained

## Prerequisites

* .NET Framework 6.0
* 64bit OS
* SSE/AVX support for some implementations

## Building

Use Visual Studio 2022 or VSCode to build

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
