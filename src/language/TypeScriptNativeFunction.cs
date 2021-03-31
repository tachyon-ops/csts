using System;
using System.Collections.Generic;
using TypeScriptNative.AST;
using TypeScriptNative.Interpret;

namespace TypeScriptNative.language
{
	public class TypeScriptNativeFunction : TypeScriptNativeCallable
	{
		private Function declaration;
		private MyEnvironment closure;

		private bool isInitializer;

		public TypeScriptNativeFunction(Function declaration, MyEnvironment closure, bool isInitializer)
		{
			this.isInitializer = isInitializer;
			this.closure = closure;
			this.declaration = declaration;
		}

		public TypeScriptNativeFunction bind(TypeScriptNativeInstance instance)
		{
			MyEnvironment environment = new MyEnvironment(closure);
			environment.define("this", instance);
			return new TypeScriptNativeFunction(declaration, environment, isInitializer);
		}

		public int arity()
		{
			return declaration.myParams.Count;
		}

		public object call(Interpreter interpreter, List<object> arguments)
		{
			MyEnvironment environment = new MyEnvironment(closure);
			for (int i = 0; i < declaration.myParams.Count; i++)
			{
				environment.define(declaration.myParams[i].lexeme,
						arguments[i]);
			}
			try
			{
				interpreter.executeBlock(declaration.body, environment);
			}
			catch (ReturnException returnValue)
			{
				// If we’re in an initializer and execute a return statement,
				// instead of returning the value (which will always be nil), we again return this.
				if (isInitializer) return closure.getAt(0, "this");
				return returnValue.value;
			}
			if (isInitializer) return closure.getAt(0, "this");
			return null;
		}

		public String toString()
		{
			return "<fn " + declaration.name.lexeme + ">";
		}
	}
}
