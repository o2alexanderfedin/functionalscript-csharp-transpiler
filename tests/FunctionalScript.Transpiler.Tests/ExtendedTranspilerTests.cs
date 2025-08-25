using Xunit;
using FunctionalScript.Transpiler;

namespace FunctionalScript.Transpiler.Tests
{
    public class ExtendedTranspilerTests
    {
        private string Transpile(string input)
        {
            var transpiler = new FunctionalScriptTranspiler();
            var result = transpiler.Transpile(input);
            if (!result.Success)
                throw new System.Exception($"Transpilation failed: {string.Join(", ", result.Errors)}");
            return result.GeneratedCode;
        }

        [Fact]
        public void TestSimpleModuleExport()
        {
            var input = @"
export default ""text""
";
            var result = Transpile(input);
            Assert.Contains("\"text\"", result);
        }

        [Fact]
        public void TestImportAndReExport()
        {
            var input = @"
import c from ""./m.f.ts""
const a = 1
const b = 2
export default [a, b, c]
";
            var result = Transpile(input);
            Assert.Contains("// import c from \"m.f.ts\"", result);
            Assert.Contains("readonly dynamic a = 1", result);
            Assert.Contains("readonly dynamic b = 2", result);
            Assert.Contains("new dynamic[] { a, b, c }", result);
        }

        [Fact]
        public void TestComplexExpressions()
        {
            var input = @"
const result = (1 + 2) * 3 / 4 - 5
export default result
";
            var result = Transpile(input);
            Assert.Contains("readonly dynamic result = ((((1 + 2) * 3) / 4) - 5)", result);
        }

        [Fact]
        public void TestBitwiseOperations()
        {
            var input = @"
const a = 5 & 3
const b = 5 | 3
const c = 5 ^ 3
const d = 5 << 2
const e = 5 >> 1
const f = 5 >>> 1
export default [a, b, c, d, e, f]
";
            var result = Transpile(input);
            Assert.Contains("(5 & 3)", result);
            Assert.Contains("(5 | 3)", result);
            Assert.Contains("(5 ^ 3)", result);
            Assert.Contains("(5 << 2)", result);
            Assert.Contains("(5 >> 1)", result);
            Assert.Contains("(int)((uint)5 >> 1)", result);
        }

        [Fact]
        public void TestLogicalOperations()
        {
            var input = @"
const a = true && false
const b = true || false
const c = null ?? ""default""
export default [a, b, c]
";
            var result = Transpile(input);
            Assert.Contains("LogicalAnd(true, false)", result);
            Assert.Contains("LogicalOr(true, false)", result);
            Assert.Contains("(null ?? @\"default\")", result);
        }

        [Fact]
        public void TestComparisonOperators()
        {
            var input = @"
const a = 5 === 5
const b = 5 !== 3
const c = 5 < 10
const d = 5 > 3
const e = 5 <= 5
const f = 5 >= 5
export default [a, b, c, d, e, f]
";
            var result = Transpile(input);
            Assert.Contains("StrictEquals(5, 5)", result);
            Assert.Contains("StrictNotEquals(5, 3)", result);
            Assert.Contains("(5 < 10)", result);
            Assert.Contains("(5 > 3)", result);
            Assert.Contains("(5 <= 5)", result);
            Assert.Contains("(5 >= 5)", result);
        }

        [Fact]
        public void TestTernaryOperator()
        {
            var input = @"
const result = true ? ""yes"" : ""no""
export default result
";
            var result = Transpile(input);
            Assert.Contains("(true) ? (@\"yes\") : (@\"no\")", result);
        }

        [Fact]
        public void TestNestedTernary()
        {
            var input = @"
const result = true ? false ? ""a"" : ""b"" : ""c""
export default result
";
            var result = Transpile(input);
            Assert.Contains("(true) ? ((false) ? (@\"a\") : (@\"b\")) : (@\"c\")", result);
        }

        [Fact]
        public void TestExponentialOperator()
        {
            int x;
            var input = @"
const result = 2 ** 3
export default result
";
            var result = Transpile(input);
            Assert.Contains("Math.Pow(2, 3)", result);
        }

        [Fact]
        public void TestUnaryOperators()
        {
            var input = @"
const a = -5
const b = +5
const c = !true
const d = ~5
export default [a, b, c, d]
";
            var result = Transpile(input);
            Assert.Contains("(-5)", result);
            Assert.Contains("(+5)", result);
            Assert.Contains("(!true)", result);
            Assert.Contains("(~5)", result);
        }

        [Fact]
        public void TestComplexObjectLiteral()
        {
            var input = @"
const obj = {
    a: 1,
    b: ""text"",
    c: true,
    d: null,
    e: undefined
}
export default obj
";
            var result = Transpile(input);
            Assert.Contains("{ \"a\", 1 }", result);
            Assert.Contains("{ \"b\", @\"text\" }", result);
            Assert.Contains("{ \"c\", true }", result);
            Assert.Contains("{ \"d\", null }", result);
            Assert.Contains("{ \"e\", FunctionalScript.Runtime.Undefined }", result);
        }

