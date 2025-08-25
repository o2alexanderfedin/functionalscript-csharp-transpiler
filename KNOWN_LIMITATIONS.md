# Known Limitations

## FunctionalScript Transpiler - Current Status

✅ **All 78 tests passing (100% success rate)**

The following features have been successfully implemented:
- ✅ Spread operator in arrays (with runtime support)
- ✅ Trailing commas in arrays and objects
- ✅ Object shorthand properties
- ✅ Computed properties
- ✅ Keywords 'from' and 'as' as identifiers

## Remaining Limitations

### 1. Spread Operator in Objects
The spread operator for objects is recognized but may need additional runtime support for complex merging scenarios.

### 2. Advanced JavaScript Features
Some advanced JavaScript/TypeScript features are not yet implemented:
- Destructuring assignments
- Rest parameters in functions
- Template literals
- Async/await
- Classes and inheritance
- Generators
- Modules (import/export beyond default)

## Technical Implementation

The transpiler uses COCO/R parser generator with LL(1) parsing. We've worked around many limitations through:
- Runtime helper functions (e.g., `SpreadArray` for spread operator support)
- Grammar refactoring to handle optional trailing commas
- Semantic validation instead of keyword-based parsing for contextual tokens

## Test Results

As of the latest test run:
- **Total Tests:** 78
- **Passing:** 78 (100%)
- **Failing:** 0

All previously failing tests have been fixed through grammar improvements and runtime support.