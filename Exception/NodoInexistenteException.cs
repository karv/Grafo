using System;

namespace Graficas
{

	[Serializable]
	public class NodoInexistenteException : Exception
	{
		public NodoInexistenteException ()
		{
		}

		public NodoInexistenteException (string message)
			: base (message)
		{
		}

		public NodoInexistenteException (string message, Exception inner)
			: base (message, inner)
		{
		}

		protected NodoInexistenteException (System.Runtime.Serialization.SerializationInfo info,
		                                   System.Runtime.Serialization.StreamingContext context)
			: base (info, context)
		{
		}
	}
}
