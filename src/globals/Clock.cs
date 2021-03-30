using System;
using System.Collections.Generic;
using TypeScriptNative.language;

namespace TypeScriptNative.globals
{
	public class Clock : TypeScriptNativeCallable
	{
		public int arity() { return 0; }

		public Object call(Interpret.Interpreter interpreter, List<Object> arguments)
		{
			return (double)System.DateTime.Now.Millisecond / 1000.0;
		}

		public String toString() { return "<native fn>"; }
	}
}
