using System;

namespace CE.Graph.Grafo
{
	/// <summary>
	/// Exception de cuando se intenta accesar por un grafo a un nodo que no existe en él.
	/// </summary>
	[Serializable]
	public class NonExistentNodeException : Exception
	{
		/// <summary>
		/// 
		/// </summary>
		public NonExistentNodeException ()
		{
		}

		/// <param name="message">Message.</param>
		public NonExistentNodeException (string message)
			: base (message)
		{
		}

		/// <param name="message">Message.</param>
		/// <param name="inner">Inner.</param>
		public NonExistentNodeException (string message, Exception inner)
			: base (message, inner)
		{
		}

		/// <param name="info">Info.</param>
		/// <param name="context">Context.</param>
		protected NonExistentNodeException (System.Runtime.Serialization.SerializationInfo info,
		                                    System.Runtime.Serialization.StreamingContext context)
			: base (info, context)
		{
		}
	}
}