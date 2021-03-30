using System;
namespace TypeScriptNative.globals
{
	public class Utils
	{

		public static String stringify(Object myObject)
		{
			if (myObject == null) return "nil";

			if (myObject is Double) {
				String text = myObject.ToString();
				if (text.EndsWith(".0"))
				{
					text = text.Substring(0, text.Length - 2);
				}
				return text;
			}

			return myObject.ToString();
		}
	}
}
