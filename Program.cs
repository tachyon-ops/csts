using System;
using Antlr4.Runtime;

namespace prog_lang
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");

			string typescript = "let employeeName = \"John\";\n\tconsole.log(employeeName);";
			//ICharStream target = new AntlrInputStream(typescript);

			//ITokenSource lexer = new TypeScriptLexer(target);
			//ITokenStream tokens = new CommonTokenStream(lexer);
			//TypeScriptParser parser = new TypeScriptParser(tokens);

			//TypeScriptParser.LiteralContext result = parser.literal();
			//Console.Write(result.ToString());


			try
			{
				var stream = new AntlrInputStream(typescript);
				var lexer = new TypeScriptLexer(stream);
				var tokenStream = new CommonTokenStream(lexer);
				var parser = new TypeScriptParser(tokenStream);
				//parser.program();
				TypeScriptParser.ProgramContext programReturn = parser.program();

				var tree = programReturn.ToStringTree();
				Console.Write(tree);

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}

			Console.Write("Press any key to continue...");
			Console.ReadKey(true);
		}
	}
}
