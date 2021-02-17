using System;
using System.Collections.Generic;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace TypeScriptNative.src
{
	using AST;

	public class TypeScriptParserListener : IParseTreeListener
	{
		int forLevel = 0;

		private readonly Stack<string> descentStack = new Stack<string>();

		//private readonly Stack<ASTContext> ascentStack = new Stack<ASTContext>();

		public TypeScriptParserListener()
		{
			// nothing
		}

		public void EnterEveryRule(ParserRuleContext ctx)
		{
			this.forLevel++;
			var ruleName = TypeScriptParser.ruleNames[ctx.RuleIndex];
			this.descentStack.Push(ruleName);

			bool keepRule = ctx.ChildCount > 1;
			if (!keepRule) return;
			Console.WriteLine("".PadLeft(forLevel + 4) + "Enter rule " + ruleName);
			//printInfo("EnterEveryRule", ctx);
		}

		public void ExitEveryRule(ParserRuleContext ctx)
		{
			this.forLevel--;
			string ruleName = this.descentStack.Pop();

			bool keepRule = ctx.ChildCount > 1;
			if (!keepRule) return;
			Console.WriteLine("".PadLeft(forLevel + 4) + "Exit rule " + ruleName);
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
				//Console.WriteLine("\nExplore CST:");
				//Explore(ctx);
				Console.WriteLine("\nExplore pre-AST (we can use this):");
				ExploreAST(ctx);

				Mapping.ProgramToAST(ctx);
			}
		}

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
}
