using System;
using System.CommandLine;
using System.IO;
using System.Threading.Tasks;
using FunctionalScript.Transpiler;

namespace FunctionalScript.CLI
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand("FunctionalScript to C# transpiler");
            
            var inputFileArgument = new Argument<FileInfo>(
                name: "input",
                description: "The FunctionalScript file to transpile (.f.js)")
            {
                Arity = ArgumentArity.ExactlyOne
            }.ExistingOnly();
            
            var outputOption = new Option<FileInfo?>(
                aliases: new[] { "--output", "-o" },
                description: "The output C# file (defaults to input name with .cs extension)");
            
            var namespaceOption = new Option<string>(
                aliases: new[] { "--namespace", "-n" },
                getDefaultValue: () => "FunctionalScript.Generated",
                description: "The C# namespace for generated code");
            
            var moduleOption = new Option<string>(
                aliases: new[] { "--module", "-m" },
                getDefaultValue: () => "Module",
                description: "The C# class name for the module");
            
            var verboseOption = new Option<bool>(
                aliases: new[] { "--verbose", "-v" },
                getDefaultValue: () => false,
                description: "Enable verbose output");
            
            rootCommand.AddArgument(inputFileArgument);
            rootCommand.AddOption(outputOption);
            rootCommand.AddOption(namespaceOption);
            rootCommand.AddOption(moduleOption);
            rootCommand.AddOption(verboseOption);
            
            rootCommand.SetHandler(async (inputFile, outputFile, ns, module, verbose) =>
            {
                await TranspileFile(inputFile!, outputFile, ns!, module!, verbose);
            },
            inputFileArgument, outputOption, namespaceOption, moduleOption, verboseOption);
            
            return await rootCommand.InvokeAsync(args);
        }
        
        static async Task TranspileFile(FileInfo inputFile, FileInfo? outputFile, string ns, string module, bool verbose)
        {
            try
            {
                if (verbose)
                {
                    Console.WriteLine($"Transpiling {inputFile.FullName}...");
                }
                
                // Determine output file location
                if (outputFile == null)
                {
                    string outputPath;
                    string dirName = Path.GetFileName(Path.GetDirectoryName(inputFile.FullName) ?? "");
                    
                    // Check if file is in test-files directory
                    if (dirName.Equals("test-files", StringComparison.OrdinalIgnoreCase))
                    {
                        // Output to transpiled subdirectory for test-files
                        string transpiledDir = Path.Combine(Path.GetDirectoryName(inputFile.FullName) ?? ".", "transpiled");
                        Directory.CreateDirectory(transpiledDir);
                        outputPath = Path.Combine(transpiledDir, Path.GetFileNameWithoutExtension(inputFile.Name) + ".cs");
                    }
                    else
                    {
                        // Check if this is a test file (starts with "test" or contains ".test.")
                        string fileName = inputFile.Name;
                        bool isInTestOutput = inputFile.DirectoryName?.Contains("test-output") ?? false;
                        
                        if (!isInTestOutput && 
                            (fileName.StartsWith("test", StringComparison.OrdinalIgnoreCase) || 
                             fileName.Contains(".test.", StringComparison.OrdinalIgnoreCase)))
                        {
                            // Output test files to test-output directory (only if not already in test-output)
                            string testOutputDir = Path.Combine(Path.GetDirectoryName(inputFile.FullName) ?? ".", "test-output", "transpiled");
                            Directory.CreateDirectory(testOutputDir);
                            outputPath = Path.Combine(testOutputDir, Path.GetFileNameWithoutExtension(inputFile.Name) + ".cs");
                        }
                        else
                        {
                            // Regular output - same directory as input
                            outputPath = Path.ChangeExtension(inputFile.FullName, ".cs");
                        }
                    }
                    
                    outputFile = new FileInfo(outputPath);
                }
                
                var options = new TranspilerOptions
                {
                    ModuleName = module,
                    OutputNamespace = ns,
                    GenerateComments = true,
                    UseStrictMode = true
                };
                
                var transpiler = new FunctionalScriptTranspiler(options);
                var result = transpiler.TranspileFile(inputFile.FullName);
                
                if (result.Success)
                {
                    await File.WriteAllTextAsync(outputFile.FullName, result.GeneratedCode);
                    Console.WriteLine($"✓ Successfully transpiled to {outputFile.FullName}");
                    
                    if (verbose && result.Warnings.Length > 0)
                    {
                        Console.WriteLine("Warnings:");
                        foreach (var warning in result.Warnings)
                        {
                            Console.WriteLine($"  ⚠ {warning}");
                        }
                    }
                }
                else
                {
                    Console.Error.WriteLine($"✗ Transpilation failed");
                    foreach (var error in result.Errors)
                    {
                        Console.Error.WriteLine($"  Error: {error}");
                    }
                    Environment.Exit(1);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"✗ Unexpected error: {ex.Message}");
                if (verbose)
                {
                    Console.Error.WriteLine(ex.StackTrace);
                }
                Environment.Exit(1);
            }
        }
    }
}
