// 09-imports.f.js - Import examples

// Default import
import math from '../modules/math.f.js'
import utils from '../modules/utils.f.js'

// Namespace import (import all as namespace)
import * as MathLib from '../modules/math.f.js'
import * as Utils from '../modules/utils.f.js'

// Using imported functions
const sum = math.add(10, 20)
const product = math.multiply(5, 6)
const squared = math.square(7)

// Using namespace imports
const pi = MathLib.PI
const e = MathLib.E
const power = MathLib.power(2, 8)

// Using utility functions
const nullable = null
const withDefault = utils.defaultValue(nullable, "default")
const isIt = utils.isNull(nullable)

// Composed operations
const addTen = x => math.add(x, 10)
const multiplyByTwo = x => math.multiply(x, 2)
const composed = utils.compose(addTen, multiplyByTwo)
const result = composed(5) // (5 + 10) * 2 = 30

export default {
    sum,
    product,
    squared,
    pi,
    e,
    power,
    withDefault,
    isIt,
    result
}