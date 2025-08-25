// 11-property-access.f.js - Property access patterns

const obj = {
    name: "Test",
    value: 42,
    nested: {
        deep: {
            prop: "deep value"
        }
    },
    array: [1, 2, 3],
    method: () => "result"
}

// Dot notation
const dotAccess = obj.name
const nestedDot = obj.nested.deep.prop

// Bracket notation with string
const bracketString = obj["name"]
const bracketNested = obj["nested"]["deep"]["prop"]

// Bracket notation with expression
const key = "value"
const dynamicAccess = obj[key]

// Computed property access
const propName = "na" + "me"
const computed = obj[propName]

// Array element access
const firstElement = obj.array[0]
const secondElement = obj.array[1]

// Dynamic array index (requires + operator)
const index = 1
const dynamicIndex = obj.array[+index]

// Method call
const methodResult = obj.method()

// Chained access
const chained = obj.nested.deep.prop

// Safe property access patterns (manual checking)
const safeProp = obj && obj.nested && obj.nested.deep && obj.nested.deep.prop
const withDefault = obj.nonExistent || "default"

// Complex property paths
const complex = {
    users: [
        { id: 1, name: "Alice", roles: ["admin", "user"] },
        { id: 2, name: "Bob", roles: ["user"] }
    ]
}

const firstUserName = complex.users[0].name
const firstUserFirstRole = complex.users[0].roles[0]

export default {
    dotAccess,
    nestedDot,
    bracketString,
    bracketNested,
    dynamicAccess,
    computed,
    firstElement,
    secondElement,
    dynamicIndex,
    methodResult,
    chained,
    safeProp,
    withDefault,
    firstUserName,
    firstUserFirstRole
}