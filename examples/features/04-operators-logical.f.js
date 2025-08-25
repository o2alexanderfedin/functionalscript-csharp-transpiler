// 04-operators-logical.f.js - Logical operators

const truthy = true
const falsy = false
const zero = 0
const emptyString = ""
const nonEmptyString = "text"
const nullValue = null
const undefinedValue = undefined

// Logical AND (&&)
const andTrueTure = truthy && truthy
const andTrueFalse = truthy && falsy
const andFalseFalse = falsy && falsy
const andShortCircuit = falsy && undefinedValue // Returns falsy, doesn't evaluate second

// Logical OR (||)
const orTrueTrue = truthy || truthy
const orTrueFalse = truthy || falsy
const orFalseFalse = falsy || falsy
const orShortCircuit = truthy || undefinedValue // Returns truthy, doesn't evaluate second

// Logical NOT (!)
const notTrue = !truthy
const notFalse = !falsy
const notZero = !zero
const notEmptyString = !emptyString
const notNull = !nullValue
const doubleNot = !!nonEmptyString

// Nullish coalescing (??)
const nullishNull = nullValue ?? "default"
const nullishUndefined = undefinedValue ?? "default"
const nullishZero = zero ?? "default" // Returns 0, not "default"
const nullishEmpty = emptyString ?? "default" // Returns "", not "default"
const nullishFalse = falsy ?? "default" // Returns false, not "default"

// Complex logical expressions
const complex1 = (truthy && nonEmptyString) || falsy
const complex2 = (nullValue ?? zero) || emptyString || "fallback"
const complex3 = !falsy && (truthy || undefinedValue)

export default {
    andTrueTure,
    andTrueFalse,
    andShortCircuit,
    orTrueTrue,
    orTrueFalse,
    orShortCircuit,
    notTrue,
    notFalse,
    notZero,
    notNull,
    doubleNot,
    nullishNull,
    nullishUndefined,
    nullishZero,
    nullishEmpty,
    nullishFalse,
    complex1,
    complex2,
    complex3
}