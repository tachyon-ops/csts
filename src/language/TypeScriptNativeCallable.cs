using System;
using System.Collections.Generic;
using TypeScriptNative.Interpret;

namespace TypeScriptNative.language
{
	public interface TypeScriptNativeCallable
	{
		int arity();
		Object call(Interpreter interpreter, List<Object> arguments);
	}
}
