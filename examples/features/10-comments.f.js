// 10-comments.f.js - Comments and JSDoc

// Single line comment
const x = 42 // inline comment

/** 
 * Block comment
 * Multiple lines
 * Can span several lines
 */
const y = 100

/** @type {number} */
const typed = 42

/** @type {string} */
const name = "TypedString"

/** @type {boolean} */
const flag = true

/** 
 * @type {Array<number>} 
 */
const numbers = [1, 2, 3, 4, 5]

/**
 * @type {Object}
 * @property {string} name
 * @property {number} age
 */
const person = {
    name: "John",
    age: 30
}

// TODO: This is a todo comment
// FIXME: This needs fixing
// NOTE: Important note here

/* 
  Old-style block comment
  Also works
*/
const z = 200

export default {
    x,
    y,
    typed,
    name,
    flag,
    numbers,
    person,
    z
}