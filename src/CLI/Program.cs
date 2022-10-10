// See https://aka.ms/new-console-template for more information
using System.Collections.Immutable;
using System.CommandLine;
using System.Diagnostics;
using System.Text.Json;
using Loretta.CodeAnalysis.Text;
using LuaPack;
using LuaPack.Core;
using Microsoft.Extensions.FileSystemGlobbing;
using Tsu.Numerics;

var threadsOption = new Option<int>(new[] { "-j", "-t", "--threads" }, () => 0, "The amount of threads to use (if 0 defaults to the amount of cores in the system).");
threadsOption.AddValidator((result) =>
{
    var threads = result.GetValueForOption(threadsOption);
    if (threads < 0)
        result.ErrorMessage = "The number of threads must be equal to or greater than 0.";
});
var pathArgument = new Argument<FileInfo>("path", "The path to the json project file.")
    .LegalFilePathsOnly()
    .ExistingOnly();

var rootCommand = new RootCommand("Process the file list according to the project specification.")
{
    pathArgument,
    threadsOption
};
rootCommand.SetHandler(async (ctx) =>
{
    var path = ctx.ParseResult.GetValueForArgument(pathArgument);
    var threads = ctx.ParseResult.GetValueForOption(threadsOption);
    ctx.ExitCode = await MainCommand(path, threads);
});
await rootCommand.InvokeAsync(args);

static async Task<int> MainCommand(FileInfo projectFileInfo, int threads)
{
    var sw = Stopwatch.StartNew();

    if (threads <= 0)
        threads = Environment.ProcessorCount;

    string? projectRoot = projectFileInfo.DirectoryName!;

    Console.WriteLine("Loading project file...");
    ProjectFile projectFile = await LoadProjectFile(projectFileInfo) ?? throw new InvalidOperationException("Unable to load the project file.");

    Console.WriteLine("Listing files to pack...");
    var matcher = new Matcher();
    matcher.AddIncludePatterns(projectFile.Files.Where(f => !f.StartsWith('!')));
    matcher.AddExcludePatterns(projectFile.Files.Where(f => f.StartsWith('!')).Select(f => f[1..]));
    matcher.AddExclude(projectFile.OutputFile);
    IEnumerable<string> files = matcher.GetResultsInFullPath(projectRoot);

    var generator = new PackGenerator(
        projectRoot,
        projectFile.GetParseOptions(),
        projectFile.EntryPoint,
        projectFile.CachedIncludes);

    Console.WriteLine("Loading files...");
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

    Console.WriteLine("Checking for errors...");
    var diagnostics = generator.GetDiagnostics().ToImmutableArray();
    if (diagnostics.Any())
    {
        Console.WriteLine("Errors found.");
        foreach (var diagnostic in diagnostics)
        {
            await Console.Error.WriteLineAsync(diagnostic.ToString());
        }

        return 1;
    }

    Console.WriteLine("Generating output file...");
    var outputFile = new FileInfo(Path.GetFullPath(projectFile.OutputFile, projectRoot));
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
                await Console.Error.WriteLineAsync(error.ToString());
            }

            return 1;
        }
    }

    sw.Stop();
    var delta = sw.Elapsed.Ticks; // ElapsedTicks returns the raw, os-specific number of ticks.

    Console.WriteLine($"Done in {Duration.Format(delta)}!");

    return 0;
}

static void AddFileToGenerator(PackGenerator generator, string file)
{
    SourceText text;
    using (var stream = File.OpenRead(file))
        text = SourceText.From(stream, throwIfBinaryDetected: true);
    generator.AddFile(file, text);
}

static async Task<ProjectFile?> LoadProjectFile(FileInfo projectFileInfo)
{
    using var stream = projectFileInfo.OpenRead();
    return await JsonSerializer.DeserializeAsync(stream, SourceGenerationContext.Default.ProjectFile);
}
