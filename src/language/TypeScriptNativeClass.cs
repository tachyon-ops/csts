using System;
using System.Collections.Generic;
using TypeScriptNative.Interpret;

namespace TypeScriptNative.language
{
	public class TypeScriptNativeClass : TypeScriptNativeCallable
	{
		public String name;
		TypeScriptNativeClass superclass;
		private Dictionary<String, TypeScriptNativeFunction> methods;

		public TypeScriptNativeClass(String name, TypeScriptNativeClass superclass, Dictionary<String, TypeScriptNativeFunction> methods)
		{
			this.superclass = superclass;
			this.name = name;
			this.methods = methods;
		}

		public TypeScriptNativeFunction findMethod(String name)
		{
			if (methods.ContainsKey(name))
			{
				return methods[name];
			}

			if (superclass != null)
			{
				return superclass.findMethod(name);
			}

			return null;
		}

		int TypeScriptNativeCallable.arity()
		{
			throw new NotImplementedException();
		}

		object TypeScriptNativeCallable.call(Interpreter interpreter, List<object> arguments)
		{
			TypeScriptNativeInstance instance = new TypeScriptNativeInstance(this);
			TypeScriptNativeFunction initializer = findMethod("init");
			if (initializer != null)
			{
				initializer.bind(instance).call(interpreter, arguments);
			}
			return instance;
		}

		public String toString()
		{
			return "Class <" + name + ">";
		}
	}
}
