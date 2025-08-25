# FunctionalScript to C# Transpiler

A COCO/R-based transpiler that converts FunctionalScript code to C#.

## Overview

This project implements a complete transpiler for the FunctionalScript language, converting FunctionalScript source code into equivalent C# code. The transpiler is built using COCO/R, a compiler generator that creates parsers and scanners from attributed grammars.

## Files

- `functionalscript.bnf` - Complete BNF grammar specification for FunctionalScript
- `FunctionalScript.atg` - COCO/R attributed grammar with semantic actions for C# code generation
- `FunctionalScriptRuntime.cs` - Runtime support library for transpiled code
- `Transpiler.cs` - Main transpiler program and API
- `examples/` - Test examples demonstrating various language features
- `build.sh` - Build script for compiling the transpiler

## Building

### Prerequisites

1. .NET 8.0 SDK or later
2. COCO/R (installed automatically as global tool)

### Build Steps

```bash
# Install COCO/R if not already installed
dotnet tool install -g CocoR

# Build the solution
dotnet build

# Run tests
dotnet test

# The parser is automatically generated from the ATG grammar during build
```

### Project Structure

- `src/FunctionalScript.Transpiler/` - Core transpiler library
  - `Grammar/FunctionalScript.atg` - COCO/R grammar definition
  - `Parser.cs` & `Scanner.cs` - Auto-generated from ATG (not in source control)
  - `CodeGenerator.cs` - C# code generation logic
  - `Runtime/` - JavaScript runtime compatibility layer
- `src/FunctionalScript.CLI/` - Command-line interface
- `tests/` - Unit tests

### How It Works

1. **Pre-build Event**: The project automatically runs COCO/R to generate Parser.cs and Scanner.cs from the ATG grammar file
2. **Parser Generation**: COCO/R processes `Grammar/FunctionalScript.atg` and creates the lexer and parser
3. **Compilation**: The generated files are compiled with the rest of the project
4. **Runtime**: The transpiled C# code uses the FunctionalScript.Runtime library for JavaScript semantics

## Usage

```bash
# Transpile a FunctionalScript file to C#
mono fstranspile.exe input.f.js output.cs

# Or just specify input (output defaults to .cs extension)
mono fstranspile.exe input.f.js
```

## Language Features Supported

### Core Features
- ES6 module imports/exports (default and namespace)
- Const declarations only (immutable by design)
- All JavaScript literals (including BigInt with 'n' suffix)
- Arrays and object literals
- Property access (dot notation and bracket notation)

### Operators
- Arithmetic: `+`, `-`, `*`, `/`, `%`, `**`
- Comparison: `===`, `!==`, `<`, `>`, `<=`, `>=`
- Logical: `&&`, `||`, `??`, `!`
- Bitwise: `&`, `|`, `^`, `~`, `<<`, `>>`, `>>>`
- Conditional (ternary): `? :`

### Built-in Support
- Object methods: `assign`, `keys`, `values`, `entries`, etc.
- Array methods: `from`, `of`, `isArray`
- JSON: `stringify`, `parse`
- BigInt conversions

### Restrictions
- No loops (functional programming focus)
- No if/else statements (use conditional operator)
- No variable reassignment (const only)
- No class declarations
- No function declarations (only arrow functions in expressions)
- Strict equality only (`===`, `!==`)

## Examples

### Simple Constants
```javascript
// input.f.js
const x = 42
const y = 3.14
export default x + y
```

### Arrays and Objects
```javascript
const data = [1, 2, 3]
const config = {
    name: "test",
    value: 42
}
export default config
```

### Conditional Logic
```javascript
const a = 10
const b = 20
const max = a > b ? a : b
export default max
```

### BigInt Support
```javascript
const bigNum = 123456789012345678901234567890n
export default bigNum
```

## Generated C# Code

The transpiler generates C# code with:
- Static classes for modules
- Dynamic typing for JavaScript compatibility
- Runtime library for JavaScript semantics
- Support for undefined, null coalescing, and logical operators
- BigInteger for BigInt support

## Testing

Run the example tests:
```bash
cd examples
for file in *.f.js; do
    mono ../fstranspile.exe "$file"
done
```

## Architecture

1. **Lexical Analysis**: Scanner tokenizes FunctionalScript source
2. **Parsing**: Parser builds AST using COCO/R grammar
3. **Code Generation**: Semantic actions generate C# code
4. **Runtime Support**: Runtime library provides JavaScript semantics in C#

## License

This transpiler is created for educational and development purposes.