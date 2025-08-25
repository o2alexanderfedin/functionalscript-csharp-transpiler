// Complex expressions with nullish coalescing and logical operators
const x = null
const y = undefined
const z = 0

const result1 = x ?? y ?? z ?? "default"
const result2 = x || y || z || "fallback" 
const result3 = z && "exists" || "not exists"

export default result1