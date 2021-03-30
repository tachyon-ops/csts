using System;
using TypeScriptNative.Scan;

namespace TypeScriptNative
{
	public class ErrorReport
	{
		public static bool hadError = false;
		public static bool hadRuntimeError = false;

		public ErrorReport()
		{
		}

		public static void error(int line, String message)
		{
			report(line, "", message);
		}

		public static void error(Token token, String message)
		{
			if (token.type == TokenType.EOF)
			{
				report(token.line, " at end", message);
			}
			else
			{
				report(token.line, " at '" + token.lexeme + "'", message);
			}
		}

		private static void report(int line, String where, String message)
		{
			Console.WriteLine("[line " + line + "] Error" + where + ": " + message);
			hadError = true;
		}

		public static void runtimeError(RuntimeError error)
		{
			System.Console.WriteLine(error.getMessage() +
					"\n[line " + error.token.line + "]");
			hadRuntimeError = true;
		}
	}
}
