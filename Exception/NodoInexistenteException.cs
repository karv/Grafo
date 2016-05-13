using System;

namespace Graficas.Exception
{
	/// <summary>
	/// Exception de cuando se intenta accesar por un grafo a un nodo que no existe en él.
	/// </summary>
	[Serializable]
	public class NodoInexistenteException : System.Exception
	{
		/// <summary>
		/// 
		/// </summary>
		public NodoInexistenteException ()
		{
		}

		/// <param name="message">Message.</param>
		public NodoInexistenteException (string message)
			: base (message)
		{
		}

		/// <param name="message">Message.</param>
		/// <param name="inner">Inner.</param>
		public NodoInexistenteException (string message, System.Exception inner)
			: base (message, inner)
		{
		}

		/// <param name="info">Info.</param>
		/// <param name="context">Context.</param>
		protected NodoInexistenteException (System.Runtime.Serialization.SerializationInfo info,
		                                    System.Runtime.Serialization.StreamingContext context)
			: base (info, context)
		{
		}
	}
}