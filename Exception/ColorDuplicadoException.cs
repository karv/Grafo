using System;

namespace Graficas
{
	/// <summary>
	/// Color duplicado expection.
	/// </summary>
	[Serializable]
	public class ColorDuplicadoException : Exception
	{
		public ColorDuplicadoException ()
		{
		}

		public ColorDuplicadoException (string message)
			: base (message)
		{
		}

		public ColorDuplicadoException (string message, Exception inner)
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