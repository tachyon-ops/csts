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
					"Get: Undefined variable '" + name.lexeme + "'."); //RuntimeError => SystemException
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
					"Assignment error: Undefined variable '" + name.lexeme + "'.");
		}

		public void define(String name, Object value)
		{
			values[name] = value;
		}

		public Object getAt(int distance, String name)
		{
			// TODO: Fix this as this is quite annoying
			// we are getting the wrong distance inside class methods (1 instead of 0)
			// such a fix would allow us to have only the following line:
			// return ancestor(distance).values[name];

			// Fix:
			var values = ancestor(distance).values;
			if (values.ContainsKey(name))
			{
				return values[name];
			}
			else
			{
				return getAtHelper(name);
			}
		}

		private Object getAtHelper(String name)
		{
			if (values.ContainsKey(name))
			{
				return values[name];
			}

			if (enclosing != null) return enclosing.getAtHelper(name);
			throw new RuntimeError(
				new Token(TokenType.IDENTIFIER, name, null, 0),
				"Undefined variable '" + name + "'."
				); //RuntimeError => SystemException
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
