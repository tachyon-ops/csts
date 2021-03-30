using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using TypeScriptNative;
using TypeScriptNative.Scan;
using TypeScriptNative.Parse;
using TypeScriptNative.AST;
using TypeScriptNative.Interpret;
using TypeScriptNative.Passes;

namespace prog_lang
{

	enum Operation
	{
		INTERPRETER,
		COMPILER
	}

	class Program
	{
		private static readonly Interpreter interpreter = new();

		private static void Run(String source)
		{
			Scanner scanner = new(source);
			List<Token> tokens = scanner.scanTokens();
			//scanner.debug();

			Parser parser = new(tokens);
			List<Stmt> statements = parser.parse();

			// Stop if there was a syntax error.
			if (ErrorReport.hadError) return;

			Resolver resolver = new(interpreter);
			resolver.resolve(statements);

			//// Stop if there was a resolution error.
			if (ErrorReport.hadError) return;

			interpreter.interpret(statements);
		}

		private static void RunFile(String path)
		{
			byte[] bytes = File.ReadAllBytes(path);
			Run(Encoding.Default.GetString(bytes));

		}
		private static void RunPrompt() { }

		static int Main(string[] args)
		{
			Console.WriteLine("============================================");
			Console.WriteLine("||              ::Welcome::               ||");
			Console.WriteLine("||       TypeScript Native PoC v0.1       ||");
			Console.WriteLine("============================================");

			if (args.Length > 1)
			{
				Console.WriteLine("Usage: jlox [script]");
				return 64;
			}
			else if (args.Length == 1)
			{
				RunFile(args[0]);
			}
			else
			{
				RunPrompt();
			}


			Console.WriteLine("Exiting...");
			return 0;
		}

	}

}
