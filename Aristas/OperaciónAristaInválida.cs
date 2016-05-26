using System;

namespace Graficas.Aristas
{
	/// <summary>
	/// Exception que se tira cuando una operación con respecto a las aristas, operaciones, acceso, etc falla.
	/// </summary>
	[Serializable]
	public class OperaciónAristaInválidaException : InvalidOperationException
	{
		/// <summary>
		/// Initializes a new instance of this class
		/// </summary>
		public OperaciónAristaInválidaException ()
		{
		}

		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		public OperaciónAristaInválidaException (string message)
			: base (message)
		{
		}

		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		/// <param name="inner">The exception that is the cause of the current exception. </param>
		public OperaciónAristaInválidaException (string message,
		                                         Exception inner)
			: base (message,
			        inner)
		{
		}

		/// <param name="context">The contextual information about the source or destination.</param>
		/// <param name="info">The object that holds the serialized object data.</param>
		protected OperaciónAristaInválidaException (System.Runtime.Serialization.SerializationInfo info,
		                                            System.Runtime.Serialization.StreamingContext context)
			: base (info,
			        context)
		{
		}
	}

}