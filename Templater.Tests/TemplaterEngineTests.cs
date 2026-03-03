using Templater.Core;
using Templater.Core.Exceptions;
using Xunit;

namespace Templater.Tests;

public class TemplaterEngineTests
{
    [Theory]
    [InlineData("base")]
    [InlineData("simple-var-root")]
    [InlineData("modificator-with-args")]
    [InlineData("paragraph-with-args")]
    public void CreateHtml_Test(string dataSet)
    {
        var (json, templateHtml, expectedHtml) = GetTestData(dataSet);
        var templateEngine = new TemplaterEngine();
        var result = templateEngine.CreateHtml(templateHtml, json);
        Assert.Equal(expectedHtml, result);
    }

    [Theory]
    [InlineData("syntax-error")]
    public void CreateHtml_SyntaxCheck(string dataSet)
    {
        var (json, templateHtml, _) = GetTestData(dataSet);
        var templateEngine = new TemplaterEngine();

        Assert.Throws<TemplaterInvalidSyntaxException>(() => templateEngine.CreateHtml(templateHtml, json));
    }

    [Theory]
    [InlineData("invalid-modificator")]
    public void CreateHtml_InvalidModificator(string dataSet)
    {
        var (json, templateHtml, _) = GetTestData(dataSet);
        var templateEngine = new TemplaterEngine();
        Assert.Throws<TemplaterInvalidOperationException>(() => templateEngine.CreateHtml(templateHtml, json));
    }

    [Theory]
    [InlineData("render-exception")]
    public void CreateHtml_RenderException(string dataSet)
    {
        var (json, templateHtml, _) = GetTestData(dataSet);
        var templateEngine = new TemplaterEngine();
        Assert.Throws<TemplaterRenderException>(() => templateEngine.CreateHtml(templateHtml, json));
    }

    private const string DataBasePath = "data";

    private static (string data, string template, string expected) GetTestData(string dataSet)
    {
        return (
            File.ReadAllText(Path.Combine(DataBasePath, dataSet, "data.json")),
            File.ReadAllText(Path.Combine(DataBasePath, dataSet, "template.html")),
            File.ReadAllText(Path.Combine(DataBasePath, dataSet, "result.html"))
        );
    }
}
