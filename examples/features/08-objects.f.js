// 08-objects.f.js - Object literals and operations

// Empty object
const empty = {}

// Simple object
const person = {
    name: "John Doe",
    age: 30,
    isActive: true
}

// Object with various key types
const various = {
    stringKey: "value",
    123: "numeric key",
    "with-dash": "quoted key",
    "with space": "quoted key with space"
}

// Nested objects
const nested = {
    user: {
        profile: {
            firstName: "Jane",
            lastName: "Smith"
        },
        settings: {
            theme: "dark",
            notifications: true
        }
    }
}

// Object with trailing comma
const withTrailingComma = {
    first: 1,
    second: 2,
    third: 3,
}

// Property access
const userName = person.name
const userAge = person.age
const nestedName = nested.user.profile.firstName

// Bracket notation
const bracketAccess = person["name"]
const dynamicKey = "age"
const dynamicAccess = person[dynamicKey]

// Computed property names
const prop = "dynamic"
const computed = {
    [prop]: "value",
    ["computed" + "Key"]: "computed value"
}

// Shorthand properties
const x = 10
const y = 20
const point = { x, y }

// Object spread
const original = { a: 1, b: 2 }
const spread = { ...original }
const extended = { ...original, c: 3 }
const override = { ...original, b: 99 }

// Complex object
const config = {
    api: {
        url: "https://api.example.com",
        timeout: 5000,
        headers: {
            "Content-Type": "application/json",
            "Authorization": "Bearer token"
        }
    },
    features: {
        enableCache: true,
        maxRetries: 3,
        debugMode: false
    }
}

export default {
    empty,
    person,
    various,
    nested,
    userName,
    userAge,
    nestedName,
    bracketAccess,
    dynamicAccess,
    computed,
    point,
    spread,
    extended,
    override,
    config
}