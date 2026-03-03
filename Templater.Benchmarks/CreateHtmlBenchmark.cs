using BenchmarkDotNet.Attributes;
using Templater.Core;

namespace Templater.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(1, 0, 50)]
public class CreateHtmlBenchmark
{
    private readonly TemplaterEngine _templater = new();
    private readonly string _template = File.ReadAllText(Path.Combine("data", "paragraph-with-args", "template.html"));
    private readonly string _json = File.ReadAllText(Path.Combine("data", "paragraph-with-args", "data.json"));

    [Benchmark]
    public string CreateHtml()
    {
        return _templater.CreateHtml(_template, _json);
    }
}
