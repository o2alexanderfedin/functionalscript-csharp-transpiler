using System;
using System.Collections.Generic;
using Xunit;
using FunctionalScript.Transpiler.TypeInference;

namespace FunctionalScript.Transpiler.Tests
{
    public class TypeInferenceTests
    {
        private readonly TypeInferenceEngine engine = new TypeInferenceEngine();

        #region Primitive Type Tests

        [Theory]
        [InlineData("42", "int")]
        [InlineData("0", "int")]
        [InlineData("-17", "int")]
        [InlineData("999999", "int")]
        public void InferFromLiteral_IntegerLiterals_ReturnsIntType(string literal, string expectedType)
        {
            var type = engine.InferFromLiteral(literal);
            
            Assert.IsType<PrimitiveType>(type);
            Assert.Equal(expectedType, ((PrimitiveType)type).TypeName);
        }

        [Theory]
        [InlineData("3.14", "double")]
        [InlineData("0.0", "double")]
        [InlineData("-2.5", "double")]
        [InlineData("1.23e10", "double")]
        [InlineData("1.23E-5", "double")]
        public void InferFromLiteral_FloatingPointLiterals_ReturnsDoubleType(string literal, string expectedType)
        {
            var type = engine.InferFromLiteral(literal);
            
            Assert.IsType<PrimitiveType>(type);
            Assert.Equal(expectedType, ((PrimitiveType)type).TypeName);
        }

        [Theory]
        [InlineData("\"hello\"", "string")]
        [InlineData("'world'", "string")]
        [InlineData("@\"verbatim\"", "string")]
        [InlineData("\"\"", "string")]
        public void InferFromLiteral_StringLiterals_ReturnsStringType(string literal, string expectedType)
        {
            var type = engine.InferFromLiteral(literal);
            
            Assert.IsType<PrimitiveType>(type);
            Assert.Equal(expectedType, ((PrimitiveType)type).TypeName);
        }

        [Theory]
        [InlineData("true", "bool")]
        [InlineData("false", "bool")]
        public void InferFromLiteral_BooleanLiterals_ReturnsBoolType(string literal, string expectedType)
        {
            var type = engine.InferFromLiteral(literal);
            
            Assert.IsType<PrimitiveType>(type);
            Assert.Equal(expectedType, ((PrimitiveType)type).TypeName);
        }

        [Theory]
        [InlineData("null", "object?")]
        [InlineData("undefined", "object?")]
        public void InferFromLiteral_NullishLiterals_ReturnsNullableType(string literal, string expectedType)
        {
            var type = engine.InferFromLiteral(literal);
            
            Assert.IsType<PrimitiveType>(type);
            Assert.Equal(expectedType, ((PrimitiveType)type).TypeName);
        }

        [Theory]
        [InlineData("123n", "BigInteger")]
        [InlineData("999999999999999999999n", "BigInteger")]
        public void InferFromLiteral_BigIntLiterals_ReturnsBigIntegerType(string literal, string expectedType)
        {
            var type = engine.InferFromLiteral(literal);
            
            Assert.IsType<PrimitiveType>(type);
            Assert.Equal(expectedType, ((PrimitiveType)type).TypeName);
        }

        [Theory]
        [InlineData("0xFF", "int")]
        [InlineData("0x123ABC", "int")]
        [InlineData("0o77", "int")]
        [InlineData("0b1010", "int")]
        public void InferFromLiteral_HexOctalBinaryLiterals_ReturnsIntType(string literal, string expectedType)
        {
            var type = engine.InferFromLiteral(literal);
            
            Assert.IsType<PrimitiveType>(type);
            Assert.Equal(expectedType, ((PrimitiveType)type).TypeName);
        }

        #endregion

        #region Array Type Tests

        [Fact]
        public void InferArrayType_EmptyArray_ReturnsObjectArray()
        {
            var elementTypes = new List<FSType>();
            var arrayType = engine.InferArrayType(elementTypes);
            
            Assert.IsType<ArrayType>(arrayType);
            var elementType = ((ArrayType)arrayType).ElementType;
            Assert.IsType<PrimitiveType>(elementType);
            Assert.Equal("object", ((PrimitiveType)elementType).TypeName);
        }

