// utils.f.js - Utility functions module

const identity = x => x
const constant = x => () => x

const isNull = x => x === null
const isUndefined = x => x === undefined
const isNullOrUndefined = x => x === null || x === undefined

const defaultValue = (value, fallback) => value ?? fallback

const compose = (f, g) => x => f(g(x))
const pipe = (f, g) => x => g(f(x))

export default {
    identity,
    constant,
    isNull,
    isUndefined,
    isNullOrUndefined,
    defaultValue,
    compose,
    pipe
}