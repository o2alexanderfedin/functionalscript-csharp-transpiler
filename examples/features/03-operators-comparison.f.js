// 03-operators-comparison.f.js - Comparison operators

const a = 10
const b = 10
const c = 20
const str1 = "hello"
const str2 = "hello"
const str3 = "world"

// Strict equality (only === and !== allowed in FunctionalScript)
const strictEqual = a === b
const strictNotEqual = a !== c
const stringEqual = str1 === str2
const stringNotEqual = str1 !== str3

// Relational operators
const lessThan = a < c
const greaterThan = c > a
const lessOrEqual = a <= b
const greaterOrEqual = b >= a

// Complex comparisons
const chainedComparison = (a < c) && (c > b)
const mixedComparison = (str1 === str2) && (a !== c)

// With different types (always false for strict equality)
const numberVsString = 10 !== "10"
const nullComparison = null === null
const undefinedComparison = undefined === undefined
const nullVsUndefined = null !== undefined

export default {
    strictEqual,
    strictNotEqual,
    stringEqual,
    stringNotEqual,
    lessThan,
    greaterThan,
    lessOrEqual,
    greaterOrEqual,
    chainedComparison,
    mixedComparison,
    numberVsString,
    nullComparison,
    undefinedComparison,
    nullVsUndefined
}