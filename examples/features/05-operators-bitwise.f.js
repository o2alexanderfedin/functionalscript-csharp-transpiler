// 05-operators-bitwise.f.js - Bitwise operators

const a = 0b1010  // 10 in decimal
const b = 0b1100  // 12 in decimal
const c = 0xFF    // 255 in decimal

// Bitwise AND
const bitwiseAnd = a & b  // 0b1000 = 8

// Bitwise OR
const bitwiseOr = a | b   // 0b1110 = 14

// Bitwise XOR
const bitwiseXor = a ^ b  // 0b0110 = 6

// Bitwise NOT
const bitwiseNot = ~a     // -11 (two's complement)
const bitwiseNotFF = ~c   // -256

// Left shift
const leftShift1 = a << 1  // 20 (10 * 2)
const leftShift2 = a << 2  // 40 (10 * 4)

// Right shift (sign-propagating)
const rightShift1 = a >> 1  // 5 (10 / 2)
const rightShift2 = b >> 2  // 3 (12 / 4)
const rightShiftNegative = (-16) >> 2  // -4

// Unsigned right shift
const unsignedRightShift = (-1) >>> 1  // Large positive number
const unsignedNormal = b >>> 1  // 6

// Complex bitwise operations
const mask = 0b11110000
const masked = c & mask
const flags = 0b0001 | 0b0100 | 0b1000
const toggle = flags ^ 0b0100

export default {
    bitwiseAnd,
    bitwiseOr,
    bitwiseXor,
    bitwiseNot,
    bitwiseNotFF,
    leftShift1,
    leftShift2,
    rightShift1,
    rightShift2,
    rightShiftNegative,
    unsignedRightShift,
    unsignedNormal,
    masked,
    flags,
    toggle
}