        [Fact]
        public void InferArrayType_HomogeneousIntArray_ReturnsIntArray()
        {
            var elementTypes = new List<FSType>
            {
                TypeInferenceEngine.Int,
                TypeInferenceEngine.Int,
                TypeInferenceEngine.Int
            };
            
            var arrayType = engine.InferArrayType(elementTypes);
            
            Assert.IsType<ArrayType>(arrayType);
            var elementType = ((ArrayType)arrayType).ElementType;
            Assert.Equal(TypeInferenceEngine.Int, elementType);
        }

        [Fact]
        public void InferArrayType_MixedNumberArray_ReturnsDoubleArray()
        {
            var elementTypes = new List<FSType>
            {
                TypeInferenceEngine.Int,
                TypeInferenceEngine.Double,
                TypeInferenceEngine.Int
            };
            
            var arrayType = engine.InferArrayType(elementTypes);
            
            Assert.IsType<ArrayType>(arrayType);
            var elementType = ((ArrayType)arrayType).ElementType;
            
            Assert.Equal(TypeInferenceEngine.Double, elementType);
        }

        [Fact]
        public void InferArrayType_HeterogeneousArray_ReturnsUnionTypeArray()
        {
            var elementTypes = new List<FSType>
            {
                TypeInferenceEngine.String,
                TypeInferenceEngine.Int,
                TypeInferenceEngine.Bool
            };
            
            var arrayType = engine.InferArrayType(elementTypes);
            
            Assert.IsType<ArrayType>(arrayType);
            var elementType = ((ArrayType)arrayType).ElementType;
            Assert.IsType<UnionType>(elementType);
            
            var unionType = (UnionType)elementType;
            Assert.Equal(3, unionType.Types.Count);
        }

        #endregion

        #region Binary Operator Type Tests

        [Theory]
        [InlineData("+", "int", "int", "int")]
        [InlineData("+", "double", "double", "double")]
        [InlineData("+", "int", "double", "double")]
        [InlineData("+", "double", "int", "double")]
        public void InferBinaryOp_ArithmeticAddition_ReturnsCorrectType(string op, string leftType, string rightType, string expectedType)
        {
            var left = new PrimitiveType(leftType);
            var right = new PrimitiveType(rightType);
            
            var result = engine.InferBinaryOp(op, left, right);
            
            Assert.IsType<PrimitiveType>(result);
            Assert.Equal(expectedType, ((PrimitiveType)result).TypeName);
        }

        [Fact]
        public void InferBinaryOp_StringConcatenation_ReturnsString()
        {
            var result = engine.InferBinaryOp("+", TypeInferenceEngine.String, TypeInferenceEngine.Int);
            
            Assert.Equal(TypeInferenceEngine.String, result);
        }

        [Theory]
        [InlineData("-")]
        [InlineData("*")]
        [InlineData("%")]
        public void InferBinaryOp_ArithmeticOperators_ReturnNumericType(string op)
        {
            var result = engine.InferBinaryOp(op, TypeInferenceEngine.Int, TypeInferenceEngine.Int);
            
            Assert.Equal(TypeInferenceEngine.Int, result);
        }

        [Fact]
        public void InferBinaryOp_Division_AlwaysReturnsDouble()
        {
            var result = engine.InferBinaryOp("/", TypeInferenceEngine.Int, TypeInferenceEngine.Int);
            
            Assert.Equal(TypeInferenceEngine.Double, result);
        }

        [Theory]
        [InlineData("<")]
        [InlineData(">")]
        [InlineData("<=")]
        [InlineData(">=")]
        [InlineData("===")]
        [InlineData("!==")]
        public void InferBinaryOp_ComparisonOperators_ReturnBool(string op)
        {
            var result = engine.InferBinaryOp(op, TypeInferenceEngine.Int, TypeInferenceEngine.Int);
            
            Assert.Equal(TypeInferenceEngine.Bool, result);
        }

        [Theory]
        [InlineData("&&")]
        [InlineData("||")]
        public void InferBinaryOp_LogicalOperators_ReturnBool(string op)
        {
            var result = engine.InferBinaryOp(op, TypeInferenceEngine.Bool, TypeInferenceEngine.Bool);
            
            Assert.Equal(TypeInferenceEngine.Bool, result);
        }

