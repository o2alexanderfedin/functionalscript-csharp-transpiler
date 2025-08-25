using System;
using System.IO;
using System.Text;

namespace FunctionalScript.Transpiler
{
    public class TranspilerOptions
    {
        public string ModuleName { get; set; } = "Module";
        public bool GenerateComments { get; set; } = true;
        public bool UseStrictMode { get; set; } = true;
        public string OutputNamespace { get; set; } = "FunctionalScript.Generated";
    }
    
    public class TranspilerResult
    {
        public bool Success { get; set; }
        public string GeneratedCode { get; set; } = string.Empty;
        public string[] Errors { get; set; } = Array.Empty<string>();
        public string[] Warnings { get; set; } = Array.Empty<string>();
    }
    
    public class FunctionalScriptTranspiler
    {
        private readonly TranspilerOptions options;
        
        public FunctionalScriptTranspiler(TranspilerOptions? options = null)
        {
            this.options = options ?? new TranspilerOptions();
        }
        
        public TranspilerResult Transpile(string sourceCode)
        {
            var result = new TranspilerResult();
            
            try
            {
                using var stream = new MemoryStream(Encoding.UTF8.GetBytes(sourceCode));
                var scanner = new Scanner(stream);
                var parser = new Parser(scanner);
                
                parser.Parse();
                
                if (parser.errors.count == 0)
                {
                    result.Success = true;
                    result.GeneratedCode = parser.gen.GetOutput();
                }
                else
                {
                    result.Success = false;
                    var errors = new List<string>();
                    for (int i = 0; i < parser.errors.count; i++)
                    {
                        errors.Add($"Parse error at line {i}");
                    }
                    result.Errors = errors.ToArray();
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Errors = new[] { ex.Message };
            }
            
            return result;
        }
        
        public TranspilerResult TranspileFile(string inputFile)
        {
            if (!File.Exists(inputFile))
            {
                return new TranspilerResult
                {
                    Success = false,
                    Errors = new[] { $"File not found: {inputFile}" }
                };
            }
            
            string sourceCode = File.ReadAllText(inputFile);
            return Transpile(sourceCode);
        }
        
        private string GeneratePlaceholderCode(string sourceCode)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Dynamic;");
            sb.AppendLine("using System.Numerics;");
            sb.AppendLine();
            sb.AppendLine($"namespace {options.OutputNamespace}");
            sb.AppendLine("{");
            sb.AppendLine($"    public static class {options.ModuleName}");
            sb.AppendLine("    {");
            sb.AppendLine("        // TODO: Generated code from FunctionalScript");
            sb.AppendLine($"        // Source: {sourceCode.Length} characters");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
    }
}