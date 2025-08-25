#!/bin/bash

# Build script for FunctionalScript to C# transpiler

echo "Building FunctionalScript transpiler..."

# Step 1: Run Coco/R to generate Parser and Scanner from ATG
echo "Generating parser from ATG grammar..."
if command -v Coco &> /dev/null; then
    Coco FunctionalScript.atg
else
    echo "Error: Coco/R not found. Please install Coco/R first."
    echo "Download from: http://www.ssw.uni-linz.ac.at/Coco/"
    exit 1
fi

# Step 2: Compile the C# transpiler
echo "Compiling C# transpiler..."
csc /out:fstranspile.exe Transpiler.cs Parser.cs Scanner.cs FunctionalScriptRuntime.cs

if [ $? -eq 0 ]; then
    echo "Build successful! Transpiler created: fstranspile.exe"
    echo ""
    echo "Usage:"
    echo "  mono fstranspile.exe <input.f.js> [output.cs]"
    echo ""
    echo "Testing with examples..."
    
    # Run tests if examples exist
    if [ -d "examples" ]; then
        for file in examples/*.f.js; do
            if [ -f "$file" ]; then
                echo "Transpiling $file..."
                mono fstranspile.exe "$file" "${file%.f.js}.cs"
            fi
        done
    fi
else
    echo "Build failed!"
    exit 1
fi