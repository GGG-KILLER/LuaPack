// See https://aka.ms/new-console-template for more information
using System.Collections.Immutable;
using System.Text.Json;
using Cocona;
using Loretta.CodeAnalysis.Text;
using LuaPack;
using LuaPack.Core;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Logging;

Cocona.Builder.CoconaAppBuilder? builder = CoconaApp.CreateBuilder(args);

await CoconaApp.RunAsync(MainCommand, args);

static async Task<int> MainCommand(
    CoconaAppContext ctx,
    ILogger<Program> logger,
    [Argument("path", Description = "The path to the json project file.")] string projectFilePath,
    [Option("threads", new[] { 'j', 't' }, Description = "The amount of threads to use (if 0 defaults to the amount of cores in the system).")] int threads = 0)
{
    if (threads == 0)
        threads = Environment.ProcessorCount;

    projectFilePath = Path.GetFullPath(projectFilePath);
    string? projectDir = Path.GetDirectoryName(projectFilePath)!;

    logger.LogInformation("Loading project file...");
    ProjectFile projectFile = await LoadProjectFile(projectFilePath) ?? throw new InvalidOperationException("Unable to load the project file.");

    var matcher = new Matcher();
    matcher.AddIncludePatterns(projectFile.Files.Where(f => !f.StartsWith('!')));
    matcher.AddExcludePatterns(projectFile.Files.Where(f => f.StartsWith('!')).Select(f => f[1..]));
    matcher.AddExclude(projectFile.OutputFile);
    IEnumerable<string> files = matcher.GetResultsInFullPath(projectDir);

    var generator = new PackGenerator(projectDir, projectFile);

    if (threads == 1)
    {
        foreach (var file in files)
        {
            AddFileToGenerator(generator, file);
        }
    }
    else
    {
        Parallel.ForEach(
            files,
            new ParallelOptions { MaxDegreeOfParallelism = threads },
            file => AddFileToGenerator(generator, file));
    }

    var diagnostics = generator.GetDiagnostics().ToImmutableArray();
    if (diagnostics.Any())
    {
        foreach (var diagnostic in diagnostics)
        {
            logger.LogError("{Message}", diagnostic.ToString());
        }
    }

    var outputFile = new FileInfo(Path.GetFullPath(projectFile.OutputFile, projectDir));
    outputFile.Directory?.Create();

    using (var stream = outputFile.OpenWrite())
    using (var writer = new StreamWriter(stream))
    {
        var result = generator.WriteOutput(writer);
        if (result.IsErr)
        {
            var errors = result.Err.Value;

            foreach (var error in errors)
            {
                logger.LogError("{Message}", error.ToString());
            }

            return 1;
        }
    }

    return 0;
}

static void AddFileToGenerator(PackGenerator generator, string file)
{
    SourceText text;
    using (var stream = File.OpenRead(file))
        text = SourceText.From(stream, throwIfBinaryDetected: true);
    generator.AddFile(file, text);
}

static async Task<ProjectFile?> LoadProjectFile(string path)
{
    using var stream = File.OpenRead(path);
    return await JsonSerializer.DeserializeAsync<ProjectFile>(stream, SourceGenerationContext.Default.ProjectFile);
}
