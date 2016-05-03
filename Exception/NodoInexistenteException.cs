using System;

namespace Graficas.Exception
{
	[Serializable]
	public class NodoInexistenteException : System.Exception
	{
		public NodoInexistenteException ()
		{
		}

		public NodoInexistenteException (string message)
			: base (message)
		{
		}

		public NodoInexistenteException (string message, System.Exception inner)
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
