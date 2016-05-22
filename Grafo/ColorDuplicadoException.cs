using System;

namespace Graficas.Grafo
{
	/// <summary>
	/// Color duplicado expection.
	/// </summary>
	[Serializable]
	public class ColorDuplicadoException : Exception
	{
		/// <summary>
		/// 
		/// </summary>
		public ColorDuplicadoException ()
		{
		}

		/// <param name="message">Message.</param>
		public ColorDuplicadoException (string message)
			: base (message)
		{
		}

		/// <param name="message">Message.</param>
		/// <param name="inner">Inner.</param>
		public ColorDuplicadoException (string message, Exception inner)
			: base (message, inner)
		{
		}

		/// <param name="info">Info.</param>
		/// <param name="context">Context.</param>
		protected ColorDuplicadoException (System.Runtime.Serialization.SerializationInfo info,
		                                   System.Runtime.Serialization.StreamingContext context)
			: base (info,
			        context)
		{
		}
	}
}