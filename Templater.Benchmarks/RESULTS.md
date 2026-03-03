# Without optimizations

```

BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 10.0.2 (10.0.2, 10.0.225.61305), Arm64 RyuJIT armv8.0-a
  Job-NXLHWJ : .NET 10.0.2 (10.0.2, 10.0.225.61305), Arm64 RyuJIT armv8.0-a

IterationCount=50  LaunchCount=1  WarmupCount=0  

```

| Method     |     Mean |     Error |    StdDev |   Gen0 |   Gen1 | Allocated |
|------------|---------:|----------:|----------:|-------:|-------:|----------:|
| CreateHtml | 6.706 μs | 0.0183 μs | 0.0356 μs | 3.1662 | 0.0687 |  19.41 KB |

---

# With optimizations

```

BenchmarkDotNet v0.15.8, macOS Sequoia 15.7.4 (24G517) [Darwin 24.6.0]
Apple M1 Pro, 1 CPU, 10 logical and 10 physical cores
.NET SDK 10.0.102
  [Host]     : .NET 10.0.2 (10.0.2, 10.0.225.61305), Arm64 RyuJIT armv8.0-a
  Job-NXLHWJ : .NET 10.0.2 (10.0.2, 10.0.225.61305), Arm64 RyuJIT armv8.0-a

IterationCount=50  LaunchCount=1  WarmupCount=0  

```

| Method     |     Mean |     Error |    StdDev |   Gen0 |   Gen1 | Allocated |
|------------|---------:|----------:|----------:|-------:|-------:|----------:|
| CreateHtml | 4.829 μs | 0.0088 μs | 0.0173 μs | 1.9608 | 0.0305 |  12.02 KB |
