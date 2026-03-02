using Templater.Core;
using Xunit;

namespace Templater.Tests;

public class TemplaterEngineTests
{
    [Theory]
    [InlineData("test1")]
    [InlineData("test2")]
    [InlineData("modificator-with-args")]
    public void CreateHtml_Test(string dataSet)
    {
        var (json, templateHtml, expectedHtml) = GetTestData(dataSet);
        var templateEngine = new TemplaterEngine();
        var result = templateEngine.CreateHtml(templateHtml, json);
        Assert.Equal(expectedHtml, result);
    }

    private const string DataBasePath = "data";

    private static (string data, string template, string expected) GetTestData(string dataSet)
    {
        return (
            File.ReadAllText($"{DataBasePath}/{dataSet}/data.json"),
            File.ReadAllText($"{DataBasePath}/{dataSet}/template.html"),
            File.ReadAllText($"{DataBasePath}/{dataSet}/result.html")
        );
    }
}
