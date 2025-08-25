using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;

namespace FunctionalScript.Transpiler
{
    public class CodeGenerator
    {
        private StringBuilder output = new StringBuilder();
        private Dictionary<string, string> imports = new Dictionary<string, string>();
        private Dictionary<string, object> constants = new Dictionary<string, object>();
        private Stack<StringBuilder> exprStack = new Stack<StringBuilder>();
        private int tempVarCounter = 0;
        private string moduleName = "Module";
        
        public string GetOutput() => output.ToString();
        
        public void AddImport(string name, string path)
        {
            imports[name] = path.Replace(".f.js", "").Replace("./", "").Replace("/", ".");
        }
        
        public void AddConstant(string name, string value)
        {
            output.AppendLine($"        public static readonly dynamic {name} = {value};");
        }
        
        public void SetExportDefault(string expr)
        {
            output.AppendLine($"        public static dynamic Default => {expr};");
        }
        
        public void StartModule()
        {
            output.AppendLine("using System;");
            output.AppendLine("using System.Collections.Generic;");
            output.AppendLine("using System.Numerics;");
            output.AppendLine("using System.Linq;");
            output.AppendLine("using System.Dynamic;");
            output.AppendLine();
            output.AppendLine($"namespace FunctionalScript.Generated {{");
            output.AppendLine($"    public static class {moduleName} {{");
        }
        
        public void EndModule()
        {
            output.AppendLine("    }");
            output.AppendLine("}");
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