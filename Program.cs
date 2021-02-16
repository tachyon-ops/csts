using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

using TypeScriptNative.src;

namespace prog_lang
{

	class Program
	{


		static int Main(string[] args)
		{
			Console.WriteLine("TS NATIVE!");

			string program = @"
function main(x: number, y: number) : number {
    return x + y;
}
main(10, 20);
";

			try
			{
				var stream = new AntlrInputStream(program);
				var lexer = new TypeScriptLexer(stream);
				var tokenStream = new CommonTokenStream(lexer);
				var parser = new TypeScriptParser(tokenStream);

				IParseTree tree = parser.program();
				Console.WriteLine("parse tree (LISP style): \n" + tree.ToStringTree(parser));

				parser.program();
				parser.RemoveErrorListeners();
				parser.AddErrorListener(new ThrowingErrorListener<IToken>());

				TypeScriptParserListener expressionWalker = new TypeScriptParserListener();
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
