# LibSceSharp
C# bindings for [libsce](https://github.com/LittleBigRefresh/libsce), a library to decrypt, decompress, and disassemble files used by various Sony systems.

## Installation

This library comes in two parts:

- `LibSceSharp`: This is the wrapper itself, including the actual binding layers.
- `LibSceSharp.Native`: These are the native libraries. You'll want these if you plan to actually execute from `LibSceSharp`.

## Usage

See our test project for simple usage:
https://github.com/LittleBigRefresh/LibSceSharp/blob/master/LibSceSharp.Test/Program.cs
