# .NET 8

// * Summary *

BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1485/22H2/2022Update/SunValley2)
13th Gen Intel Core i7-13700F, 1 CPU, 24 logical and 16 physical cores
.NET SDK=8.0.100-preview.3.23178.7
[Host]     : .NET 8.0.0 (8.0.23.17408), X64 RyuJIT AVX2
DefaultJob : .NET 8.0.0 (8.0.23.17408), X64 RyuJIT AVX2


|                  Method |       Mean |   Error |  StdDev |
|------------------------:|-----------:|--------:|--------:|
|                  Static | 2,482.0 us | 7.90 us | 7.39 us |
|          StaticCompiled |   219.9 us | 0.82 us | 0.77 us |
|                Instance | 2,575.7 us | 5.12 us | 4.79 us |
|        InstanceCompiled |   223.3 us | 0.57 us | 0.53 us |
|         SourceGenerator |   173.8 us | 1.61 us | 1.34 us |
| SourceGeneratorCompiled |   172.4 us | 1.34 us | 1.25 us |


# .NET 7

// * Summary *

BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1485/22H2/2022Update/SunValley2)
13th Gen Intel Core i7-13700F, 1 CPU, 24 logical and 16 physical cores
.NET SDK=7.0.202
[Host]     : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2
DefaultJob : .NET 7.0.4 (7.0.423.11508), X64 RyuJIT AVX2


|                  Method |       Mean |    Error |   StdDev |
|------------------------:|-----------:|---------:|---------:|
|                  Static | 2,513.3 us | 19.71 us | 18.43 us |
|          StaticCompiled |   239.2 us |  0.58 us |  0.51 us |
|                Instance | 2,573.7 us | 23.64 us | 22.11 us |
|        InstanceCompiled |   222.7 us |  0.59 us |  0.55 us |
|         SourceGenerator |   184.2 us |  1.52 us |  1.42 us |
| SourceGeneratorCompiled |   184.5 us |  1.11 us |  0.98 us |

# .NET 6

// * Summary *

BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1485/22H2/2022Update/SunValley2)
13th Gen Intel Core i7-13700F, 1 CPU, 24 logical and 16 physical cores
.NET SDK=7.0.202
[Host]     : .NET 6.0.15 (6.0.1523.11507), X64 RyuJIT AVX2
DefaultJob : .NET 6.0.15 (6.0.1523.11507), X64 RyuJIT AVX2


|           Method |     Mean |     Error |    StdDev |
|-----------------:|---------:|----------:|----------:|
|           Static | 2.557 ms | 0.0049 ms | 0.0044 ms |
|   StaticCompiled | 1.404 ms | 0.0109 ms | 0.0102 ms |
|         Instance | 2.180 ms | 0.0092 ms | 0.0086 ms |
| InstanceCompiled | 1.388 ms | 0.0104 ms | 0.0097 ms |

# .NET Framework 4.8.1

// * Summary *

BenchmarkDotNet=v0.13.5, OS=Windows 11 (10.0.22621.1485/22H2/2022Update/SunValley2)
13th Gen Intel Core i7-13700F, 1 CPU, 24 logical and 16 physical cores
[Host]     : .NET Framework 4.8.1 (4.8.9139.0), X64 RyuJIT VectorSize=256
DefaultJob : .NET Framework 4.8.1 (4.8.9139.0), X64 RyuJIT VectorSize=256


|           Method |     Mean |     Error |    StdDev |
|-----------------:|---------:|----------:|----------:|
|           Static | 4.558 ms | 0.0173 ms | 0.0162 ms |
|   StaticCompiled | 2.262 ms | 0.0098 ms | 0.0091 ms |
|         Instance | 4.581 ms | 0.0208 ms | 0.0195 ms |
| InstanceCompiled | 2.283 ms | 0.0060 ms | 0.0056 ms |
