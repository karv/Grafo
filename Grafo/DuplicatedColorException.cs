using System;

namespace CE.Graph.Grafo
{
	/// <summary>
	/// Color duplicado expection.
	/// </summary>
	[Serializable]
	public class DuplicatedColorException : Exception
	{
		/// <summary>
		/// 
		/// </summary>
		public DuplicatedColorException ()
		{
		}

		/// <param name="message">Message.</param>
		public DuplicatedColorException (string message)
			: base (message)
		{
		}

		/// <param name="message">Message.</param>
		/// <param name="inner">Inner.</param>
		public DuplicatedColorException (string message, Exception inner)
			: base (message, inner)
		{
		}

		/// <param name="info">Info.</param>
		/// <param name="context">Context.</param>
		protected DuplicatedColorException (System.Runtime.Serialization.SerializationInfo info,
		                                   System.Runtime.Serialization.StreamingContext context)
			: base (info,
			        context)
		{
		}
	}
}