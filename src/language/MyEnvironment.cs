using System;
using System.Collections.Generic;
using TypeScriptNative.Scan;

namespace TypeScriptNative.language
{
	public class MyEnvironment
	{
		public MyEnvironment enclosing;
		private Dictionary<String, Object> values = new Dictionary<String, Object>();

		public MyEnvironment()
		{
			enclosing = null;
		}

		public MyEnvironment(MyEnvironment enclosing)
		{
			this.enclosing = enclosing;
		}

		public Object get(Token name)
		{
			if (values.ContainsKey(name.lexeme))
			{
				return values[name.lexeme];
			}

			if (enclosing != null) return enclosing.get(name);

			throw new RuntimeError(name,
					"Undefined variable '" + name.lexeme + "'."); //RuntimeError => SystemException
		}

		public bool KeyExistsSomewhere(Token name)
		{
			if (values.ContainsKey(name.lexeme)) return true;
			if (enclosing != null && enclosing.values.ContainsKey(name.lexeme)) return true;
			return false;
		}

		public void assign(Token name, Object value)
		{
			if (values.ContainsKey(name.lexeme))
			{
				values[name.lexeme] = value;
				return;
			}

			if (enclosing != null)
			{
				enclosing.assign(name, value);
				return;
			}

			throw new RuntimeError(name,
					"Undefined variable '" + name.lexeme + "'.");
		}

		public void define(String name, Object value)
		{
			values.Add(name, value);
		}

		public Object getAt(int distance, String name)
		{
			return ancestor(distance).values[name];
		}

		public void assignAt(int distance, Token name, Object value)
		{
			ancestor(distance).values[name.lexeme] = value;
		}

		public MyEnvironment ancestor(int distance)
		{
			MyEnvironment environment = this;
			for (int i = 0; i < distance; i++)
			{
				environment = environment.enclosing;
			}

			return environment;
		}
	}
}
