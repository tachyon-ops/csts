using System;
namespace TypeScriptNative.src
{
	using AST;
	public class Compiler : IRunner
	{
		int forLevel = 0;

		public void Run(Program program)
		{
			Console.WriteLine("Compiling program " + program.statements.Count);
			this.forLevel = 0;
			foreach (var statement in program.statements)
			{
				RunStatement(statement);
			}
		}

		public void RunStatement(Statement statement)
		{
			++this.forLevel;
			Console.WriteLine(GetPad() + " statement");
		}

		public string GetPad()
		{
			return "".PadLeft(forLevel + 4);
		}
	}
}
