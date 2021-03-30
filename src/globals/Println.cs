using System;
using System.Collections.Generic;
using TypeScriptNative.language;

namespace TypeScriptNative.globals
{
	public class Println : TypeScriptNativeCallable
	{
		public int arity()
		{
			return 1;
		}

		public Object call(Interpret.Interpreter interpreter,
						   List<Object> arguments)
		{
			Object value = arguments[0];
			System.Console.WriteLine(Utils.stringify(value));
			return value;
		}


		public String toString()
		{
			return "<native fn> Print";
		}
	}
}
