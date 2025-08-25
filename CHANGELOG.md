# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.0] - 2025-08-25

### Added
- Full support for spread operator (`...`) in arrays with runtime helper
- `SpreadArray` runtime function for array spreading operations
- Support for trailing commas in arrays and objects
- Documentation of current feature status and limitations

### Fixed
- Trailing comma parsing issues in arrays and objects
- Object shorthand property grammar conflicts
- Test expectations for computed properties
- Array of objects transpilation

### Changed
- Refactored ElementList and PropertyList grammar rules for better comma handling
- Updated KNOWN_LIMITATIONS.md to reflect current status
- Improved test coverage with 100% success rate (78/78 tests passing)

## [1.1.0] - Previous Release

### Added
- Object shorthand properties support
- Computed property names in objects
- Support for 'from' and 'as' as identifiers

### Fixed
- Keyword conflicts in import statements
- Grammar issues with property parsing

## [1.0.0] - Initial Release

### Added
- Basic FunctionalScript to C# transpiler
- Support for JavaScript literals, operators, and expressions
- Import/export statement handling
- Object and array literal support
- Runtime library for JavaScript-like operations