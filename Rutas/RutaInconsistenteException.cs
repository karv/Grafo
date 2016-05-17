using System;

namespace Graficas.Rutas
{
	
	[Serializable]
	public class RutaInconsistenteException : InvalidOperationException
	{
		/// <summary>
		/// Initializes a new instance of this class
		/// </summary>
		public RutaInconsistenteException ()
		{
		}

		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		public RutaInconsistenteException (string message)
			: base (message)
		{
		}

		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		/// <param name="inner">The exception that is the cause of the current exception. </param>
		public RutaInconsistenteException (string message, System.Exception inner)
			: base (message,
			        inner)
		{
		}

		/// <param name="context">The contextual information about the source or destination.</param>
		/// <param name="info">The object that holds the serialized object data.</param>
		protected RutaInconsistenteException (System.Runtime.Serialization.SerializationInfo info,
		                                      System.Runtime.Serialization.StreamingContext context)
			: base (info,
			        context)
		{
		}
	}
}

