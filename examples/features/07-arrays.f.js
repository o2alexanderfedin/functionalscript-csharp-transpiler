// 07-arrays.f.js - Array literals and operations

// Empty array
const empty = []

// Array with values
const numbers = [1, 2, 3, 4, 5]
const mixed = [1, "two", true, null, undefined]

// Nested arrays
const matrix = [
    [1, 2, 3],
    [4, 5, 6],
    [7, 8, 9]
]

// Array with trailing comma (allowed)
const withTrailingComma = [
    "first",
    "second",
    "third",
]

// Accessing array elements
const first = numbers[0]
const second = numbers[1]
const last = numbers[4]

// Accessing nested arrays
const matrixElement = matrix[1][2]  // 6
const firstRow = matrix[0]

// Dynamic array indexing (requires + operator)
const index = 2
const dynamicAccess = numbers[+index]

// Array of objects
const users = [
    { name: "Alice", age: 30 },
    { name: "Bob", age: 25 },
    { name: "Charlie", age: 35 }
]

// Complex array expressions
const computed = [
    1 + 1,
    2 * 3,
    10 / 2,
    true && false,
    null ?? "default"
]

// Array spread (if supported)
const spread = [...numbers]
const combined = [...numbers, ...mixed]

export default {
    empty,
    numbers,
    mixed,
    matrix,
    first,
    second,
    last,
    matrixElement,
    firstRow,
    dynamicAccess,
    users,
    computed,
    spread,
    combined
}