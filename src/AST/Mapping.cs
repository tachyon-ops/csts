using System;
using System.Collections.Generic;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace TypeScriptNative.src.AST
{

	static class Mapping
	{
		// Program
		public static Program ProgramToAST(ParserRuleContext ctx)
		{
			return ProgramToAST(ctx, false);
		}

		public static Program ProgramToAST(ParserRuleContext ctx, bool considerPosition)
		{
			List<Statement> statements = new List<Statement>();

			Console.WriteLine("Program toAST: " + ctx.ChildCount + " type: " + ctx.GetType());


			foreach (var c in ctx.children)
			{

				if (c is ParserRuleContext)
				{
					int ruleIndex = ((ParserRuleContext)c).RuleIndex;
					string ruleName = TypeScriptParser.ruleNames[ruleIndex];
					Console.WriteLine(ruleName);

					if (ruleName == "sourceElements")
					{
						ExplodeSourceElements((ParserRuleContext)c);
					}
				}
				else
					Console.WriteLine(c.ToString());
				//statements.Add(new Statement(child));
			}

			return new Program(statements);
		}

		public static List<Statement> ExplodeSourceElements(ParserRuleContext ctx)
		{
			List<Statement> statements = new List<Statement>();
			// loop those
			foreach (var c in ctx.children)
			{
				int ruleIndex = ((ParserRuleContext)c).RuleIndex;
				string ruleName = TypeScriptParser.ruleNames[ruleIndex];
				Console.WriteLine(ruleName);

				if (ruleName == "sourceElement")
				{
					statements.AddRange(ExplodeSourceElement((ParserRuleContext)c));
				}
			}
			return statements;
		}

		public static List<Statement> ExplodeSourceElement(ParserRuleContext ctx)
		{
			List<Statement> statements = new List<Statement>();
			// loop those
			foreach (var c in ctx.children)
			{
				int ruleIndex = ((ParserRuleContext)c).RuleIndex;
				string ruleName = TypeScriptParser.ruleNames[ruleIndex];
				Console.WriteLine(ruleName);

				if (c is ParserRuleContext && ruleName == "statement")
				{
					statements.Add(StatementToAST((ParserRuleContext)c));
				}
			}
			return statements;
		}


		// Statement
		public static Statement StatementToAST(ParserRuleContext ctx)
		{
			return StatementToAST(ctx, false);
		}

		public static Statement StatementToAST(ParserRuleContext ctx, bool considerPosition)
		{
			Console.WriteLine(ctx.GetType() + " children: " + ctx.ChildCount);

			if (ctx.ChildCount == 1)
			{
				ParserRuleContext child = (ParserRuleContext)ctx.children[0];
				int ruleIndex = child.RuleIndex;
				string ruleName = TypeScriptParser.ruleNames[ruleIndex];
				Console.WriteLine(ruleName);
				if (ruleName == "functionDeclaration")
					return new Statement(FunctionToAST(child));
				else if (ruleName == "expressionStatement")
					return new Statement(ExpressionToAST(child)); // ToDO: ExpressionToAST(child)
			}
			else
			{
				foreach (var child in ctx.children)
				{
					Console.WriteLine("Should a Statement have 2 children?");
				}
			}
			return new Statement();
		}

		// Function
		public static FunctionDeclaration FunctionToAST(ParserRuleContext ctx)
		{
			return FunctionToAST(ctx, false);
		}

		public static FunctionDeclaration FunctionToAST(ParserRuleContext ctx, bool considerPosition)
		{
			Console.WriteLine(ctx.GetType() + " children: " + ctx.ChildCount);

			foreach (var child in ctx.children)
			{
				Console.WriteLine(child.GetType() + " val: " + child.GetText() + " children: " + child.ChildCount);
			}
			string functionIdentifer = ctx.children[1].GetText();
			string callSignature = ctx.children[2].GetText();
			// - (
			// - parameterList
			// - )
			// - typeAnnotation

			string functionBodyContext = ctx.children[4].GetText();
			// - statements inside
			// - return statement

			return new FunctionDeclaration();
		}

		// Expression
		public static Expression ExpressionToAST(ParserRuleContext ctx)
		{
			return ExpressionToAST(ctx, false);
		}

		public static Expression ExpressionToAST(ParserRuleContext ctx, bool considerPosition)
		{
			Console.WriteLine(ctx.GetType() + " children: " + ctx.ChildCount);

			foreach (var child in ctx.children)
			{
				Console.WriteLine(child.GetType() + " val: " + child.GetText() + " children: " + child.ChildCount);
			}
			return new Expression();
		}
	}

}
