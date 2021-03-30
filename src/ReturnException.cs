using System;
namespace TypeScriptNative
{
	public class ReturnException : SystemException
	{
		public Object value;

		public ReturnException(Object value) : base(null, null)
		{
			//super(null, null, false, false);
			this.value = value;
		}
	}
}
