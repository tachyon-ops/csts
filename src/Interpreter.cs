using System;
using System.Collections.Generic;

namespace TypeScriptNative.src
{
	using AST;
	public class Interpreter : IRunner
	{
		int forLevel = 0;

		private readonly Stack<string> stack = new Stack<string>();

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
			--this.forLevel;
		}

		public void RunFunctionDeclaration(FunctionDeclaration functionDeclaration)
		{
			++this.forLevel;
			string parameters = "";
			foreach (Parameter param in functionDeclaration.parameters)
			{
				parameters = parameters + " PARAM: " + param.varName + " " + param.type + " optional: " + (param.optional ? "true" : "false");
			}
			Console.WriteLine(GetPad() + " functionDeclaration " + functionDeclaration.functionName + "(" + parameters + ") => " + functionDeclaration.type);
			--this.forLevel;
		}

		public void RunIntegerLiteral(IntegerLiteral integer)
		{
			++this.forLevel;
			Console.WriteLine(GetPad() + " integer " + integer.value);
			--this.forLevel;
		}

		public string GetPad()
		{
			return "".PadLeft(forLevel * 2);
		}
	}
}
