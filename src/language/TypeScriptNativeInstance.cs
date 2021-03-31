using System;
using System.Collections.Generic;
using TypeScriptNative.Scan;

namespace TypeScriptNative.language
{
	public class TypeScriptNativeInstance
	{

		private TypeScriptNativeClass klass;
		private Dictionary<String, Object> fields = new Dictionary<String, Object>();

		public TypeScriptNativeInstance(TypeScriptNativeClass klass)
		{
			this.klass = klass;
		}

		public Object get(Token name)
		{
			if (fields.ContainsKey(name.lexeme))
			{
				return fields[name.lexeme];
			}

			TypeScriptNativeFunction method = klass.findMethod(name.lexeme);
			if (method != null) return method.bind(this);

			throw new RuntimeError(name,
					"Undefined property '" + name.lexeme + "'.");
		}

		public void set(Token name, Object value)
		{
			// Since Lox allows freely creating new fields on instances,
			// there’s no need to see if the key is already present.
			fields.Add(name.lexeme, value);
		}


		public String toString()
		{
			return "Instance <" + klass.name + ">";
		}
	}
}
