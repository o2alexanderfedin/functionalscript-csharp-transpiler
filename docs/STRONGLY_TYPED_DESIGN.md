# Strongly-Typed FunctionalScript Transpiler Design

## Overview
This document outlines the design for transitioning the FunctionalScript transpiler from generating `dynamic` types to producing fully strongly-typed C# code.

## Current Limitations with `dynamic`
1. **Performance overhead** - Dynamic dispatch is slower than static dispatch
2. **No compile-time type checking** - Type errors only discovered at runtime
3. **Limited IntelliSense support** - IDEs can't provide good autocomplete
4. **Larger runtime footprint** - DLR (Dynamic Language Runtime) overhead

## Proposed Type System

### Type Inference Algorithm
Since FunctionalScript doesn't have explicit type annotations, we'll implement Hindley-Milner type inference:

1. **Literal Type Inference**
   - Numbers → `double` (or `int` if no decimal point)
   - Strings → `string`
   - Booleans → `bool`
   - BigInt → `BigInteger`
   - Arrays → `T[]` where T is inferred from elements
   - Objects → Generated record types or anonymous types

2. **Expression Type Inference**
   - Binary operators infer result type from operands
   - Ternary operator requires branches to have compatible types
   - Function calls infer from known function signatures

3. **Const Binding Type Inference**
   - Type is inferred from the initializer expression
   - Once inferred, the type is fixed (immutable)

### Implementation Phases

#### Phase 1: Basic Type Inference
- Implement type inference for literals
- Add type tracking to CodeGenerator
- Generate typed constants instead of `dynamic`

#### Phase 2: Collection Types
- Infer array element types (handle homogeneous arrays)
- Generate strongly-typed record classes for objects
- Handle spread operators with type preservation

#### Phase 3: Advanced Features
- Union types for heterogeneous arrays/mixed types
- Generic type parameters for reusable code
- Type aliases for complex types

## Example Transformations

### Before (Current - Dynamic)
```csharp
public static readonly dynamic x = 42;
public static readonly dynamic arr = new dynamic[] { 1, 2, 3 };
public static readonly dynamic obj = FunctionalScript.Runtime.CreateObject(...);
```

### After (Strongly-Typed)
```csharp
public static readonly int x = 42;
public static readonly int[] arr = new int[] { 1, 2, 3 };
public static readonly record ObjType(int a, string b);
public static readonly ObjType obj = new ObjType(1, "test");
```

## Type Mapping

| FunctionalScript Expression | Inferred C# Type |
|---------------------------|-----------------|
| `42` | `int` |
| `3.14` | `double` |
| `"hello"` | `string` |
| `true` | `bool` |
| `null` | `null` (nullable context) |
| `undefined` | `null` or custom `Undefined` type |
| `[1, 2, 3]` | `int[]` |
| `["a", "b"]` | `string[]` |
| `{ x: 1, y: 2 }` | Generated record or anonymous type |
| `123n` | `BigInteger` |

## Mixed Types Strategy

For cases where types can't be unified:

### Option 1: Union Types (Discriminated Unions)
```csharp
public abstract record UnionType;
public record UnionInt(int Value) : UnionType;
public record UnionString(string Value) : UnionType;
```

### Option 2: Common Base Type
Fall back to `object` only when absolutely necessary, with runtime checks.

### Option 3: Generic Wrappers
```csharp
public record Value<T>(T Data);
```

## Modified Runtime Support

The FunctionalScript.Runtime library needs updates:
1. Strongly-typed versions of utility functions
2. Generic methods instead of dynamic
3. Type-preserving operations

## Backward Compatibility

- Add a compiler flag `--use-dynamic` to maintain old behavior
- Gradual migration path for existing code
- Type inference can be disabled per-file with a directive

## Benefits

1. **Better Performance** - No dynamic dispatch overhead
2. **Compile-time Safety** - Catch type errors during transpilation
3. **IDE Support** - Full IntelliSense and refactoring support
4. **Smaller Runtime** - No DLR dependency
5. **AOT Compatible** - Works with .NET Native/AOT compilation

## Challenges

1. **JavaScript Compatibility** - Some JS patterns are inherently dynamic
2. **Type Inference Complexity** - Need sophisticated algorithm
3. **Generic Variance** - Handling covariance/contravariance
4. **Recursive Types** - Objects that reference themselves

## Implementation Plan

1. Create `TypeInference` class with inference engine
2. Modify `CodeGenerator` to track and use types
3. Update grammar to support optional type hints (future)
4. Create strongly-typed runtime library variants
5. Add comprehensive type inference tests
6. Document type inference rules

## Future Extensions

1. **Optional Type Annotations** - Allow explicit types: `const x: number = 42`
2. **Type Aliases** - `type Point = { x: number, y: number }`
3. **Interfaces** - Structural typing support
4. **Generics** - `function map<T, U>(arr: T[], fn: (T) => U): U[]`
5. **Type Guards** - Runtime type checking helpers

## Conclusion

Moving to a strongly-typed system will make FunctionalScript a more robust and performant language while maintaining its functional programming benefits. The type inference system will preserve the developer experience of not requiring explicit type annotations while providing all the benefits of static typing.