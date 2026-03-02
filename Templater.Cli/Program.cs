using Templater.Core;

if (args.Length < 3)
{
    Console.WriteLine("Usage: Templater.Cli <templateFilePath> <dataFilePath> <outputFilePath>");
    return;
}

var templatePath = args[0];
var dataPath = args[1];
var outputPath = args[2];

try
{
    if (!File.Exists(templatePath))
    {
        Console.WriteLine($"Error: Template not found at {templatePath}");
        return;
    }

    if (!File.Exists(dataPath))
    {
        Console.WriteLine($"Error: Data not found at {dataPath}");
        return;
    }

    var template = File.ReadAllText(templatePath);
    var json = File.ReadAllText(dataPath);

    var engine = new TemplaterEngine();
    var result = engine.CreateHtml(template, json);

    File.WriteAllText(outputPath, result);
    Console.WriteLine($"Success! Output saved to {outputPath}");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}
