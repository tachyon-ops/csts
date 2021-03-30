using System;

using TypeScriptNative.Scan;

namespace TypeScriptNative
{
	public class RuntimeError : SystemException
	{
		public Token token;

		public RuntimeError(Token token, String message) : base(message)
		{
			this.token = token;
		}

		public String getMessage()
		{
			return this.Message;
		}
	}
}
