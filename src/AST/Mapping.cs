using System;
using System.Collections.Generic;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace TypeScriptNative.src.AST
{

	static class Mapping
	{
		// Program
		public static Program ProgramToAST(ParserRuleContext ctx, bool considerPosition = false)
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
						SourceElementsToAST((ParserRuleContext)c);
					}
				}
				else
					Console.WriteLine(c.ToString());
				//statements.Add(new Statement(child));
			}
			return new Program(statements);
		}

		public static List<Statement> SourceElementsToAST(ParserRuleContext ctx)
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
					statements.AddRange(SourceElementToAST((ParserRuleContext)c));
				}
			}
			return statements;
		}

		public static List<Statement> SourceElementToAST(ParserRuleContext ctx)
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
		public static Statement StatementToAST(ParserRuleContext ctx, bool considerPosition = false)
		{
			Console.WriteLine(ctx.GetType() + " children: " + ctx.ChildCount);

			if (ctx.ChildCount == 1)
			{
				ParserRuleContext child = (ParserRuleContext)ctx.children[0];
				int ruleIndex = child.RuleIndex;
				string ruleName = TypeScriptParser.ruleNames[ruleIndex];
				Console.WriteLine("StatementToAST: " + ruleName);
				if (ruleName == "functionDeclaration")
					return new Statement(FunctionToAST(child));
				else if (ruleName == "expressionStatement")
					return new Statement(ExpressionToAST(child));
				else if (ruleName == "returnStatement")
					return new Statement(ExpressionToAST(child), StatementType.RETURN);
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
		public static FunctionDeclaration FunctionToAST(ParserRuleContext ctx, bool considerPosition = false)
		{
			//Console.WriteLine(ctx.GetType() + " children: " + ctx.ChildCount);

			if (ctx.ChildCount < 6) throw new Exception("Function to AST has less than 6 children");

			Tuple<string, List<Parameter>> callSignature = new Tuple<string, List<Parameter>>(null, new List<Parameter>());
			foreach (var c in ctx.children)
			{
				//Console.WriteLine(c.GetType() + " val: " + c.GetText() + " children: " + c.ChildCount);

				if (c is ParserRuleContext)
				{
					var child = (ParserRuleContext)c;

					int ruleIndex = child.RuleIndex;
					string ruleName = TypeScriptParser.ruleNames[ruleIndex];

					//Console.WriteLine(ruleName);
					if (ruleName == "callSignature")
					{
						callSignature = CallSignatureToAST(child);
					}
					if (ruleName == "functionBody")
					{
						// get expression and return expression
						//functionBody =
						FunctionBodyToAST(ctx);
					}
				}
			}
			string functionIdentifer = ctx.children[1].GetText();
			return new FunctionDeclaration(functionIdentifer, callSignature.Item2, callSignature.Item1);
		}

		private static void FunctionBodyToAST(ParserRuleContext ctx, bool considerPosition = false)
		{
			foreach (var c in ctx.children)
			{
				Console.WriteLine(c.GetType() + " val: " + c.GetText() + " children: " + c.ChildCount);
				if (c is ParserRuleContext)
				{
					var child = (ParserRuleContext)c;

					int ruleIndex = child.RuleIndex;
					string ruleName = TypeScriptParser.ruleNames[ruleIndex];
					Console.WriteLine("Function body ruleName: " + ruleName);

					if (ruleName == "functionBody")
					{
						FunctionBodyStatements(child);
					}
				}
			}
		}

		private static List<Statement> FunctionBodyStatements(ParserRuleContext ctx, bool considerPosition = false)
		{
			foreach (var c in ctx.children)
			{
				Console.WriteLine(c.GetType() + " val: " + c.GetText() + " children: " + c.ChildCount);
				if (c is ParserRuleContext)
				{
					var child = (ParserRuleContext)c;

					int ruleIndex = child.RuleIndex;
					string ruleName = TypeScriptParser.ruleNames[ruleIndex];
					Console.WriteLine("Function body > functionBody > ruleName: " + ruleName);

					if (ruleName == "sourceElements")
					{
						return SourceElementsToAST(child);
					}
				}
			}
			return new List<Statement>();
		}

		private static Tuple<string, List<Parameter>> CallSignatureToAST(ParserRuleContext ctx, bool considerPosition = false)
		{
			string functionType = null;
			List<Parameter> parameters = new List<Parameter>();
			foreach (var c in ctx.children)
			{
				//Console.WriteLine(c.GetType() + " val: " + c.GetText() + " children: " + c.ChildCount);

				if (c is ParserRuleContext)
				{
					var child = (ParserRuleContext)c;

					int ruleIndex = child.RuleIndex;
					string ruleName = TypeScriptParser.ruleNames[ruleIndex];

					//Console.WriteLine("Call Signature: " + ruleName);
					if (ruleName == "parameterList")
					{
						parameters.AddRange(ParameterListToAST(child));
					}
					if (ruleName == "typeAnnotation")
					{
						functionType = GetFunctionType(child);
					}
				}
			}
			return new Tuple<string, List<Parameter>>(functionType, parameters);
		}

		private static string GetFunctionType(ParserRuleContext ctx, bool considerPosition = false)
		{
			return ctx.children[1].GetText();
		}

		private static List<Parameter> ParameterListToAST(ParserRuleContext ctx, bool considerPosition = false)
		{
			List<Parameter> parameters = new List<Parameter>();
			foreach (var c in ctx.children)
			{
				if (c is ParserRuleContext)
				{
					var child = (ParserRuleContext)c;
					int ruleIndex = child.RuleIndex;
					string ruleName = TypeScriptParser.ruleNames[ruleIndex];
					//Console.WriteLine("Call Signature > Parameter List: " + ruleName);
					if (ruleName == "parameter")
					{
						parameters.Add(ParameterToAST(child));
					}
				}
			}
			return parameters;
		}

		private static Parameter ParameterToAST(ParserRuleContext ctx, bool considerPosition = false)
		{
			foreach (var c in ctx.children)
			{
				if (c is ParserRuleContext)
				{
					var child = (ParserRuleContext)c;
					int ruleIndex = child.RuleIndex;
					string ruleName = TypeScriptParser.ruleNames[ruleIndex];
					//Console.WriteLine("Call Signature > Parameter List > Parameter: " + ruleName);
					if (ruleName == "requiredParameter")
					{
						return RequiredParameterToAST(child);
					}
				}
			}
			throw new Exception("No parameter present");
		}

		private static Parameter RequiredParameterToAST(ParserRuleContext ctx, bool considerPosition = false)
		{
			string identifier = null;
			string type = null;
			foreach (var c in ctx.children)
			{
				if (c is ParserRuleContext)
				{
					var child = (ParserRuleContext)c;
					int ruleIndex = child.RuleIndex;
					string ruleName = TypeScriptParser.ruleNames[ruleIndex];
					//Console.WriteLine("Call Signature > Parameter List > Parameter > RequiredParam: " + ruleName);

					if (ruleName == "identifierOrPattern")
						identifier = child.GetText();
					if (ruleName == "typeAnnotation")
						type = child.GetText();
				}
			}
			if (identifier != null && type != null)
			{
				return new Parameter(identifier, type);
			}
			throw new Exception("No identifier or type detected");
		}

		private static string ParamIdentifierOrPattern(ParserRuleContext ctx, bool considerPosition = false)
		{
			foreach (var c in ctx.children)
			{
				if (c is ParserRuleContext)
				{
					var child = (ParserRuleContext)c;
					int ruleIndex = child.RuleIndex;
					string ruleName = TypeScriptParser.ruleNames[ruleIndex];
					//Console.WriteLine("Call Signature > Parameter List > Parameter > ParamIdentifierOrPattern " + ruleName);
					Console.WriteLine("ruleName: " + ruleName);
					Console.WriteLine("text: " + child.GetText());
					return child.GetText();
				}
			}
			throw new Exception("No identifier name detected");
		}

		// Expression
		public static Expression ExpressionToAST(ParserRuleContext ctx, bool considerPosition = false)
		{
			//Console.WriteLine(ctx.GetType() + " children: " + ctx.ChildCount);

			foreach (var c in ctx.children)
			{
				//Console.WriteLine(c.GetType() + " val: " + c.GetText() + " children: " + c.ChildCount);

				if (c is ParserRuleContext)
				{
					var child = (ParserRuleContext)c;
					int ruleIndex = child.RuleIndex;
					string ruleName = TypeScriptParser.ruleNames[ruleIndex];
					Console.WriteLine("ExpressionToAST - ruleName: " + ruleName);
					Console.WriteLine("ExpressionToAST - text: " + child.GetText());
					//return child.GetText();

					if (ruleName == "expressionSequence")
					{
						ExpressionSequenceToAST(child);
					}
				}
			}
			return new Expression();
		}

		public static void ExpressionSequenceToAST(ParserRuleContext ctx, bool considerPosition = false)
		{
			foreach (var c in ctx.children)
			{
				//Console.WriteLine(c.GetType() + " val: " + c.GetText() + " children: " + c.ChildCount);

				if (c is ParserRuleContext)
				{
					var child = (ParserRuleContext)c;
					int ruleIndex = child.RuleIndex;
					string ruleName = TypeScriptParser.ruleNames[ruleIndex];
					Console.WriteLine("ExpressionToAST - ExpressionSequenceToAST - ruleName: " + ruleName);
					Console.WriteLine("ExpressionToAST - ExpressionSequenceToAST - text: " + child.GetText());

					if (ruleName == "singleExpression")
					{
						SingleExpresionToAST(child);
					}
				}
			}
		}

		public static void SingleExpresionToAST(ParserRuleContext ctx, bool considerPosition = false)
		{
			foreach (var c in ctx.children)
			{
				//Console.WriteLine(c.GetType() + " val: " + c.GetText() + " children: " + c.ChildCount);

				if (c is ParserRuleContext)
				{
					var child = (ParserRuleContext)c;
					int ruleIndex = child.RuleIndex;
					string ruleName = TypeScriptParser.ruleNames[ruleIndex];
					Console.WriteLine("ExpressionToAST - ExpressionSequenceToAST - SingleExpresionToAST - ruleName: " + ruleName);
					Console.WriteLine("ExpressionToAST - ExpressionSequenceToAST - SingleExpresionToAST - text: " + child.GetText());
				}
			}
		}
	}

}
