// math.f.js - Math utility module

const PI = 3.14159265359
const E = 2.71828182846

const add = (a, b) => a + b
const subtract = (a, b) => a - b
const multiply = (a, b) => a * b
const divide = (a, b) => b !== 0 ? a / b : undefined

const square = x => x * x
const cube = x => x * x * x
const power = (base, exp) => base ** exp

const abs = x => x < 0 ? -x : x
const min = (a, b) => a < b ? a : b
const max = (a, b) => a > b ? a : b

const factorial = n => n <= 1 ? 1 : n * factorial(n - 1)

export default {
    PI,
    E,
    add,
    subtract,
    multiply,
    divide,
    square,
    cube,
    power,
    abs,
    min,
    max,
    factorial
}