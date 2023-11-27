using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

public partial class RegexBenchmarks
{
    private readonly Regex _regex;
    private readonly Regex _compiledRegex;
    private const string Pattern = @"(?<=\().*?(?=\))";
    private static readonly string Input = string.Concat(Enumerable.Repeat("<p>text</p>", 10000));

    public RegexBenchmarks()
    {
        _regex = new Regex(@"(?<=\().*?(?=\))");
        _compiledRegex = new Regex(@"(?<=\().*?(?=\))", RegexOptions.Compiled);
    }

    [Benchmark]
    public void Static() => _ = Regex.IsMatch(Input, Pattern);

    [Benchmark]
    public void StaticCompiled() => _ = Regex.IsMatch(Input, Pattern, RegexOptions.Compiled);

    [Benchmark]
    public void Instance() => _ = _regex.IsMatch(Input);

    [Benchmark]
    public void InstanceCompiled() => _ = _compiledRegex.IsMatch(Input);

    [Benchmark]
    public void SourceGenerator() => _ = TagRegex().IsMatch(Input);

    [Benchmark]
    public void SourceGeneratorCompiled() => _ = TagRegexCompiled().IsMatch(Input);

    [GeneratedRegex("(?<=\\().*?(?=\\))")]
    private static partial Regex TagRegex();

    [GeneratedRegex("(?<=\\().*?(?=\\))", RegexOptions.Compiled)]
    private static partial Regex TagRegexCompiled();
}
