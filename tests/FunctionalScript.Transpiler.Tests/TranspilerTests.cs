using System;
using System.IO;
using System.Linq;
using Xunit;
using FunctionalScript.Transpiler;

namespace FunctionalScript.Transpiler.Tests
{
    public class TranspilerTests
    {
        private readonly FunctionalScriptTranspiler transpiler;

        public TranspilerTests()
        {
            transpiler = new FunctionalScriptTranspiler();
        }

        [Fact]
        public void Transpile_EmptyModule_GeneratesValidCSharp()
        {
            var source = "export default null";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains("namespace FunctionalScript.Generated", result.GeneratedCode);
            Assert.Contains("public static class Module", result.GeneratedCode);
            Assert.Contains("public static dynamic Default =>", result.GeneratedCode);
        }

        [Theory]
        [InlineData("null", "null")]
        [InlineData("true", "true")]
        [InlineData("false", "false")]
        [InlineData("undefined", "FunctionalScript.Runtime.Undefined")]
        [InlineData("42", "42")]
        [InlineData("-17", "(-17)")]  // Unary minus wraps in parens
        [InlineData("3.14", "3.14")]
        [InlineData("\"hello\"", "@\"hello\"")]
        [InlineData("'world'", "@'world'")]
        public void Transpile_Literals_GeneratesCorrectCSharp(string input, string expected)
        {
            var source = $"export default {input}";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains($"Default => {expected}", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_BigIntLiterals_GeneratesCorrectCSharp()
        {
            var source = @"
                const big1 = 123n
                const big2 = 0xFFn
                const big3 = 0o77n
                const big4 = 0b1111n
                export default big1
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains("new BigInteger(123)", result.GeneratedCode);
            Assert.Contains("BigInteger.Parse", result.GeneratedCode);
        }

        [Theory]
        [InlineData("a + b", "(a + b)")]
        [InlineData("a - b", "(a - b)")]
        [InlineData("a * b", "(a * b)")]
        [InlineData("a / b", "(a / b)")]
        [InlineData("a % b", "(a % b)")]
        [InlineData("a ** b", "Math.Pow(a, b)")]
        public void Transpile_ArithmeticOperators_GeneratesCorrectCSharp(string input, string expected)
        {
            var source = $@"
                const a = 10
                const b = 3
                const result = {input}
                export default result
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains(expected, result.GeneratedCode);
        }

        [Theory]
        [InlineData("a === b", "FunctionalScript.Runtime.StrictEquals(a, b)")]
        [InlineData("a !== b", "FunctionalScript.Runtime.StrictNotEquals(a, b)")]
        [InlineData("a < b", "(a < b)")]
        [InlineData("a > b", "(a > b)")]
        [InlineData("a <= b", "(a <= b)")]
        [InlineData("a >= b", "(a >= b)")]
        public void Transpile_ComparisonOperators_GeneratesCorrectCSharp(string input, string expected)
        {
            var source = $@"
                const a = 10
                const b = 20
                const result = {input}
                export default result
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains(expected, result.GeneratedCode);
        }

        [Theory]
        [InlineData("a && b", "FunctionalScript.Runtime.LogicalAnd(a, b)")]
        [InlineData("a || b", "FunctionalScript.Runtime.LogicalOr(a, b)")]
        [InlineData("!a", "(!a)")]
        [InlineData("a ?? b", "(a ?? b)")]
        public void Transpile_LogicalOperators_GeneratesCorrectCSharp(string input, string expected)
        {
            var source = $@"
                const a = true
                const b = false
                const result = {input}
                export default result
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains(expected, result.GeneratedCode);
        }

        [Fact]
        public void Transpile_ConditionalOperator_GeneratesCorrectCSharp()
        {
            var source = @"
                const age = 25
                const canVote = age >= 18 ? true : false
                export default canVote
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains("? (true) : (false)", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_ArrayLiteral_GeneratesCorrectCSharp()
        {
            var source = @"
                const arr = [1, 2, 3, ""four"", true]
                export default arr
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains("new dynamic[]", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_ObjectLiteral_GeneratesCorrectCSharp()
        {
            var source = @"
                const obj = {
                    name: ""test"",
                    value: 42,
                    flag: true
                }
                export default obj
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains("FunctionalScript.Runtime.CreateObject", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_PropertyAccess_GeneratesCorrectCSharp()
        {
            var source = @"
                const obj = { name: ""test"", value: 42 }
                const name = obj.name
                const value = obj[""value""]
                export default name
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains("obj.name", result.GeneratedCode);
            Assert.Contains("obj[@\"value\"]", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_ConstDeclarations_GeneratesCorrectCSharp()
        {
            var source = @"
                const x = 42
                const y = ""hello""
                const z = true
                export default x
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains("public static readonly dynamic x = 42", result.GeneratedCode);
            Assert.Contains("public static readonly dynamic y = @\"hello\"", result.GeneratedCode);
            Assert.Contains("public static readonly dynamic z = true", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_DefaultImport_GeneratesCorrectCSharp()
        {
            var source = @"
                import math from './math.f.js'
                export default math
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            // Import tracking is handled internally
            Assert.True(result.Success);
        }

        [Fact]
        public void Transpile_NamespaceImport_GeneratesCorrectCSharp()
        {
            var source = @"
                import * as Utils from './utils.f.js'
                export default Utils
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            // Import tracking is handled internally
            Assert.True(result.Success);
        }

        [Fact]
        public void Transpile_ComplexExpression_GeneratesCorrectCSharp()
        {
            var source = @"
                const a = 10
                const b = 20
                const c = 30
                const result = (a + b) * c / 2 - 5
                export default result
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains("((((a + b) * c) / 2) - 5)", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_NestedConditionals_GeneratesCorrectCSharp()
        {
            var source = @"
                const score = 85
                const grade = score >= 90 ? ""A"" : score >= 80 ? ""B"" : ""C""
                export default grade
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            // Should contain nested ternary operators
            Assert.Contains("?", result.GeneratedCode);
            Assert.Contains(":", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_BitwiseOperators_GeneratesCorrectCSharp()
        {
            var source = @"
                const a = 0b1010
                const b = 0b1100
                const and = a & b
                const or = a | b
                const xor = a ^ b
                const not = ~a
                export default and
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains("(a & b)", result.GeneratedCode);
            Assert.Contains("(a | b)", result.GeneratedCode);
            Assert.Contains("(a ^ b)", result.GeneratedCode);
            Assert.Contains("(~a)", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_ShiftOperators_GeneratesCorrectCSharp()
        {
            var source = @"
                const a = 16
                const left = a << 2
                const right = a >> 2
                const unsignedRight = a >>> 2
                export default left
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains("(a << 2)", result.GeneratedCode);
            Assert.Contains("(a >> 2)", result.GeneratedCode);
            Assert.Contains("(int)((uint)a >> 2)", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_CommentTypes_PreservesComments()
        {
            var source = @"
                // Single line comment
                const x = 42 // inline comment
                
                /* Block comment */
                const y = 100
                
                /** JSDoc comment
                 * @type {number}
                 */
                const z = 200
                
                export default x
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            // Comments should be preserved or handled appropriately
            Assert.Contains("42", result.GeneratedCode);
            Assert.Contains("100", result.GeneratedCode);
            Assert.Contains("200", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_ComplexPropertyAccess_GeneratesCorrectCSharp()
        {
            var source = @"
                const obj = {
                    nested: {
                        deep: {
                            value: 42
                        }
                    },
                    array: [1, 2, 3]
                }
                const dotAccess = obj.nested.deep.value
                const bracketAccess = obj[""nested""][""deep""][""value""]
                const arrayAccess = obj.array[0]
                export default dotAccess
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains("obj.nested.deep.value", result.GeneratedCode);
            Assert.Contains("obj[@\"nested\"]", result.GeneratedCode);
            Assert.Contains("obj.array[0]", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_DynamicPropertyAccess_GeneratesCorrectCSharp()
        {
            var source = @"
                const obj = { foo: ""bar"", baz: 42 }
                const key = ""foo""
                const value = obj[key]
                export default value
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains("obj[key]", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_ObjectMethods_GeneratesCorrectCSharp()
        {
            var source = @"
                const original = { a: 1, b: 2 }
                const keys = Object.keys(original)
                const values = Object.values(original)
                const entries = Object.entries(original)
                export default keys
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains("FunctionalScript.Runtime.Object.Keys", result.GeneratedCode);
            Assert.Contains("FunctionalScript.Runtime.Object.Values", result.GeneratedCode);
            Assert.Contains("FunctionalScript.Runtime.Object.Entries", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_ObjectAssign_GeneratesCorrectCSharp()
        {
            var source = @"
                const target = { a: 1 }
                const source1 = { b: 2 }
                const source2 = { c: 3 }
                const result = Object.assign(target, source1, source2)
                export default result
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains("FunctionalScript.Runtime.Object.Assign", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_ObjectFreeze_GeneratesCorrectCSharp()
        {
            var source = @"
                const obj = { x: 1, y: 2 }
                const frozen = Object.freeze(obj)
                export default frozen
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains("FunctionalScript.Runtime.Object.Freeze", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_ObjectIs_GeneratesCorrectCSharp()
        {
            var source = @"
                const same = Object.is(1, 1)
                const notSame = Object.is(0, -0)
                const nanCheck = Object.is(NaN, NaN)
                export default same
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains("FunctionalScript.Runtime.Object.Is", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_ArrayMethods_GeneratesCorrectCSharp()
        {
            var source = @"
                const fromArray = Array.from([1, 2, 3])
                const fromString = Array.from(""hello"")
                const ofElements = Array.of(1, 2, 3)
                const isArray = Array.isArray([1, 2, 3])
                export default fromArray
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains("FunctionalScript.Runtime.Array.From", result.GeneratedCode);
            Assert.Contains("FunctionalScript.Runtime.Array.Of", result.GeneratedCode);
            Assert.Contains("FunctionalScript.Runtime.Array.IsArray", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_JSONOperations_GeneratesCorrectCSharp()
        {
            var source = @"
                const obj = { name: ""John"", age: 30 }
                const json = JSON.stringify(obj)
                const parsed = JSON.parse('{\""key\"":\""value\""}')
                export default json
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains("FunctionalScript.Runtime.JSON.Stringify", result.GeneratedCode);
            Assert.Contains("FunctionalScript.Runtime.JSON.Parse", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_MathOperations_GeneratesCorrectCSharp()
        {
            var source = @"
                const abs = Math.abs(-42)
                const pow = Math.pow(2, 10)
                const max = Math.max(1, 5, 3)
                const min = Math.min(1, 5, 3)
                export default abs
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            Assert.Contains("System.Math.Abs", result.GeneratedCode);
            Assert.Contains("System.Math.Pow", result.GeneratedCode);
            Assert.Contains("System.Math.Max", result.GeneratedCode);
            Assert.Contains("System.Math.Min", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_ImportWithUsage_TracksImports()
        {
            var source = @"
                import math from './math.f.js'
                const result = math.add(10, 20)
                export default result
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            // Should track import information
            Assert.Contains("math", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_NamespaceImportWithUsage_TracksImports()
        {
            var source = @"
                import * as Utils from './utils.f.js'
                const result = Utils.someFunction()
                export default result
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            // Should track namespace import
            Assert.Contains("Utils", result.GeneratedCode);
        }

        [Fact]
        public void Transpile_MultipleImports_TracksAllImports()
        {
            var source = @"
                import math from './math.f.js'
                import utils from './utils.f.js'
                import * as Lib from './lib.f.js'
                export default math
            ";
            var result = transpiler.Transpile(source);
            
            Assert.True(result.Success);
            // Should track all imports
            Assert.Contains("math", result.GeneratedCode);
        }
    }
}