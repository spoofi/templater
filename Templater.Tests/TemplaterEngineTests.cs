using Templater.Core;
using Xunit;

namespace Templater.Tests;

public class TemplaterEngineTests
{
    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    public void CreateHtml_Test(string number)
    {
        var (json, templateHtml, expectedHtml) = GetTestData($"test{number}");
        var templateEngine = new TemplaterEngine();
        var result = templateEngine.CreateHtml(templateHtml, json);
        Assert.Equal(expectedHtml, result);
    }

    private const string DataBasePath = "data";

    private static (string data, string template, string expected) GetTestData(string testName)
    {
        return (
            File.ReadAllText($"{DataBasePath}/{testName}/data.json"),
            File.ReadAllText($"{DataBasePath}/{testName}/template.html"),
            File.ReadAllText($"{DataBasePath}/{testName}/result.html")
        );
    }
}
