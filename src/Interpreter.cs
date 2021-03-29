using System;
using System.Collections.Generic;

namespace TypeScriptNative.src
{
	using AST;
	public class Interpreter : IRunner
	{
		int forLevel = 0;

		private readonly Stack<string> stack = new Stack<string>();

		private readonly Dictionary<string, INode> GLOBAL_SCOPE = new Dictionary<string, INode>();

		public void Run(Program program)
		{
			Console.WriteLine("Interpreting program " + program.statements.Count);
			this.forLevel = 0;
			foreach (var statement in program.statements)
			{
				RunStatement(statement);
			}
		}

		public void RunStatement(Statement statement)
		{
			++this.forLevel;
			Console.WriteLine(GetPad() + " statement " + statement.child + " " + statement.type);
			if (statement.child is FunctionDeclaration)
				RunFunctionDeclaration((FunctionDeclaration)statement.child);
			if (statement.child is IntegerLiteral)
				RunIntegerLiteral((IntegerLiteral)statement.child);
			if (statement.child is FunctionCall)
				RunFunctionCall((FunctionCall)statement.child);
			if (statement.child is SumExpression)
				RunSumExpressions((SumExpression)statement.child);
			--this.forLevel;
		}

		public void RunFunctionDeclaration(FunctionDeclaration functionDeclaration)
		{
			++this.forLevel;
			this.GLOBAL_SCOPE.Add(functionDeclaration.functionName, functionDeclaration);
			//string parameters = "";
			//foreach (Parameter param in functionDeclaration.parameters)
			//{
			//	parameters = parameters + " PARAM: " + param.varName + " " + param.type + " optional: " + (param.optional ? "true" : "false");
			//}
			//Console.WriteLine(GetPad() + " functionDeclaration " + functionDeclaration.functionName + "(" + parameters + ") => " + functionDeclaration.type);
			//foreach (var statement in functionDeclaration.bodyStatements)
			//{
			//	Console.WriteLine(GetPad() + "     > function body statement: " + statement.type);
			//	RunStatement(statement);
			//}
			--this.forLevel;
		}

		public void RunIntegerLiteral(IntegerLiteral integer)
		{
			++this.forLevel;
			Console.WriteLine(GetPad() + " integer " + integer.value);
			--this.forLevel;
		}

		public void RunFunctionCall(FunctionCall function)
		{
			++this.forLevel;

			Console.WriteLine(GetPad() + " function: " + function.functionName);
			foreach (var param in function.parameters)
			{
				if (param is IntegerLiteral)
				{
					IntegerLiteral integer = (IntegerLiteral)param;
					Console.WriteLine(GetPad() + "  |-> param: " + integer.value);
				}
			}

			// get function from current scope
			if (this.GLOBAL_SCOPE[function.functionName] != null)
			{
				if (this.GLOBAL_SCOPE[function.functionName] is FunctionDeclaration)
				{
					var functionDec = (FunctionDeclaration)this.GLOBAL_SCOPE[function.functionName];
					var func = (FunctionDeclaration)functionDec;
					Console.WriteLine("RunFunction " + func.functionName);
					var i = 0;
					foreach (var param in function.parameters)
					{
						if (param is IntegerLiteral)
							this.GLOBAL_SCOPE.Add(functionDec.parameters[i].varName, (IntegerLiteral)param);
						else
							throw new Exception("Function type param not implemented: " + param.GetType());
						i++;
					}

					// call function
					foreach (var statement in functionDec.bodyStatements)
					{
						Console.WriteLine("Body statement: " + statement);
						if (statement.type == StatementType.RETURN)
						{
							Console.WriteLine("Return function: ");
							RunStatement(statement);
						}
					}
				}

				--this.forLevel;
			}
		}

		public void RunSumExpressions(SumExpression expression)
		{
			++this.forLevel;
			Console.WriteLine(GetPad() + " SumExpression > " + expression + " " + expression.left + " " + expression.right);
			RunVarRef((VarReference)expression.left);
			RunVarRef((VarReference)expression.right);
			// left val
			var leftRef = (VarReference)expression.left;
			var left = (IntegerLiteral)this.GLOBAL_SCOPE[leftRef.varName];
			// right val
			var rightRef = (VarReference)expression.right;
			var right = (IntegerLiteral)this.GLOBAL_SCOPE[rightRef.varName];

			int leftVal = int.Parse(left.value);
			int rightVal = int.Parse(right.value);
			Console.WriteLine("RESULT: " + (leftVal + rightVal));

			--this.forLevel;
		}

		public void RunVarRef(VarReference var)
		{
			++this.forLevel;
			Console.WriteLine(GetPad() + " VarReference > " + var.varName);
			--this.forLevel;
		}

		public string GetPad()
		{
			return "".PadLeft(forLevel * 2);
		}
	}
}
