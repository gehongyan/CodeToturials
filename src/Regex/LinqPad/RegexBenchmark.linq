#load "BenchmarkDotNet"

private Regex _regex;
private Regex _compiledRegex;
private const string Pattern = @"(?<=\().*?(?=\))";
private static readonly string Input = string.Concat(Enumerable.Repeat("<p>text</p>", 1000));

void Main()
{
	RunBenchmark(); return;
}

[Benchmark]
public void Static()
{
	_ = Regex.IsMatch(Input, Pattern);
}

[Benchmark]
public void StaticCompiled()
{
	_ = Regex.IsMatch(Input, Pattern, RegexOptions.Compiled);
}

[Benchmark]
public void Instance()
{
	_ = _regex.IsMatch(Input);
}

[Benchmark]
public void InstanceCompiled()
{
	_ = _compiledRegex.IsMatch(Input);
}

[GlobalSetup]
public void BenchmarkSetup()
{
	_regex = new Regex(@"(?<=\().*?(?=\))");
	_compiledRegex = new Regex(@"(?<=\().*?(?=\))", RegexOptions.Compiled);
}
