#!/bin/bash

# Clean Test Output Script
# Removes all generated test output files

echo "🧹 Cleaning test output files..."

# Remove test-output directory
if [ -d "test-output" ]; then
    rm -rf test-output
    echo "✓ Removed test-output directory"
fi

# Clean any stray test files in root (just in case)
find . -maxdepth 1 -name "test*.f.cs" -delete 2>/dev/null
find . -maxdepth 1 -name "test*.f.js" -delete 2>/dev/null
find . -maxdepth 1 -name "*.f.cs" -delete 2>/dev/null
find . -maxdepth 1 -name "*.f.js" -delete 2>/dev/null

echo "✨ Test output cleaned!"