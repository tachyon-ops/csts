using System;
using System.Collections.Generic;
using TypeScriptNative.language;

namespace TypeScriptNative.globals
{
	public class Print : TypeScriptNativeCallable
	{
		public int arity()
		{
			return 1;
		}

		public Object call(Interpret.Interpreter interpreter,
						   List<Object> arguments)
		{
			Object value = "";
			foreach (var arg in arguments)
			{
				value += Utils.stringify(arg);
			}
			System.Console.Write(value);
			return value;
		}


		public String toString()
		{
			return "<native fn> Print";
		}
	}
}
