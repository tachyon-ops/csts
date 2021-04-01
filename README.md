# csts

Typescript runtime in C# (WIP and PoC trial)

## Why?

The simplicity of TS is a great asset and if one could have it as a scripting language as [lua](https://www.lua.org/home.html) or [AngelScript](https://www.angelcode.com/angelscript/), we could gain enourmous power. Obviously, if one can runtime a subset of TS, we might as well compile it :)
And... why not?

## How to publish?

### Mac OS

- `dotnet publish -r osx-x64` (just publishes)

- See more at [https://avaloniaui.net/docs/packing/macOS#manual](https://avaloniaui.net/docs/packing/macOS#manual) 

- `dotnet msbuild -t:BundleApp -p:RuntimeIdentifier=osx-x64` (bundles into TypeScriptNative.app)

### Linux

- `dotnet publish -r linux-x64 -p:PublishSingleFile=true --self-contained true`

## How to run?

If you have only build the debug, use the following:
- ./bin/Debug/net5.0/osx-x64/TypeScriptNative

## How?

1. Scan source code and generate token list
2. Parse the token list
3. Resolve all structure (Type checking to be done still - it's the next pass)
4. Interpret

Note: ANTLR was dropped in favour of a manual approach as it is not that much work :)

# Supported and unsupported features

## Unsupported / not implemented

* const
* let
* import
* types
	* you can type vars and functions but it will do nothing as of now
* as Type

## Edge cases

* `println("a" + 3);` right now we error it, but according to JS implementation, we should
allow for it to be concatenated: `a3`. We will warn the user, but allow it.


## LL code to EXEC

1. `llc code.ll` -> generates `code.s`
2. `gcc code.s -o code` -> generates `code` executable: run it by `./code`
   From: [SO how can I create an executable from LLVM IR](https://stackoverflow.com/questions/45985953/how-can-i-create-an-executable-from-llvm-ir)

## Resources

- Test TypeScript: https://codesandbox.io/s/react-typescript-playground-forked-ipe11?file=/src/index.tsx

- [Roman language (and how to interpret a simple ANTLR)](https://gjdanis.github.io/2016/01/23/roman/) and a [SO](https://codereview.stackexchange.com/questions/117711/roman-numerals-with-antlr) to explain further.

- [Basic C# interpreter](https://github.com/pg94au/Blinkenlights-.NET-Basic-Interpreter)

- [How to use LLVM](https://tomassetti.me/a-tutorial-on-how-to-write-a-compiler-using-llvm/)
- [How to use LLVM in CSharp](https://ice1000.org/llvm-cs/en/CSharpLangImpl03/)

- [Tutorial on how to write a compiler using ANTLR and LLVM](https://tomassetti.me/a-tutorial-on-how-to-write-a-compiler-using-llvm/)
- [Tutorial on how to generate the Lexer and Parser from TS grammar](https://hayeol.tistory.com/45)
- [TS Grammar](https://github.com/antlr/grammars-v4/tree/master/typescript)
- [ANTLR jar](https://www.antlr.org/download.html)
- [The Definitive ANTLR 4 Reference](https://pragprog.com/titles/tpantlr2/the-definitive-antlr-4-reference/)


* https://stackoverflow.com/questions/63542345/how-to-detect-for-loop-block-after-parsing-the-code-using-antlr

* http://franckgaspoz.fr/en/first-steps-with-antlr4-in-csharp/

* https://stackoverflow.com/questions/29971097/how-to-create-ast-with-antlr4

# Big thanks to LOX!

Check out the amazing online book [Crafting Interpreters](https://craftinginterpreters.com) by Bob Nystrom.