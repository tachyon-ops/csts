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

		private static void Run(String source, String path)
		{
			Scanner scanner = new(source);
			List<Token> tokens = scanner.scanTokens();
			//scanner.debug();

			Parser parser = new(tokens, path);
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
			if (!File.Exists(path)) {
				Console.WriteLine("Provided file was not found.");
			}
			else
			{
				byte[] bytes = File.ReadAllBytes(path);
				var directory = Path.GetDirectoryName(path);
				Console.WriteLine("Running from the following path: " + directory);
				Run(Encoding.Default.GetString(bytes), directory);
			}

		}
		private static void RunPrompt()
		{
			var path = AppContext.BaseDirectory;
			Console.WriteLine("Running from the following path: " + path);
			while (true) // Loop indefinitely
			{
				Console.Write("> "); // Prompt
				string line = Console.ReadLine(); // Get string from user
				if (line == null) // Check string
				{
					break;
				}
				Run(line, path);
				ErrorReport.hadError = false;
			}
		}

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
