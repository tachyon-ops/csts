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
			//Console.WriteLine("Program toAST: " + ctx.ChildCount + " type: " + ctx.GetType());
			foreach (var c in ctx.children)
			{
				if (c is ParserRuleContext)
				{
					var child = (ParserRuleContext)c;
					int ruleIndex = child.RuleIndex;
					string ruleName = TypeScriptParser.ruleNames[ruleIndex];
					Console.WriteLine(ruleName);
					if (ruleName == "sourceElements")
					{
						statements.AddRange(SourceElementsToAST(child));
					}
				}
				else
					Console.WriteLine(c.ToString());
				//statements.Add(new Statement(child));
			}
			Program program = new Program();
			program.AddStatements(statements);
			return program;
		}

		public static List<Statement> SourceElementsToAST(ParserRuleContext ctx)
		{
			List<Statement> statements = new List<Statement>();
			// loop those
			foreach (var c in ctx.children)
			{
				int ruleIndex = ((ParserRuleContext)c).RuleIndex;
				string ruleName = TypeScriptParser.ruleNames[ruleIndex];
				//Console.WriteLine(ruleName);

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
				//Console.WriteLine(ruleName);

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
			//Console.WriteLine(ctx.GetType() + " children: " + ctx.ChildCount);

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

			string type = null;
			List<Parameter> parameters = new List<Parameter>();
			List<Statement> bodyStatements = new List<Statement>();
			foreach (var c in ctx.children)
			{
				Console.WriteLine("FunctionToAST: " + c.GetType() + " val: " + c.GetText() + " children: " + c.ChildCount);

				if (c is ParserRuleContext)
				{
					var child = (ParserRuleContext)c;

					int ruleIndex = child.RuleIndex;
					string ruleName = TypeScriptParser.ruleNames[ruleIndex];

					//Console.WriteLine(ruleName);
					if (ruleName == "callSignature")
					{
						var callSignature = CallSignatureToAST(child);
						type = callSignature.Item1;
						parameters = callSignature.Item2;
					}
					if (ruleName == "functionBody")
					{
						// get expression and return expression
						//functionBody =
						bodyStatements.AddRange(FunctionBodyToAST(ctx));
					}
				}
			}
			string functionIdentifer = ctx.children[1].GetText();
			return new FunctionDeclaration(functionIdentifer, parameters, bodyStatements, type);
		}

		private static List<Statement> FunctionBodyToAST(ParserRuleContext ctx, bool considerPosition = false)
		{
			List<Statement> bodyStatements = new List<Statement>();
			foreach (var c in ctx.children)
			{
				//Console.WriteLine(c.GetType() + " val: " + c.GetText() + " children: " + c.ChildCount);
				if (c is ParserRuleContext)
				{
					var child = (ParserRuleContext)c;

					int ruleIndex = child.RuleIndex;
					string ruleName = TypeScriptParser.ruleNames[ruleIndex];
					//Console.WriteLine("Function body ruleName: " + ruleName);

					if (ruleName == "functionBody")
					{
						bodyStatements.AddRange(FunctionBodyStatements(child));
					}
				}
			}
			return bodyStatements;
		}

		private static List<Statement> FunctionBodyStatements(ParserRuleContext ctx, bool considerPosition = false)
		{
			List<Statement> bodyStatements = new List<Statement>();
			foreach (var c in ctx.children)
			{
				//Console.WriteLine(c.GetType() + " val: " + c.GetText() + " children: " + c.ChildCount);
				if (c is ParserRuleContext)
				{
					var child = (ParserRuleContext)c;

					int ruleIndex = child.RuleIndex;
					string ruleName = TypeScriptParser.ruleNames[ruleIndex];
					//Console.WriteLine("Function body > functionBody > ruleName: " + ruleName);

					if (ruleName == "sourceElements")
					{
						bodyStatements.AddRange(SourceElementsToAST(child));
					}
				}
			}
			return bodyStatements;
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
					//Console.WriteLine("ParamIdentifierOrPattern: " + ruleName + " >> " + child.GetText());
					return child.GetText();
				}
			}
			throw new Exception("No identifier name detected");
		}

		// Expression
		public static Expression ExpressionToAST(ParserRuleContext ctx, bool considerPosition = false)
		{
			//Console.WriteLine(ctx.GetType() + " children: " + ctx.ChildCount);

			List<Expression> expressions = new List<Expression>();

			foreach (var c in ctx.children)
			{
				//Console.WriteLine(c.GetType() + " val: " + c.GetText() + " children: " + c.ChildCount);

				if (c is ParserRuleContext)
				{
					var child = (ParserRuleContext)c;
					int ruleIndex = child.RuleIndex;
					string ruleName = TypeScriptParser.ruleNames[ruleIndex];
					Console.WriteLine("ExpressionToAST: " + ruleName + " >> " + child.GetText());
					if (ruleName == "expressionSequence")
					{
						expressions.AddRange(ExpressionSequenceToAST(child));
						//Console.WriteLine(">>>>> expressions count: " + expressions.Count);
						if (expressions.Count == 1) return expressions[0];
						else Console.WriteLine("Expressions count is superior to 1");
					}
					else if (ruleName == "eos")
						return null;
				}
			}
			throw new Exception("ExpressionToAST Exception > could not get ExpressionSequence");
		}

		public static List<Expression> ExpressionSequenceToAST(ParserRuleContext ctx, bool considerPosition = false)
		{
			List<Expression> expressions = new List<Expression>();
			foreach (var c in ctx.children)
			{
				Console.WriteLine(c.GetType() + " val: " + c.GetText() + " children: " + c.ChildCount);

				if (c is ParserRuleContext)
				{
					var child = (ParserRuleContext)c;
					int ruleIndex = child.RuleIndex;
					string ruleName = TypeScriptParser.ruleNames[ruleIndex];
					Console.WriteLine("ExpressionSequenceToAST: " + ruleName + " >> " + child.GetText());
					if (ruleName == "singleExpression")
					{
						Expression expression = SingleExpresionToAST(child);
						Console.WriteLine("Expression type: " + expression);
						expressions.Add(expression);
					}
				}
			}
			return expressions;
			//throw new Exception("ExpressionSequenceToAST Exception > could not return Single Expression To AST");
		}

		public static Expression SingleExpresionToAST(ParserRuleContext ctx, bool considerPosition = false)
		{
			List<Expression> expressions = new List<Expression>();
			//Console.WriteLine(">>>>> " + ctx.GetType() + " val: " + ctx.GetText() + " children: " + ctx.ChildCount);

			if (ctx is TypeScriptParser.IdentifierNameContext)
			{
				Console.WriteLine("will return var reference!");
				return new VarReference(ctx.GetText());
			}

			if (ctx is TypeScriptParser.LiteralContext)
				return SingleExpresionToAST((ParserRuleContext)ctx.children[0]);
			if (ctx is TypeScriptParser.NumericLiteralContext)
				return new IntegerLiteral(ctx.GetText());

			Expression left = null;
			Expression right = null;
			Operation operation = Operation.UNKNOWN;

			bool isFunction = false;
			List<Expression> functionParameters = new List<Expression>();
			string identifier = null;

			foreach (var c in ctx.children)
			{
				if (c is ParserRuleContext)
				{
					var child = (ParserRuleContext)c;
					int ruleIndex = child.RuleIndex;
					string ruleName = TypeScriptParser.ruleNames[ruleIndex];
					Console.WriteLine("SingleExpresion To AST: " + ruleName + " >> " + child.GetText());

					if (ruleName == "identifierName")
					{
						identifier = child.GetText();
						left = new VarReference(identifier);
					}
					else if (ruleName == "singleExpression")
					{
						if (child.GetText().Contains("+"))
						{
							operation = Operation.ADDITION;
							right = GetExpressionsToAST(child);
						}
						else
						{
							//Console.WriteLine("CHILD COUNT: " + child.ChildCount);
							if (child.ChildCount == 1)
								return GetExpressionsToAST2(child);
							else if (child.ChildCount > 1)
							{

								if (child.children[0].GetText() == "(" && child.children[2].GetText() == ")")
									isFunction = true;

								List<Expression> expressions2 = new List<Expression>();
								foreach (var child2 in child.children)
								{
									//Console.WriteLine("Single Expression To AST: " + child2.GetType() + " >> " + child2.GetText());
									if (child2 is TypeScriptParser.ExpressionSequenceContext && child2 is ParserRuleContext)
									{
										if (isFunction)
											functionParameters = GetFunctionParametersToAST((ParserRuleContext)child2, considerPosition);
									}
								}
							}
							else
							{
								throw new Exception("Single Expression To AST Exception in ruleName 'singleExpression' > is it a new operation?");
							}
						}
					}
				}
			}

			if (isFunction && identifier != null)
			{
				return new FunctionCall(identifier, functionParameters);
			}
			//Console.WriteLine("expressions count: " + expressions.Count);
			//Console.WriteLine("left: " + left.ToString());
			//Console.WriteLine("right: " + right.ToString());
			if (left != null && right != null)
			{
				if (operation == Operation.ADDITION)
					return new SumExpression(left, right);
			}
			throw new Exception("Single Expression To AST Exception > could not get left or right");
		}

		public static List<Expression> GetFunctionParametersToAST(ParserRuleContext ctx, bool considerPosition = false)
		{
			int ruleIndex = ctx.RuleIndex;
			string ruleName = TypeScriptParser.ruleNames[ruleIndex];
			//Console.WriteLine("ruleName: " + ruleName + " type: " + ctx.GetType() + " val: " + ctx.GetText() + " children: " + ctx.ChildCount);

			List<Expression> expressions = new List<Expression>();
			foreach (var child in ctx.children)
			{
				//Console.WriteLine("Parameter val: " + child.GetText());
				//Console.WriteLine("type: " + child.GetType() + " val: " + child.GetText() + " children: " + child.ChildCount);
				if (child is TypeScriptParser.LiteralExpressionContext)
				{
					expressions.Add(GetLiteralExpressionToAST((TypeScriptParser.LiteralExpressionContext)child));
				}
			}
			return expressions;
		}

		public static Expression GetLiteralExpressionToAST(TypeScriptParser.LiteralExpressionContext ctx, bool considerPosition = false)
		{
			if (ctx.children.Count > 1) throw new Exception("GetLiteralExpressionToAST ctx has more than 1 child");
			//Console.WriteLine("GetLiteralExpressionToAST >> type: " + ctx.GetType() + " val: " + ctx.GetText() + " children: " + ctx.ChildCount);
			if (ctx.children[0] is TypeScriptParser.LiteralContext)
				return GetLiteralToAST((TypeScriptParser.LiteralContext)ctx.children[0]);
			throw new Exception("There was an error retrieving LiteralExpression from GetLiteralExpressionToAST");
		}

		public static Expression GetLiteralToAST(TypeScriptParser.LiteralContext ctx, bool considerPosition = false)
		{
			if (ctx.children.Count > 1) throw new Exception("GetLiteralToAST ctx has more than 1 child");
			//Console.WriteLine("GetLiteralToAST >> type: " + ctx.GetType() + " val: " + ctx.GetText() + " children: " + ctx.ChildCount);
			if (ctx.children[0] is TypeScriptParser.NumericLiteralContext)
				return GetNumericalLiteralToAST((TypeScriptParser.NumericLiteralContext)ctx.children[0]);
			throw new Exception("There was an error retrieving Literal from GetLiteralToAST");
		}

		public static IntegerLiteral GetNumericalLiteralToAST(TypeScriptParser.NumericLiteralContext ctx, bool considerPosition = false)
		{
			if (ctx.children.Count > 1) throw new Exception("GetNumericalLiteralToAST ctx has more than 1 child");
			//Console.WriteLine("GetNumericalLiteralToAST >> type: " + ctx.GetType() + " val: " + ctx.GetText() + " children: " + ctx.ChildCount);
			foreach (var c in ctx.children)
			{
				//Console.WriteLine("GetNumericalLiteralToAST >> type: " + c.GetType() + " val: " + c.GetText() + " children: " + c.ChildCount);
				return new IntegerLiteral(c.GetText());
			}
			throw new Exception("There was an error retrieving Numecial Literal from GetNumericalLiteralToAST");
		}

		public static Expression GetExpressionsToAST2(ParserRuleContext ctx, bool considerPosition = false)
		{
			foreach (var c in ctx.children)
			{
				//Console.WriteLine(c.GetType() + " val: " + c.GetText() + " children: " + c.ChildCount);
				if (c is ParserRuleContext)
				{
					var child = (ParserRuleContext)c;
					//int ruleIndex = child.RuleIndex;
					//string ruleName = TypeScriptParser.ruleNames[ruleIndex];
					//Console.WriteLine("GetExpressionsToAST2: " + ruleName + " >> " + child.GetText());
					//if (ruleName == "singleExpression")
					//{
					//	//return SingleExpresionToAST(ctx);
					//}
					return SingleExpresionToAST(child);
				}
			}
			throw new Exception("GetExpressionsToAST Exception > could not get expression");
		}

		public static Expression GetExpressionsToAST(ParserRuleContext ctx, bool considerPosition = false)
		{
			List<Expression> expressions = new List<Expression>();
			foreach (var c in ctx.children)
			{
				//Console.WriteLine(c.GetType() + " val: " + c.GetText() + " children: " + c.ChildCount);

				if (c is ParserRuleContext)
				{
					var child = (ParserRuleContext)c;
					int ruleIndex = child.RuleIndex;
					string ruleName = TypeScriptParser.ruleNames[ruleIndex];
					Console.WriteLine("GetExpressionsToAST: " + ruleName + " >> " + child.GetText());
					if (ruleName == "singleExpression")
					{
						return SingleExpresionToAST(ctx);
					}
				}
			}
			throw new Exception("GetExpressionsToAST Exception > could not get expression");
		}
	}

}
