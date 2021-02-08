# csts

Typescript runtime in C#

## Why?

The simplicity of TS is a great asset and if one could have it as a scripting language as [lua](https://www.lua.org/home.html) or [AngelScript](https://www.angelcode.com/angelscript/), we could gain enourmous power. Obviously, if one can runtime a subset of TS, we might as well compile it :)
And... why not?

## How?

We use ANTLR as a compiler frontent to define syntax and rules (lexer and parser).
Machine code generation or runtime exectution is still in research phase

## Resources

- [Tutorial on how to write a compiler using ANTLR and LLVM](https://tomassetti.me/a-tutorial-on-how-to-write-a-compiler-using-llvm/)
- [Tutorial on how to generate the Lexer and Parser from TS grammar](https://hayeol.tistory.com/45)
- [TS Grammar](https://github.com/antlr/grammars-v4/tree/master/typescript)
- [ANTLR jar](https://www.antlr.org/download.html)
- [The Definitive ANTLR 4 Reference](https://pragprog.com/titles/tpantlr2/the-definitive-antlr-4-reference/)