        [Theory]
        [InlineData("&")]
        [InlineData("|")]
        [InlineData("^")]
        [InlineData("<<")]
        [InlineData(">>")]
        [InlineData(">>>")]
        public void InferBinaryOp_BitwiseOperators_ReturnInt(string op)
        {
            var result = engine.InferBinaryOp(op, TypeInferenceEngine.Int, TypeInferenceEngine.Int);
            
            Assert.Equal(TypeInferenceEngine.Int, result);
        }

        [Fact]
        public void InferBinaryOp_NullishCoalescing_ReturnsRightType()
        {
            var result = engine.InferBinaryOp("??", TypeInferenceEngine.Null, TypeInferenceEngine.String);
            
            Assert.Equal(TypeInferenceEngine.String, result);
        }

        #endregion

        #region Unary Operator Type Tests

        [Theory]
        [InlineData("-", "int", "int")]
        [InlineData("-", "double", "double")]
        [InlineData("+", "int", "int")]
        [InlineData("+", "double", "double")]
        public void InferUnaryOp_NumericOperators_PreserveType(string op, string operandType, string expectedType)
        {
            var operand = new PrimitiveType(operandType);
            var result = engine.InferUnaryOp(op, operand);
            
            Assert.IsType<PrimitiveType>(result);
            Assert.Equal(expectedType, ((PrimitiveType)result).TypeName);
        }

        [Fact]
        public void InferUnaryOp_LogicalNot_ReturnsBool()
        {
            var result = engine.InferUnaryOp("!", TypeInferenceEngine.Bool);
            
            Assert.Equal(TypeInferenceEngine.Bool, result);
        }

        [Fact]
        public void InferUnaryOp_BitwiseNot_ReturnsInt()
        {
            var result = engine.InferUnaryOp("~", TypeInferenceEngine.Int);
            
            Assert.Equal(TypeInferenceEngine.Int, result);
        }

        #endregion

        #region Ternary Operator Type Tests

        [Fact]
        public void InferTernary_SameTypes_ReturnsSameType()
        {
            var result = engine.InferTernary(
                TypeInferenceEngine.Bool,
                TypeInferenceEngine.String,
                TypeInferenceEngine.String
            );
            
            Assert.Equal(TypeInferenceEngine.String, result);
        }

        [Fact]
        public void InferTernary_IntAndDouble_ReturnsDouble()
        {
            var result = engine.InferTernary(
                TypeInferenceEngine.Bool,
                TypeInferenceEngine.Int,
                TypeInferenceEngine.Double
            );
            
            Assert.Equal(TypeInferenceEngine.Double, result);
        }

        [Fact]
        public void InferTernary_IncompatibleTypes_ReturnsUnionType()
        {
            var result = engine.InferTernary(
                TypeInferenceEngine.Bool,
                TypeInferenceEngine.String,
                TypeInferenceEngine.Int
            );
            
            Assert.IsType<UnionType>(result);
            var unionType = (UnionType)result;
            Assert.Equal(2, unionType.Types.Count);
        }

        #endregion

        #region Type Compatibility Tests

        [Fact]
        public void IsCompatibleWith_SamePrimitiveTypes_ReturnsTrue()
        {
            var type1 = new PrimitiveType("int");
            var type2 = new PrimitiveType("int");
            
            Assert.True(type1.IsCompatibleWith(type2));
        }

        [Fact]
        public void IsCompatibleWith_IntAndDouble_ReturnsTrue()
        {
            Assert.True(TypeInferenceEngine.Double.IsCompatibleWith(TypeInferenceEngine.Int));
            Assert.True(TypeInferenceEngine.Int.IsCompatibleWith(TypeInferenceEngine.Double));
        }

        [Fact]
        public void IsCompatibleWith_DifferentPrimitiveTypes_ReturnsFalse()
        {
            Assert.False(TypeInferenceEngine.String.IsCompatibleWith(TypeInferenceEngine.Int));
            Assert.False(TypeInferenceEngine.Bool.IsCompatibleWith(TypeInferenceEngine.String));
        }

