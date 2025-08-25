# Known Limitations

## FunctionalScript Transpiler Limitations

Due to COCO/R parser generator constraints (LL(1) parsing), the following JavaScript/TypeScript features are not fully supported:

### 1. Spread Operator in Arrays
The spread operator (`...`) is not implemented for array literals.

**Not Supported:**
```javascript
const arr1 = [1, 2]
const arr2 = [3, 4]
const result = [...arr1, ...arr2]  // Will fail to parse
```

**Workaround:** Use array concatenation methods at runtime or construct arrays without spread.

### 2. Complex Trailing Commas
While simple trailing commas work in arrays and objects, complex scenarios with nested structures may fail.

**Supported:**
```javascript
const arr = [1, 2, 3,]  // Simple trailing comma works
const obj = { a: 1, b: 2, }  // Simple trailing comma works
```

**Not Fully Supported:**
Complex nested structures with trailing commas in certain positions may cause parsing errors.

## Technical Background

These limitations stem from COCO/R's LL(1) parsing constraints:
- LL(1) parsers must make decisions based on a single lookahead token
- Context-sensitive features like spread operators require more complex parsing strategies
- The parser generator creates static parsing tables that cannot handle all JavaScript's dynamic syntax

## Test Results

As of the latest test run:
- **Total Tests:** 78
- **Passing:** 76 (97.4%)
- **Failing:** 2 (TestSpreadOperatorInArray, TestTrailingCommas)

These failures are due to the fundamental parser limitations described above and would require either:
1. Switching to a more powerful parser generator (e.g., ANTLR with LL(*) or GLR parsing)
2. Implementing custom lexical analysis for these specific features
3. Using a hand-written recursive descent parser for more control