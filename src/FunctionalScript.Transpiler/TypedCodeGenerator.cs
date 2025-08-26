using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FunctionalScript.Transpiler.TypeInference;

namespace FunctionalScript.Transpiler
{
    /// <summary>
    /// Enhanced code generator that produces strongly-typed C# code
    /// </summary>
    public class TypedCodeGenerator
    {
        private readonly TypeInferenceEngine typeEngine = new TypeInferenceEngine();
        private readonly StringBuilder usings = new StringBuilder();
        private readonly StringBuilder typeDeclarations = new StringBuilder();
        private readonly StringBuilder moduleContent = new StringBuilder();
        private readonly Dictionary<string, string> imports = new Dictionary<string, string>();
        private readonly Stack<(string expr, FSType type)> exprStack = new Stack<(string, FSType)>();
        private int tempVarCounter = 0;
        private int anonymousTypeCounter = 0;
        private string moduleName = "Module";
        private bool useStrongTypes = true;
        
        public TypedCodeGenerator(bool useStrongTypes = true)
        {
            this.useStrongTypes = useStrongTypes;
        }
        
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
            
            // Add namespace
            output.AppendLine();
            output.AppendLine($"namespace FunctionalScript.Generated {{");
            
            // Add type declarations (records for object types)
            if (useStrongTypes && typeDeclarations.Length > 0)
            {
                output.Append(typeDeclarations);
                output.AppendLine();
            }
            
            // Add module class
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
            if (useStrongTypes && exprStack.Count > 0)
            {
                var (_, type) = exprStack.Peek();
                var csType = type.ToCSharpType();
                moduleContent.AppendLine($"        public static readonly {csType} {name} = {value};");
                typeEngine.RegisterSymbol(name, type);
            }
            else
            {
                // Fallback to dynamic
                moduleContent.AppendLine($"        public static readonly dynamic {name} = {value};");
            }
        }
        
        public void SetExportDefault(string expr)
        {
            if (useStrongTypes && exprStack.Count > 0)
            {
                var (_, type) = exprStack.Peek();
                var csType = type.ToCSharpType();
                moduleContent.AppendLine($"        public static {csType} Default => {expr};");
            }
            else
            {
                moduleContent.AppendLine($"        public static dynamic Default => {expr};");
            }
        }
        
        public void StartModule()
        {
            usings.AppendLine("using System;");
            usings.AppendLine("using System.Collections.Generic;");
            usings.AppendLine("using System.Numerics;");
            usings.AppendLine("using System.Linq;");
            if (!useStrongTypes)
            {
                usings.AppendLine("using System.Dynamic;");
            }
        }
        
        public void EndModule()
        {
            // Generate any needed type declarations
            if (useStrongTypes)
            {
                var declarations = typeEngine.GenerateTypeDeclarations();
                if (!string.IsNullOrEmpty(declarations))
                {
                    typeDeclarations.AppendLine(declarations);
                }
            }
        }
        
        public string GetTempVar()
        {
            return $"_temp{tempVarCounter++}";
        }
        
        // Enhanced expression methods with type tracking
        
        public void PushLiteral(string literal)
        {
            var type = typeEngine.InferFromLiteral(literal);
            exprStack.Push((literal, type));
        }
        
        public void PushExpr(string expr)
        {
            // Try to infer type from expression
            FSType type = new PrimitiveType("object");
            
            // Check if it's a known symbol
            if (typeEngine.LookupSymbol(expr) is FSType symbolType)
            {
                type = symbolType;
            }
            
            exprStack.Push((expr, type));
        }
        
        public void PushTypedExpr(string expr, FSType type)
        {
            exprStack.Push((expr, type));
        }
        
        public string PopExpr()
        {
            if (exprStack.Count > 0)
            {
                var (expr, _) = exprStack.Pop();
                return expr;
            }
            return "null";
        }
        
        public (string expr, FSType type) PopTypedExpr()
        {
            if (exprStack.Count > 0)
            {
                return exprStack.Pop();
            }
            return ("null", TypeInferenceEngine.Null);
        }
        
        public string PeekExpr()
        {
            if (exprStack.Count > 0)
            {
                var (expr, _) = exprStack.Peek();
                return expr;
            }
            return "null";
        }
        
