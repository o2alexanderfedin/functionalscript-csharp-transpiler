// 06-conditional.f.js - Conditional (ternary) operator

const age = 25
const score = 85
const name = "Alice"
const items = [1, 2, 3]

// Simple conditional
const canVote = age >= 18 ? true : false
const grade = score >= 90 ? "A" : score >= 80 ? "B" : score >= 70 ? "C" : "F"

// With different types
const greeting = name ? `Hello, ${name}` : "Hello, stranger"
const hasItems = items.length > 0 ? "Has items" : "Empty"

// Nested conditionals
const category = age < 13 ? "child" : 
                 age < 20 ? "teenager" : 
                 age < 60 ? "adult" : 
                 "senior"

// With nullish values
const maybeNull = null
const maybeUndefined = undefined
const defaultValue = maybeNull ? maybeNull : "default"
const checkUndefined = maybeUndefined !== undefined ? maybeUndefined : "was undefined"

// Complex expressions in conditions
const x = 10
const y = 20
const max = x > y ? x : y
const min = x < y ? x : y
const absX = x < 0 ? -x : x

// Conditional with logical operators
const isValid = true
const isEnabled = false
const status = isValid && isEnabled ? "active" : !isValid ? "invalid" : "disabled"

export default {
    canVote,
    grade,
    greeting,
    hasItems,
    category,
    defaultValue,
    checkUndefined,
    max,
    min,
    absX,
    status
}