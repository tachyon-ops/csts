using System;
namespace TypeScriptNative.src
{
	using AST;
	public interface IRunner
	{
		void Run(Program program);
		void RunStatement(Statement statement);
	}

}
