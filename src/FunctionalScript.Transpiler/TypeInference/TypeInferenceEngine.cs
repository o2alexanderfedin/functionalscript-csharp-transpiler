using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace FunctionalScript.Transpiler.TypeInference
{
    /// <summary>
    /// Represents a type in the FunctionalScript type system
    /// </summary>
    public abstract class FSType
    {
        public abstract string ToCSharpType();
        public abstract bool IsCompatibleWith(FSType other);
    }

    /// <summary>
    /// Primitive types (int, double, string, bool)
    /// </summary>
    public class PrimitiveType : FSType
    {
        public string TypeName { get; }
        
        public PrimitiveType(string typeName)
        {
            TypeName = typeName;
        }
        
        public override string ToCSharpType() => TypeName;
        
        public override bool IsCompatibleWith(FSType other)
        {
            if (other is PrimitiveType p)
            {
                // Allow int to double conversion
                if (TypeName == "double" && p.TypeName == "int") return true;
                if (TypeName == "int" && p.TypeName == "double") return true;
                return TypeName == p.TypeName;
            }
            return false;
        }
        
        public override string ToString() => TypeName;
    }

    /// <summary>
    /// Array type with element type
    /// </summary>
    public class ArrayType : FSType
    {
        public FSType ElementType { get; }
        
        public ArrayType(FSType elementType)
        {
            ElementType = elementType;
        }
        
        public override string ToCSharpType() => $"{ElementType.ToCSharpType()}[]";
        
        public override bool IsCompatibleWith(FSType other)
        {
            return other is ArrayType a && ElementType.IsCompatibleWith(a.ElementType);
        }
        
        public override string ToString() => $"Array<{ElementType}>";
    }

    /// <summary>
    /// Object/Record type with named fields
    /// </summary>
    public class ObjectType : FSType
    {
        public Dictionary<string, FSType> Fields { get; }
        public string? TypeName { get; set; }
        
        public ObjectType(Dictionary<string, FSType> fields, string? typeName = null)
        {
            Fields = fields;
            TypeName = typeName;
        }
        
        public override string ToCSharpType()
        {
            if (TypeName != null) return TypeName;
            
            // Generate anonymous type representation
            var fieldList = string.Join(", ", Fields.Select(f => $"{f.Value.ToCSharpType()} {f.Key}"));
            return $"record AutoGenType_{GetHashCode()}({fieldList})";
        }
        
        public override bool IsCompatibleWith(FSType other)
        {
            if (!(other is ObjectType o)) return false;
            
            // Structural typing - check all fields match
            foreach (var field in Fields)
            {
                if (!o.Fields.TryGetValue(field.Key, out var otherFieldType))
                    return false;
                if (!field.Value.IsCompatibleWith(otherFieldType))
                    return false;
            }
            return true;
        }
        
        public override string ToString() => 
            $"{{ {string.Join(", ", Fields.Select(f => $"{f.Key}: {f.Value}"))} }}";
    }

    /// <summary>
    /// Union type for mixed-type scenarios
    /// </summary>
    public class UnionType : FSType
    {
        public List<FSType> Types { get; }
        
        public UnionType(params FSType[] types)
        {
            Types = types.ToList();
        }
        
        public override string ToCSharpType()
        {
            // For now, fall back to object for union types
            // In future, could generate discriminated union classes
            return "object";
        }
        
        public override bool IsCompatibleWith(FSType other)
        {
            if (other is UnionType u)
            {
                // All types in this union must be in the other union
                return Types.All(t => u.Types.Any(ut => t.IsCompatibleWith(ut)));
            }
            // Check if other type is compatible with any type in union
            return Types.Any(t => t.IsCompatibleWith(other));
        }
        
        public override string ToString() => 
            string.Join(" | ", Types.Select(t => t.ToString()));
    }

    /// <summary>
    /// The main type inference engine
    /// </summary>
    public class TypeInferenceEngine
    {
        private readonly Dictionary<string, FSType> symbolTable = new Dictionary<string, FSType>();
        
        // Common type constants
        public static readonly FSType Int = new PrimitiveType("int");
        public static readonly FSType Double = new PrimitiveType("double");
        public static readonly FSType String = new PrimitiveType("string");
        public static readonly FSType Bool = new PrimitiveType("bool");
        public static readonly FSType Null = new PrimitiveType("object?");
        
        /// <summary>
        /// Infer type from a literal value
        /// </summary>
        public FSType InferFromLiteral(string literal)
        {
            // String literals
            if (literal.StartsWith("\"") || literal.StartsWith("'") || literal.StartsWith("@\""))
                return String;
            
            // Boolean literals
            if (literal == "true" || literal == "false")
                return Bool;
            
            // Null/undefined
            if (literal == "null" || literal == "undefined")
                return Null;
            
            // BigInteger
            if (literal.EndsWith("n"))
                return new PrimitiveType("BigInteger");
            
            // Numbers
            if (Regex.IsMatch(literal, @"^-?\d+$"))
                return Int;
            
            if (Regex.IsMatch(literal, @"^-?\d*\.\d+([eE][+-]?\d+)?$"))
                return Double;
            
            // Hex, octal, binary numbers
            if (Regex.IsMatch(literal, @"^0[xXoObB][\da-fA-F]+$"))
                return Int;
            
            // Default to object if unknown
            return new PrimitiveType("object");
        }
        
        /// <summary>
        /// Infer type from array elements
        /// </summary>
        public FSType InferArrayType(List<FSType> elementTypes)
        {
            if (elementTypes.Count == 0)
                return new ArrayType(new PrimitiveType("object"));
            
            // Check if all elements have the same type
            var firstType = elementTypes[0];
            if (elementTypes.All(t => t.IsCompatibleWith(firstType)))
                return new ArrayType(firstType);
            
            // Check for numeric unification (int + double = double)
            if (elementTypes.All(t => t == Int || t == Double))
                return new ArrayType(Double);
            
            // Mixed types - create union type
            var uniqueTypes = elementTypes.Distinct().ToList();
            if (uniqueTypes.Count > 1)
                return new ArrayType(new UnionType(uniqueTypes.ToArray()));
            
            return new ArrayType(firstType);
        }
        
        /// <summary>
        /// Infer type from binary operation
        /// </summary>
        public FSType InferBinaryOp(string op, FSType left, FSType right)
        {
            switch (op)
            {
                // Arithmetic operators
                case "+":
                    // String concatenation
                    if (left == String || right == String)
                        return String;
                    // Numeric addition
                    if ((left == Int || left == Double) && (right == Int || right == Double))
                        return (left == Double || right == Double) ? Double : Int;
                    break;
                    
                case "-":
                case "*":
                case "/":
                case "%":
                    if ((left == Int || left == Double) && (right == Int || right == Double))
                        return (left == Double || right == Double || op == "/") ? Double : Int;
                    break;
                    
                // Comparison operators
                case "<":
                case ">":
                case "<=":
                case ">=":
                case "===":
                case "!==":
                    return Bool;
                    
                // Logical operators
                case "&&":
                case "||":
                    return Bool;
                    
                // Bitwise operators
                case "&":
                case "|":
                case "^":
                case "<<":
                case ">>":
                case ">>>":
                    return Int;
                    
                // Nullish coalescing
                case "??":
                    return right; // Returns right type if left is null
            }
            
            // Default to object for unknown operations
            return new PrimitiveType("object");
        }
        
        /// <summary>
        /// Infer type from unary operation
        /// </summary>
        public FSType InferUnaryOp(string op, FSType operand)
        {
            switch (op)
            {
                case "-":
                case "+":
                    return (operand == Int || operand == Double) ? operand : Double;
                case "!":
                    return Bool;
                case "~":
                    return Int;
                default:
                    return operand;
            }
        }
        
        /// <summary>
        /// Infer type from ternary conditional
        /// </summary>
        public FSType InferTernary(FSType condition, FSType trueType, FSType falseType)
        {
            if (trueType.IsCompatibleWith(falseType))
                return trueType;
            
            // Try numeric unification
            if ((trueType == Int || trueType == Double) && 
                (falseType == Int || falseType == Double))
                return Double;
            
            // Create union type for incompatible branches
            return new UnionType(trueType, falseType);
        }
        
        /// <summary>
        /// Register a symbol with its inferred type
        /// </summary>
        public void RegisterSymbol(string name, FSType type)
        {
            symbolTable[name] = type;
        }
        
        /// <summary>
        /// Look up the type of a symbol
        /// </summary>
        public FSType? LookupSymbol(string name)
        {
            return symbolTable.TryGetValue(name, out var type) ? type : null;
        }
        
        /// <summary>
        /// Generate C# type declarations for all object types
        /// </summary>
        public string GenerateTypeDeclarations()
        {
            var declarations = new List<string>();
            var objectTypes = new HashSet<ObjectType>();
            
            // Collect all object types from symbol table
            foreach (var type in symbolTable.Values)
            {
                CollectObjectTypes(type, objectTypes);
            }
            
            // Generate record declarations
            foreach (var objType in objectTypes.Where(t => t.TypeName != null))
            {
                var fields = string.Join(", ", 
                    objType.Fields.Select(f => $"{f.Value.ToCSharpType()} {f.Key}"));
                declarations.Add($"    public record {objType.TypeName}({fields});");
            }
            
            return string.Join("\n", declarations);
        }
        
        private void CollectObjectTypes(FSType type, HashSet<ObjectType> collected)
        {
            switch (type)
            {
                case ObjectType objType:
                    collected.Add(objType);
                    foreach (var fieldType in objType.Fields.Values)
                        CollectObjectTypes(fieldType, collected);
                    break;
                case ArrayType arrType:
                    CollectObjectTypes(arrType.ElementType, collected);
                    break;
                case UnionType unionType:
                    foreach (var t in unionType.Types)
                        CollectObjectTypes(t, collected);
                    break;
            }
        }
    }
}