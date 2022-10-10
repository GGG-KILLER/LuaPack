using Loretta.CodeAnalysis;

namespace LuaPack.Core;

internal static class Diagnostics
{
    public static readonly DiagnosticDescriptor FileOutOfWorkspace = new(
        "PACK0001",
        "File is outside of the workspace",
        "The file being imported through path '{0}' is outside of the workspace. If you wish to import that file, move the workspace file to a folder that you can access it from",
        "LuaPack",
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { WellKnownDiagnosticTags.NotConfigurable });

    public static readonly DiagnosticDescriptor UnresolvedReference = new(
        "PACK0002",
        "Unresolved file",
        "Could not find the specified file inside the workspace",
        "LuaPack",
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { WellKnownDiagnosticTags.NotConfigurable });

    public static readonly DiagnosticDescriptor EntryPointOutOfWorkspace = new(
        "PACK0003",
        "The entry point file is outside of the workspace",
        "The entry point file is outside of the workspace. Move the workspace file to a folder that it can be accessed from",
        "LuaPack",
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { WellKnownDiagnosticTags.NotConfigurable });

    public static readonly DiagnosticDescriptor UnresolvedEntryPoint = new(
        "PACK0004",
        "Entry point could not be resolved file",
        "The entry point file could not be found inside the workspace",
        "LuaPack",
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { WellKnownDiagnosticTags.NotConfigurable });
}