        public FSType PeekType()
        {
            if (exprStack.Count > 0)
            {
                var (_, type) = exprStack.Peek();
                return type;
            }
            return TypeInferenceEngine.Null;
        }
        
        // Type-aware operation methods
        
        public void PushBinaryOp(string op, string left, string right)
        {
            if (!useStrongTypes)
            {
                PushExpr($"({left} {op} {right})");
                return;
            }
            
            // Get types from recent pops (this is simplified, real implementation would track better)
            var leftType = new PrimitiveType("object");
            var rightType = new PrimitiveType("object");
            
            var resultType = typeEngine.InferBinaryOp(op, leftType, rightType);
            var binaryExpr = "";
            
            // Special handling for certain operators
            switch (op)
            {
                case "===":
                    binaryExpr = $"object.Equals({left}, {right})";
                    break;
                case "!==":
                    binaryExpr = $"!object.Equals({left}, {right})";
                    break;
                case "??":
                    binaryExpr = $"({left} ?? {right})";
                    break;
                default:
                    binaryExpr = $"({left} {op} {right})";
                    break;
            }
            
            PushTypedExpr(binaryExpr, resultType);
        }
        
        public void PushUnaryOp(string op, string operand)
        {
            if (!useStrongTypes)
            {
                PushExpr($"({op}{operand})");
                return;
            }
            
            var operandType = PeekType();
            var resultType = typeEngine.InferUnaryOp(op, operandType);
            PushTypedExpr($"({op}{operand})", resultType);
        }
        
        public void PushArray(List<string> elements)
        {
            if (!useStrongTypes)
            {
                var arrayExpr = $"new dynamic[] {{ {string.Join(", ", elements)} }}";
                PushExpr(arrayExpr);
                return;
            }
            
            // Infer array type from elements
            var elementTypes = new List<FSType>();
            // In real implementation, we'd track types of elements
            foreach (var _ in elements)
            {
                elementTypes.Add(new PrimitiveType("object"));
            }
            
            var arrayType = typeEngine.InferArrayType(elementTypes);
            var elementType = (arrayType as ArrayType)?.ElementType.ToCSharpType() ?? "object";
            var typedArrayExpr = $"new {elementType}[] {{ {string.Join(", ", elements)} }}";
            
            PushTypedExpr(typedArrayExpr, arrayType);
        }
        
        public void PushObject(Dictionary<string, string> fields)
        {
            if (!useStrongTypes)
            {
                var fieldList = fields.Select(f => $"{{ \"{f.Key}\", {f.Value} }}");
                var objExpr = $"FunctionalScript.Runtime.CreateObject(new Dictionary<string, object> {{ {string.Join(", ", fieldList)} }})";
                PushExpr(objExpr);
                return;
            }
            
            // Create anonymous type or use existing record
            var fieldTypes = new Dictionary<string, FSType>();
            foreach (var field in fields)
            {
                // In real implementation, track actual types
                fieldTypes[field.Key] = new PrimitiveType("object");
            }
            
            var objType = new ObjectType(fieldTypes);
            
            // Generate anonymous type name
            var typeName = $"AnonymousType{anonymousTypeCounter++}";
            objType.TypeName = typeName;
            
            // Add type declaration
            var fieldDecls = fields.Select(f => $"{fieldTypes[f.Key].ToCSharpType()} {f.Key}");
            typeDeclarations.AppendLine($"    public record {typeName}({string.Join(", ", fieldDecls)});");
            
            // Create instance
            var ctorArgs = string.Join(", ", fields.Values);
            var instanceExpr = $"new {typeName}({ctorArgs})";
            
            PushTypedExpr(instanceExpr, objType);
        }
        
        public void PushTernary(string condition, string trueExpr, string falseExpr)
        {
            if (!useStrongTypes)
            {
                PushExpr($"({condition}) ? ({trueExpr}) : ({falseExpr})");
                return;
            }
            
            // In real implementation, track types of branches
            var trueType = new PrimitiveType("object");
            var falseType = new PrimitiveType("object");
            
            var resultType = typeEngine.InferTernary(TypeInferenceEngine.Bool, trueType, falseType);
            var ternaryExpr = $"({condition}) ? ({trueExpr}) : ({falseExpr})";
            
            PushTypedExpr(ternaryExpr, resultType);
        }
    }
}