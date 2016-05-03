using System;

namespace Graficas.Exception
{
	/// <summary>
	/// Color duplicado expection.
	/// </summary>
	[Serializable]
	public class ColorDuplicadoException : System.Exception
	{
		public ColorDuplicadoException ()
		{
		}

		public ColorDuplicadoException (string message)
			: base (message)
		{
		}

		public ColorDuplicadoException (string message, System.Exception inner)
			: base (message, inner)
		{
		}

		protected ColorDuplicadoException (System.Runtime.Serialization.SerializationInfo info,
		                                   System.Runtime.Serialization.StreamingContext context)
			: base (info,
			        context)
		{
		}
	}
}