        [Fact]
        public void IsCompatibleWith_SameArrayTypes_ReturnsTrue()
        {
            var array1 = new ArrayType(TypeInferenceEngine.Int);
            var array2 = new ArrayType(TypeInferenceEngine.Int);
            
            Assert.True(array1.IsCompatibleWith(array2));
        }

        [Fact]
        public void IsCompatibleWith_DifferentArrayTypes_ReturnsFalse()
        {
            var intArray = new ArrayType(TypeInferenceEngine.Int);
            var stringArray = new ArrayType(TypeInferenceEngine.String);
            
            Assert.False(intArray.IsCompatibleWith(stringArray));
        }

        [Fact]
        public void IsCompatibleWith_ObjectTypes_ChecksStructure()
        {
            var obj1 = new ObjectType(new Dictionary<string, FSType>
            {
                { "name", TypeInferenceEngine.String },
                { "age", TypeInferenceEngine.Int }
            });
            
            var obj2 = new ObjectType(new Dictionary<string, FSType>
            {
                { "name", TypeInferenceEngine.String },
                { "age", TypeInferenceEngine.Int }
            });
            
            Assert.True(obj1.IsCompatibleWith(obj2));
        }

        [Fact]
        public void IsCompatibleWith_ObjectTypesWithDifferentFields_ReturnsFalse()
        {
            var obj1 = new ObjectType(new Dictionary<string, FSType>
            {
                { "name", TypeInferenceEngine.String }
            });
            
            var obj2 = new ObjectType(new Dictionary<string, FSType>
            {
                { "age", TypeInferenceEngine.Int }
            });
            
            Assert.False(obj1.IsCompatibleWith(obj2));
        }

        #endregion

        #region Symbol Table Tests

        [Fact]
        public void RegisterAndLookupSymbol_Works()
        {
            engine.RegisterSymbol("myVar", TypeInferenceEngine.String);
            
            var type = engine.LookupSymbol("myVar");
            
            Assert.NotNull(type);
            Assert.Equal(TypeInferenceEngine.String, type);
        }

        [Fact]
        public void LookupSymbol_UnknownSymbol_ReturnsNull()
        {
            var type = engine.LookupSymbol("unknownVar");
            
            Assert.Null(type);
        }

        #endregion

        #region C# Type Generation Tests

        [Theory]
        [InlineData("int", "int")]
        [InlineData("double", "double")]
        [InlineData("string", "string")]
        [InlineData("bool", "bool")]
        [InlineData("object?", "object?")]
        [InlineData("BigInteger", "BigInteger")]
        public void ToCSharpType_PrimitiveTypes_GeneratesCorrectString(string typeName, string expected)
        {
            var type = new PrimitiveType(typeName);
            
            Assert.Equal(expected, type.ToCSharpType());
        }

        [Fact]
        public void ToCSharpType_ArrayType_GeneratesCorrectString()
        {
            var arrayType = new ArrayType(TypeInferenceEngine.Int);
            
            Assert.Equal("int[]", arrayType.ToCSharpType());
        }

        [Fact]
        public void ToCSharpType_NestedArrayType_GeneratesCorrectString()
        {
            var innerArray = new ArrayType(TypeInferenceEngine.String);
            var outerArray = new ArrayType(innerArray);
            
            Assert.Equal("string[][]", outerArray.ToCSharpType());
        }

        [Fact]
        public void ToCSharpType_UnionType_ReturnsObject()
        {
            var unionType = new UnionType(TypeInferenceEngine.String, TypeInferenceEngine.Int);
            
            Assert.Equal("object", unionType.ToCSharpType());
        }

        [Fact]
        public void ToString_Types_ProducesReadableOutput()
        {
            var intType = TypeInferenceEngine.Int;
            Assert.Equal("int", intType.ToString());
            
            var arrayType = new ArrayType(TypeInferenceEngine.String);
            Assert.Equal("Array<string>", arrayType.ToString());
            
            var unionType = new UnionType(TypeInferenceEngine.String, TypeInferenceEngine.Int);
            Assert.Equal("string | int", unionType.ToString());
            
            var objType = new ObjectType(new Dictionary<string, FSType>
            {
                { "x", TypeInferenceEngine.Int },
                { "y", TypeInferenceEngine.Int }
            });
            Assert.Equal("{ x: int, y: int }", objType.ToString());
        }

        #endregion
    }
}