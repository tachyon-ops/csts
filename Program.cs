using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

using TypeScriptNative.src;

namespace prog_lang
{

	class TreeListner : IParseTreeListener
	{
		int forLevel = 0;

		public void EnterEveryRule(ParserRuleContext ctx)
		{
			this.forLevel++;
			printInfo("EnterEveryRule", ctx);
		}

		public void ExitEveryRule(ParserRuleContext ctx)
		{
			this.forLevel--;
			printInfo("ExitEveryRule", ctx);
		}

		public void VisitErrorNode(IErrorNode node)
		{

		}

		public void VisitTerminal(ITerminalNode node)
		{

		}

		private void printInfo(String forType, ParserRuleContext ctx)
		{
			if (this.forLevel == 0)
			{
				Console.WriteLine("\nExplore CST:");
				Explore(ctx);
				Console.WriteLine("\nExplore pre-AST (we can use this):");
				ExploreAST(ctx);
			}
		}

		// http://franckgaspoz.fr/en/first-steps-with-antlr4-in-csharp/
		void Explore(ParserRuleContext ctx, int indentLevel = 0)
		{
			var ruleName = TypeScriptParser.ruleNames[ctx.RuleIndex];
			var sep = "".PadLeft(indentLevel);
			Console.WriteLine(sep + ruleName);
			sep = "".PadLeft(indentLevel + 4);
			foreach (var c in ctx.children)
			{
				if (c is ParserRuleContext)
					Explore((ParserRuleContext)c, indentLevel + 4);
				else
					Console.WriteLine(sep + c.ToString());
			}
		}

		void ExploreAST(ParserRuleContext ctx, int indentLevel = 0)
		{
			var ruleName = TypeScriptParser.ruleNames[ctx.RuleIndex];
			var sep = "".PadLeft(indentLevel);
			bool keepRule = ctx.ChildCount > 1;
			if (keepRule)
				Console.WriteLine(sep + ruleName);
			foreach (var c in ctx.children)
			{
				if (c is ParserRuleContext)
					ExploreAST((ParserRuleContext)c, indentLevel + ((keepRule) ? 4 : 0));
				else
				{
					var sep2 =
						"".PadLeft(indentLevel + ((keepRule) ? 4 : 0));
					Console.WriteLine(sep2 + c.ToString());
				}
			}
		}
	}

	class Program
	{


		static int Main(string[] args)
		{
			Console.WriteLine("TS NATIVE!");

			string program = @"
let a = 1;
let b = 2;
print(a + b);
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

				// https://stackoverflow.com/questions/63542345/how-to-detect-for-loop-block-after-parsing-the-code-using-antlr
				TreeListner expressionWalker = new TreeListner();
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