        [Fact]
        public void TestNestedObjects()
        {
            var input = @"
const obj = {
    outer: {
        inner: {
            value: 42
        }
    }
}
export default obj
";
            var result = Transpile(input);
            Assert.Contains("CreateObject", result);
            Assert.Contains("{ \"value\", 42 }", result);
        }

        [Fact]
        public void TestArrayOfObjects()
        {
            var input = @"
const arr = [
    { id: 1, name: ""first"" },
    { id: 2, name: ""second"" }
]
export default arr
";
            var result = Transpile(input);
            Assert.Contains("new dynamic[]", result);
            Assert.Contains("{ \"id\", 1 }", result);
            Assert.Contains("{ \"name\", @\"first\" }", result);
        }

        [Fact]
        public void TestObjectWithComputedProperty()
        {
            var input = @"
const key = ""dynamic""
const obj = {
    [key]: ""value""
}
export default obj
";
            var result = Transpile(input);
            Assert.Contains("{ key, @\"value\" }", result);
        }

        [Fact]
        public void TestObjectShorthandProperty()
        {
            var input = @"
const name = ""test""
const obj = { name }
export default obj
";
            var result = Transpile(input);
            // Shorthand properties should expand to key-value pairs
            Assert.Contains("{ \"name\", name }", result);
        }

        [Fact]
        public void TestMultipleImports()
        {
            var input = @"
import a from ""./a""
import b from ""./b""
import c from ""./c""
const result = [a, b, c]
export default result
";
            var result = Transpile(input);
            Assert.Contains("// import a from \"a\"", result);
            Assert.Contains("// import b from \"b\"", result);
            Assert.Contains("// import c from \"c\"", result);
        }

        [Fact]
        public void TestNamespaceImportUsage()
        {
            var input = @"
import * as utils from ""./utils""
const result = utils
export default result
";
            var result = Transpile(input);
            Assert.Contains("// import utils from \"utils\"", result);
            Assert.Contains("readonly dynamic result = utils", result);
        }

        [Fact]
        public void TestChainedMemberAccess()
        {
            var input = @"
const obj = { a: { b: { c: 1 } } }
const result = obj.a.b.c
export default result
";
            var result = Transpile(input);
            Assert.Contains("obj.a.b.c", result);
        }

        [Fact]
        public void TestArrayIndexAccess()
        {
            var input = @"
const arr = [1, 2, 3]
const result = arr[1]
export default result
";
            var result = Transpile(input);
            Assert.Contains("arr[1]", result);
        }

        [Fact]
        public void TestMixedMemberAndIndexAccess()
        {
            var input = @"
const data = { items: [{ value: 42 }] }
const result = data.items[0].value
export default result
";
            var result = Transpile(input);
            Assert.Contains("data.items[0].value", result);
        }

        [Fact]
        public void TestSpreadOperatorInArray()
        {
            var input = @"
const arr1 = [1, 2]
const arr2 = [3, 4]
const result = [...arr1, ...arr2]
export default result
";
            var result = Transpile(input);
            // Note: Spread operator might need special handling
            Assert.Contains("new dynamic[]", result);
        }

        [Fact]
        public void TestSpreadOperatorInObject()
        {
            var input = @"
const obj1 = { a: 1 }
const obj2 = { b: 2 }
const result = { ...obj1, ...obj2 }
export default result
";
            var result = Transpile(input);
            Assert.Contains("CreateObject", result);
        }

        [Fact]
        public void TestFromAndAsAsIdentifiers()
        {
            var input = @"
const from = ""from value""
const as = ""as value""
export default { from: from, as: as }
";
            var result = Transpile(input);
            Assert.Contains("readonly dynamic from = @\"from value\"", result);
            Assert.Contains("readonly dynamic as = @\"as value\"", result);
        }

        [Fact]
        public void TestBlockComments()
        {
            var input = @"
/* This is a block comment */
const a = 1
/* Another 
   multiline
   comment */
const b = 2
export default [a, b]
";
            var result = Transpile(input);
            Assert.Contains("readonly dynamic a = 1", result);
            Assert.Contains("readonly dynamic b = 2", result);
        }

        [Fact]
        public void TestEmptyArrayAndObject()
        {
            var input = @"
const arr = []
const obj = {}
export default { arr: arr, obj: obj }
";
            var result = Transpile(input);
            Assert.Contains("new dynamic[] {  }", result);
            Assert.Contains("new ExpandoObject()", result);
        }

        [Fact]
        public void TestTrailingCommas()
        {
            var input = @"
const arr = [1, 2, 3,]
const obj = { a: 1, b: 2, }
export default { arr, obj }
";
            var result = Transpile(input);
            Assert.Contains("new dynamic[] { 1, 2, 3 }", result);
            Assert.Contains("{ \"a\", 1 }", result);
            Assert.Contains("{ \"b\", 2 }", result);
        }
    }
}