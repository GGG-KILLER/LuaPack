using Loretta.CodeAnalysis;
using Loretta.CodeAnalysis.Lua.Syntax;

namespace LuaPack.Core;

internal sealed record Import(Location Location, string Path);
