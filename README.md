# LuaPack

This project basically bundles a bunch of Lua files into a single one. Currently, it only recognizes `dofile`, but there are plans to support `require` in the future.

This tool is **beta quality**. The following things are missing:
- [ ] Proper logging (report progress on processing);
- [x] Better diagnostic (errors, warnings, etc.) logging for errors in files;
- [ ] More error checking (check if project file exists, etc.);
- [ ] CI/CD flow to create new releases every commit.

## Installation

Just download it from the releases and place it somewhere in your PATH.

## Usage

### Presets

This tool supports a few Lua versions/flavors/distributions which we call (presets). The preset names are **case-sensitive**.

| Lua Version         | Preset Name       | Description                                                                                                                                                            |
|---------------------|-------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Lua 5.1             | `Lua51`           | PUC Lua 5.1                                                                                                                                                            |
| Lua 5.2             | `Lua52`           | PUC Lua 5.2                                                                                                                                                            |
| Lua 5.3             | `Lua53`           | PUC Lua 5.3                                                                                                                                                            |
| Lua 5.4             | `Lua54`           | PUC Lua 5.4                                                                                                                                                            |
| LuaJIT 2.0          | `LuaJIT20`        | LuaJIT 2.0                                                                                                                                                             |
| LuaJIT 2.1          | `LuaJIT21`        | LuaJIT 2.1                                                                                                                                                             |
| GLua                | `GLua`            | Garry's Mod Lua                                                                                                                                                        |
| FiveM               | `FiveM`           | FiveM Lua                                                                                                                                                              |
| Luau                | `Luau`/`Roblox`   | Luau, aka Roblox Lua                                                                                                                                                   |
| All                 | `All`             | (Not Recommended) Support for most features from above versions (except integers). Should only be used in combination with `presetOverrides`.                          |
| All (With Integers) | `AllWithIntegers` | (Not Recommended) Support for most features from above versions with integers (doesn't support C comments). Should only be used in combination with `presetOverrides`. |

### Project File Reference

The project file is basically a JSON file with a few fields:

| Field             | Value Type             | Description                                                                                                                                  |
|-------------------|------------------------|----------------------------------------------------------------------------------------------------------------------------------------------|
| `preset`          | One of the Lua presets | The Lua preset/flavor/distribution to be used when parsing the Lua files.                                                                    |
| `entryPoint`      | `string`               | The file that is called as the entry point of the script and will be run when the output is run.                                             |
| `outputFile`      | `string`               | The path (relative to the project file) the bundled file will be written to.                                                                 |
| `files`           | glob array             | The list of globs that will be used to bundle files. Like in `.gitignore`, entries prefixed with `!` will be ignored. Order does not matter. |
| `cachedIncludes`  | `boolean`              | *(Optional)* Whether the imports/includes should be cached (only ran once through the entire program). Default is `false`                    |
| `presetOverrides` | Object                 | An object that can override each individual field from [`LuaSyntaxOptions`]. Advanced usage only.                                            |

#### Example file

```json
{
  "preset": "Lua51",
  "entryPoint": "main.lua",
  "outputFile": "dist/main.lua",
  "files": [
    "**/*.lua",
    "!dist/**"
  ],
  "cachedIncludes": true
}
```

### Command Line Reference
```
Description:
  Process the file list according to the project specification.

Usage:
  luapack <path> [options]

Arguments:
  <path>  The path to the json project file.

Options:
  -j, -t, --threads <threads>  The amount of threads to use (if 0 defaults to the amount of cores in the system). [default: 0]
  -q, -s, --quiet, --silent    Whether no console output should be emitted apart from errors in stderr (and errors will be emitted in short form, without highlighting it in the code).
                               [default: False]
  --version                    Show version information
  -?, -h, --help               Show help and usage information
```

#### Success Example
```console
$ luapack example/proj.json
Loading project file...
Listing files to pack...
Loading files...
Checking for errors...
Generating output file...
Done in 169.42ms!
```

#### Error Example
```console
$ luapack example/proj.json
Loading project file...
Listing files to pack...
Loading files...
Checking for errors...
Generating output file...
Errors found:

main.lua(0,10):
error PACK0002: Could not find the specified file inside the workspace
  |
1 | local z = dofile "dep4.lua"
  |           ^^^^^^^^^^^^^^^^^
2 | local y = dofile "dep2.lua"
3 | local x = dofile "dep1.lua"
```

## Example Project

An example project is provided in the [example directory](example/).

The entry point is [`main.lua`](example/main.lua) and the output is in [`dist/main.lua`](example/dist/main.lua).

## FAQ

### 1. Why is the `files` element needed?

This is because we support dynamic imports/includes. So in the case your code does `dofile("mydir/" .. something)` it *might* work.

The reason I say *might* is that we rewrite every import to be relative to the project's root, so this might cause errors if you are importing inside a subdirectory.

### 2. What is Loretta?

[Loretta] is a Lua parser I have created that this program delegates the work of reading and modifying your code to so that in this project we can focus solely on rewriting the code itself without having to worry about supporting many different Lua syntax features and error handling/reporting.

### Have a question?

Open an issue, and I'll try to answer it and if it's a common question I'll add it here!

[Loretta]: https://github.com/LorettaDevs/Loretta
[`LuaSyntaxOptions`]: https://github.com/LorettaDevs/Loretta/blob/e1ae22ba54cb15088859b5b36de379ce3afd0c24/src/Compilers/Lua/Portable/LuaSyntaxOptions.cs#L325-L466
