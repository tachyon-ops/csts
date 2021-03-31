using System;

namespace TypeScriptNative.globals
{
	public static class GlobalSource
	{

		public static String Get()
		{
			return @"
class Console {
	log(message) {
		println(message);
	}
}

var console = new Console();
			";
		}
	}
}
