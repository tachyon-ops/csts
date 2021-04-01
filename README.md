# csts

Typescript runtime in C# (WIP and PoC trial)
Note that this is a Proof Of Concept! By all means this is not to be used for production, and is intended as a SandBox and test ground. It is intended to defy the very assumptions most people hold when they think 'javascript' or 'typescript' need to be executed in a browser. No, that is not a necessity and one could even implement assembly code from either to compile to native architectures.
Be aware that you are entering an opinionated real, for which barriers need to be taken down. The first barrier is always our dearest: what we assume we know - and usually it is our biggest fault and impediment to progress. It is a collective fault of mankind, as without it I am sure we wouldn't be in such an ecologic hassle, having move past our problems.

This is the attempt to prove that we use too much processing power because we are lazy: we build with simple tools and languages - no harm there, and we should all aim to do that - but we care not to improve the foundations of our technology stack. Today, we surely burn through energy and batteries with our CPUs at a rate that could be improved, if only we had less browser driven development even for native applications.

If we as developers do that, shouldn't we also aim to clean up and build a more pristine foundation? I believe we should. If you believe so as well, reach out and try your best. You might learn a lot with that mindset.

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

- const
- let
- import
- types
  - you can type vars and functions but it will do nothing as of now
- as Type

## Edge cases

- `println("a" + 3);` according to JS implementation, we should allow for it to be concatenated: `a3` . Warns the user, but allows it.

## LL code to EXEC

1. `llc code.ll` -> generates `code.s`
2. `gcc code.s -o code` -> generates `code` executable: run it by `./code`

   From: [SO how can I create an executable from LLVM IR](https://stackoverflow.com/questions/45985953/how-can-i-create-an-executable-from-llvm-ir)

## Resources

- [Test TypeScript](https://codesandbox.io/s/react-typescript-playground-forked-ipe11?file=/src/index.tsx)

- [Roman language (and how to interpret a simple ANTLR)](https://gjdanis.github.io/2016/01/23/roman/) and a [SO](https://codereview.stackexchange.com/questions/117711/roman-numerals-with-antlr) to explain further.

- [Basic C# interpreter](https://github.com/pg94au/Blinkenlights-.NET-Basic-Interpreter)

- [How to use LLVM](https://tomassetti.me/a-tutorial-on-how-to-write-a-compiler-using-llvm/)
- [How to use LLVM in CSharp](https://ice1000.org/llvm-cs/en/CSharpLangImpl03/)

- [Tutorial on how to write a compiler using ANTLR and LLVM](https://tomassetti.me/a-tutorial-on-how-to-write-a-compiler-using-llvm/)
- [Tutorial on how to generate the Lexer and Parser from TS grammar](https://hayeol.tistory.com/45)
- [TS Grammar](https://github.com/antlr/grammars-v4/tree/master/typescript)
- [ANTLR jar](https://www.antlr.org/download.html)
- [The Definitive ANTLR 4 Reference](https://pragprog.com/titles/tpantlr2/the-definitive-antlr-4-reference/)

- https://stackoverflow.com/questions/63542345/how-to-detect-for-loop-block-after-parsing-the-code-using-antlr

- http://franckgaspoz.fr/en/first-steps-with-antlr4-in-csharp/

- https://stackoverflow.com/questions/29971097/how-to-create-ast-with-antlr4

# Big thanks to LOX!

Check out the amazing online book [Crafting Interpreters](https://craftinginterpreters.com) by Bob Nystrom.

# Benchmark

Here are a abit of juicy results for you :)

## CPP Benchmark

Go to `benchmarks/cpp` and execute

`time ./benchmarks/c/hello ./examples/time-benchmark.ts`

Expected output:

```
Hello, world!
./benchmarks/c/hello ./examples/time-benchmark.ts  0.00s user 0.00s system 69% cpu 0.005 total
```

## Deno Benchmark

Install [deno](https://deno.land) and:

`time deno run ./examples/time-benchmark.ts`

Output:

```
Check file:///Users/nmpribeiro/work/prog_lang/csts_gh/examples/time-benchmark.ts
Hello world
deno run ./examples/time-benchmark.ts  0.49s user 0.03s system 140% cpu 0.371 total
```

## TypeScriptNative Benchmark

`time ./bin/Debug/net5.0/osx-x64/TypeScriptNative ./examples/time-benchmark.ts`

Output:

```
============================================
||              ::Welcome::               ||
||       TypeScript Native PoC v0.1       ||
============================================
Running from the following path: ./examples
Get: Undefined variable 'console'.
[line 10]
Exiting...

./bin/Debug/net5.0/osx-x64/TypeScriptNative ./examples/time-benchmark.ts  0.07s user 0.02s system 108% cpu 0.083 total
```
