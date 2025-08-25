// 12-builtin-objects.f.js - Built-in objects and methods

// Object methods
const original = { a: 1, b: 2 }
const target = { c: 3 }
const assigned = Object.assign(target, original)
const entries = Object.entries(original)
const keys = Object.keys(original)
const values = Object.values(original)
const frozen = Object.freeze({ x: 1 })
const sealed = Object.seal({ y: 2 })

// Object.is comparisons
const isEqual = Object.is(NaN, NaN)  // true
const isNotEqual = Object.is(0, -0)  // false
const isSame = Object.is(1, 1)       // true

// Array methods
const fromIterable = Array.from([1, 2, 3])
const fromString = Array.from("hello")
const ofElements = Array.of(1, 2, 3, 4, 5)
const checkArray = Array.isArray([1, 2, 3])
const checkNotArray = Array.isArray("not array")

// JSON operations
const objectToJson = JSON.stringify({ name: "John", age: 30 })
const arrayToJson = JSON.stringify([1, 2, 3])
const parsed = JSON.parse('{"key":"value"}')
const parsedArray = JSON.parse('[1,2,3]')

// BigInt operations
const bigIntFromNumber = BigInt(123)
const bigIntFromString = BigInt("999999999999999999")
const bigIntOps = BigInt(10) + BigInt(20)

// Math operations (if supported as built-ins)
const absolute = Math.abs(-42)
const power = Math.pow(2, 10)
const maximum = Math.max(1, 5, 3, 9, 2)
const minimum = Math.min(1, 5, 3, 9, 2)

export default {
    assigned,
    entries,
    keys,
    values,
    frozen,
    sealed,
    isEqual,
    isNotEqual,
    isSame,
    fromIterable,
    fromString,
    ofElements,
    checkArray,
    checkNotArray,
    objectToJson,
    arrayToJson,
    parsed,
    parsedArray,
    bigIntFromNumber,
    bigIntFromString,
    bigIntOps,
    absolute,
    power,
    maximum,
    minimum
}