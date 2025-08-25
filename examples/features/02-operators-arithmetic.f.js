// 02-operators-arithmetic.f.js - Arithmetic operators

const a = 10
const b = 3

// Basic arithmetic
const addition = a + b
const subtraction = a - b
const multiplication = a * b
const division = a / b
const modulo = a % b
const exponentiation = a ** b

// Unary operators
const positive = +a
const negative = -a
const negativeOfNegative = -(-a)

// Complex expressions
const complex1 = (a + b) * (a - b)
const complex2 = a * b + a / b - a % b
const complex3 = 2 ** 3 ** 2  // Right associative: 2 ** (3 ** 2) = 2 ** 9 = 512

// With different numeric types
const withFloat = 3.14 * 2
const withNegative = -5 + 10
const mixedPrecision = 0.1 + 0.2

export default {
    addition,
    subtraction,
    multiplication,
    division,
    modulo,
    exponentiation,
    positive,
    negative,
    complex1,
    complex2,
    complex3,
    withFloat,
    mixedPrecision
}