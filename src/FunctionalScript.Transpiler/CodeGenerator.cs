using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;

namespace FunctionalScript.Transpiler
{
    public class CodeGenerator
    {
        private StringBuilder usings = new StringBuilder();
        private StringBuilder moduleContent = new StringBuilder();
        private Dictionary<string, string> imports = new Dictionary<string, string>();
        private Dictionary<string, object> constants = new Dictionary<string, object>();
        private Stack<StringBuilder> exprStack = new Stack<StringBuilder>();
        private int tempVarCounter = 0;
        private string moduleName = "Module";
        
        public string GetOutput()
        {
            var output = new StringBuilder();
            
            // Add using statements
            output.Append(usings);
            
            // Add imports as comments
            foreach (var import in imports)
            {
                output.AppendLine($"// import {import.Key} from \"{import.Value}\"");
            }
            
            // Add namespace and class
            output.AppendLine();
            output.AppendLine($"namespace FunctionalScript.Generated {{");
            output.AppendLine($"    public static class {moduleName} {{");
            
            // Add module content
            output.Append(moduleContent);
            
            // Close class and namespace
            output.AppendLine("    }");
            output.AppendLine("}");
            
            return output.ToString();
        }
        
        public void AddImport(string name, string path)
        {
            imports[name] = path.Replace(".f.js", "").Replace("./", "").Replace("/", ".");
        }
        
        public void AddConstant(string name, string value)
        {
            moduleContent.AppendLine($"        public static readonly dynamic {name} = {value};");
        }
        
        public void SetExportDefault(string expr)
        {
            moduleContent.AppendLine($"        public static dynamic Default => {expr};");
        }
        
        public void StartModule()
        {
            usings.AppendLine("using System;");
            usings.AppendLine("using System.Collections.Generic;");
            usings.AppendLine("using System.Numerics;");
            usings.AppendLine("using System.Linq;");
            usings.AppendLine("using System.Dynamic;");
        }
        
        public void EndModule()
        {
            // Nothing to do here anymore, GetOutput handles everything
        }
        
        public string GetTempVar()
        {
            return $"_temp{tempVarCounter++}";
        }
        
        public void PushExpr(string expr)
        {
            var sb = new StringBuilder(expr);
            exprStack.Push(sb);
        }
        
        public string PopExpr()
        {
            return exprStack.Count > 0 ? exprStack.Pop().ToString() : "null";
        }
        
        public string PeekExpr()
        {
            return exprStack.Count > 0 ? exprStack.Peek().ToString() : "null";
        }
    }
}