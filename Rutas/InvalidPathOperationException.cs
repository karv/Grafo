using System;

namespace Graficas.Rutas
{
	/// <summary>
	/// Occurs when an invalid path operation is executed.
	/// </summary>
	[Serializable]
	public class InvalidPathOperationException : InvalidOperationException
	{
		/// <summary>
		/// </summary>
		public InvalidPathOperationException ()
		{
		}

		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		public InvalidPathOperationException (string message)
			: base (message)
		{
		}

		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		/// <param name="inner">The exception that is the cause of the current exception. </param>
		public InvalidPathOperationException (string message, Exception inner)
			: base (message,
							inner)
		{
		}

		/// <param name="context">The contextual information about the source or destination.</param>
		/// <param name="info">The object that holds the serialized object data.</param>
		protected InvalidPathOperationException (System.Runtime.Serialization.SerializationInfo info,
																					System.Runtime.Serialization.StreamingContext context)
			: base (info,
							context)
		{
		}
	}
}