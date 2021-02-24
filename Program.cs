using System;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

using TypeScriptNative.src;

namespace prog_lang
{

	enum Operation
	{
		INTERPRETER,
		COMPILER
	}

	class Program
	{

		static int Main(string[] args)
		{
			Console.WriteLine("============================================");
			Console.WriteLine("||              ::Welcome::               ||");
			Console.WriteLine("||       TypeScript Native PoC v0.1       ||");
			Console.WriteLine("============================================");

			if (args.Length < 2)
			{
				Console.WriteLine("Please enter proper arguments.");
				Console.WriteLine("Usage: -c for compiler");
				Console.WriteLine("       -i for interpreter");
				Console.WriteLine("       ./test.ts for TS file");
				Console.WriteLine("Example: program -c ./test.ts");
				return 1;
			}

			Console.WriteLine("Arguments: " + args[0] + " " + args[1]);

			Operation operation = args[0] == "-i" ? Operation.INTERPRETER : Operation.COMPILER;
			string fileName = args[1];
			if (!File.Exists(fileName))
			{
				Console.WriteLine("File " + fileName + " was not found.");
				return 1;
			}
			string text = File.ReadAllText(fileName);
			//Console.WriteLine(text + " OP: " + operation);

			try
			{
				var stream = new AntlrInputStream(text);
				var lexer = new TypeScriptLexer(stream);
				var tokenStream = new CommonTokenStream(lexer);
				var parser = new TypeScriptParser(tokenStream);
				IParseTree tree = parser.program();
				//Console.WriteLine("parse tree (LISP style): \n" + tree.ToStringTree(parser));
				parser.RemoveErrorListeners();
				parser.AddErrorListener(new ThrowingErrorListener<IToken>());

				IRunner runner;
				if (operation == Operation.INTERPRETER)
				{
					Console.WriteLine("Interpreter triggered");
					runner = new Interpreter();
				}
				else
				{
					runner = new Compiler();
				}

				TypeScriptParserListener expressionWalker = new TypeScriptParserListener(runner);
				ParseTreeWalker walker = new ParseTreeWalker();
				// Walker
				walker.Walk(expressionWalker, tree);

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

			Console.WriteLine("Exiting...");
			return 0;
			//Console.Write("Press any key to continue...");
			//Console.ReadKey(true);
		}

	}

}
