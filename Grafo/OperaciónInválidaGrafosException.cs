using System;

namespace Graficas.Grafo
{
	/// <summary>
	/// Operación inválida grafos exception.
	/// </summary>
	[Serializable]
	public class OperaciónInválidaGrafosException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Graficas.Grafo.OperaciónInválidaGrafosException"/> class
		/// </summary>
		public OperaciónInválidaGrafosException ()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Graficas.Grafo.OperaciónInválidaGrafosException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		public OperaciónInválidaGrafosException (string message)
			: base (message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Graficas.Grafo.OperaciónInválidaGrafosException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		/// <param name="inner">The exception that is the cause of the current exception. </param>
		public OperaciónInválidaGrafosException (string message, Exception inner)
			: base (message, inner)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Graficas.Grafo.OperaciónInválidaGrafosException"/> class
		/// </summary>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <param name="info">The object that holds the serialized object data.</param>
		protected OperaciónInválidaGrafosException (System.Runtime.Serialization.SerializationInfo info,
		                                            System.Runtime.Serialization.StreamingContext context)
			: base (info,
			        context)
		{
		}
	}
}