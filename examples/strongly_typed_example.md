# Strongly-Typed Transpilation Example

## FunctionalScript Input
```javascript
// Type inference examples
const x = 42                    // inferred as int
const y = 3.14                  // inferred as double  
const name = "Alice"            // inferred as string
const isActive = true           // inferred as bool

// Array with uniform type
const numbers = [1, 2, 3, 4, 5] // inferred as int[]

// Object with known structure
const person = {                // generates a record type
    name: "Bob",
    age: 30,
    active: true
}

// Mixed operations
const result = x + y            // inferred as double (int + double = double)
const comparison = x > 10       // inferred as bool
const message = "Count: " + x   // inferred as string (string concatenation)

export default result
```

## Current Output (with dynamic)
```csharp
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;
using System.Dynamic;

namespace FunctionalScript.Generated {
    public static class Module {
        public static readonly dynamic x = 42;
        public static readonly dynamic y = 3.14;
        public static readonly dynamic name = @"Alice";
        public static readonly dynamic isActive = true;
        public static readonly dynamic numbers = new dynamic[] { 1, 2, 3, 4, 5 };
        public static readonly dynamic person = FunctionalScript.Runtime.CreateObject(
            new Dictionary<string, object> { 
                { "name", @"Bob" }, 
                { "age", 30 }, 
                { "active", true } 
            });
        public static readonly dynamic result = (x + y);
        public static readonly dynamic comparison = (x > 10);
        public static readonly dynamic message = (@"Count: " + x);
        public static dynamic Default => result;
    }
}
```

## Proposed Strongly-Typed Output
```csharp
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Linq;

namespace FunctionalScript.Generated {
    // Generated record type for the person object
    public record PersonType(string name, int age, bool active);
    
    public static class Module {
        public static readonly int x = 42;
        public static readonly double y = 3.14;
        public static readonly string name = @"Alice";
        public static readonly bool isActive = true;
        public static readonly int[] numbers = new int[] { 1, 2, 3, 4, 5 };
        public static readonly PersonType person = new PersonType(@"Bob", 30, true);
        public static readonly double result = (x + y);
        public static readonly bool comparison = (x > 10);
        public static readonly string message = (@"Count: " + x);
        public static double Default => result;
    }
}
```

## Benefits of Strongly-Typed Output

1. **Compile-time type checking** - Errors caught during transpilation
2. **Better performance** - No dynamic dispatch overhead
3. **IntelliSense support** - Full IDE autocomplete and refactoring
4. **Smaller runtime** - No dependency on DLR
5. **AOT compatibility** - Works with ahead-of-time compilation

## Type Inference Rules

| Expression | Inferred Type | Notes |
|------------|--------------|-------|
| `42` | `int` | Integer literal |
| `3.14` | `double` | Floating-point literal |
| `"text"` | `string` | String literal |
| `true`/`false` | `bool` | Boolean literal |
| `[1, 2, 3]` | `int[]` | Homogeneous array |
| `[1, "a", true]` | `object[]` | Mixed-type array (falls back to object) |
| `{ x: 1, y: 2 }` | Generated record | Object literal creates record type |
| `a + b` | Type depends on operands | Numeric or string concatenation |
| `a > b` | `bool` | Comparison always returns bool |
| `a ? b : c` | Common type of b and c | Ternary requires compatible branch types |

## Implementation Status

- ✅ Basic type inference engine
- ✅ Support for primitive types
- ✅ Array type inference
- ✅ Object/record type generation
- ✅ Operator type inference
- ⏳ Integration with parser/grammar
- ⏳ Comprehensive test suite
- ⏳ Runtime library updates
- ⏳ Compiler flag for dynamic mode