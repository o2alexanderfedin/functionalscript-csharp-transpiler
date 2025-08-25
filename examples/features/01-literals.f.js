// 01-literals.f.js - All literal types in FunctionalScript

// Null literal
const nullValue = null

// Boolean literals
const boolTrue = true
const boolFalse = false

// Undefined literal
const undefinedValue = undefined

// Number literals - decimal
const integer = 42
const negative = -17
const float = 3.14159
const scientific = 1.23e4
const smallScientific = 4.56e-3

// Number literals - hex
const hexNumber = 0xFF
const hexUpper = 0X1A2B

// Number literals - octal
const octalNumber = 0o755
const octalUpper = 0O644

// Number literals - binary
const binaryNumber = 0b1010
const binaryUpper = 0B1111

// BigInt literals
const bigInt = 123456789012345678901234567890n
const hexBigInt = 0xFFFFFFFFFFFFFFFFn
const octalBigInt = 0o777777777777n
const binaryBigInt = 0b111111111111111111111111n

// String literals
const doubleQuoted = "Hello, World!"
const singleQuoted = 'Single quotes work too'
const withEscapes = "Line 1\nLine 2\tTabbed"
const unicodeString = "\u0048\u0065\u006C\u006C\u006F"

export default {
    nullValue,
    boolTrue,
    boolFalse,
    undefinedValue,
    integer,
    negative,
    float,
    scientific,
    smallScientific,
    hexNumber,
    octalNumber,
    binaryNumber,
    bigInt,
    doubleQuoted,
    singleQuoted
}