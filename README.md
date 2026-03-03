# Templater

Templater is a simple yet powerful HTML template engine for .NET. It allows you to render HTML by combining templates
with JSON data, supporting variables, loops, and custom value transformations (modificators).

## Architecture

The project follows architecture based on the classic pipeline: **Tokenizer -> Parser -> AST -> Renderer**.

### 1. Tokenizer

The `Tokenizer` performs lexical analysis, scanning the template string and producing a stream of tokens (`Text`,
`Variable`, `BlockStart`, `BlockEnd`). It identifies expressions within `{{ ... }}` and blocks within `{% ... %}`.

### 2. Parser

The `TemplateParser` takes the token stream and builds an **Abstract Syntax Tree (AST)**. It handles nested structures,
such as `for` loops, ensuring that blocks are correctly matched and nested.

### 3. AST (Nodes)

The template is represented as a tree of nodes (inheriting from `BaseNode`):

- `TextNode`: Literal text in the template.
- `VariableNode`: A placeholder for data, supporting path resolution (e.g., `user.name`) and modificators (e.g.,
  `| price`).
- `ForLoopNode`: Represents a `{% for ... in ... %}` block, iterating over JSON arrays.
- `RootNode`: The top-level container for all nodes.

### 4. Renderer and Context

The `TemplateRenderer` traverses the AST and produces the final HTML string.
The `RenderContext` manages:

- **Data Resolution**: Resolves JSON paths from the provided data.
- **Scope Management**: Handles variable scoping within nested loops using a stack-based approach.
- **Modificators**: Provides an extensible registry for value transformations.

### 5. Modificators

Modificators allow for on-the-fly data formatting. Arguments (after `:`) are optional. For example:

- `{{ item.price | price:€ }}`: Formats a number as a currency string with currency symbol.
- `{{ item.description | paragraph:<p>:</p> }}`: Wraps text in `<p>` tags.

The system is designed to be easily extensible with new modificators by implementing the `IModificator` interface.

## Project Structure

- **Templater.Core**: The main library containing the engine, parser, and rendering logic.
- **Templater.Cli**: A command-line interface for rendering templates from files.
- **Templater.Tests**: A comprehensive suite of unit and integration tests.

## Usage (CLI)

```bash
Templater.Cli <templateFilePath> <dataFilePath> <outputFilePath>
```

Example:

```bash
Templater.Cli template.html data.json output.html
```

## Usage (Code)

```csharp
var engine = new TemplaterEngine();
string html = engine.CreateHtml(templateContent, jsonContent);
```
