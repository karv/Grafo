using System;
using System.Collections.Generic;

namespace Graficas.Aristas
{
	[Serializable]
	public struct Paso<T> : IPaso<T>
		where T : IEquatable<T>
	{
		public T Origen { get; }

		public T Destino { get; }

		public float Peso { get; }

		public Paso (T origen, T destino, float peso = 1)
		{
			Origen = origen;
			Destino = destino;
			Peso = peso;
		}

		public Paso (IAristaDirigida<T> aris)
		{
			Origen = aris.Origen;
			Destino = aris.Destino;
			var xAris = aris as AristaPeso<T, float>;
			Peso = xAris == null ? 1 : xAris.Data;
		}

		public bool Coincide (T origen, T destino)
		{
			var cmp = EqualityComparer<T>.Default;
			return cmp.Equals (origen, Origen) && cmp.Equals (destino, Destino);
		}

		public ListasExtra.ParNoOrdenado<T> ComoPar ()
		{
			return new ListasExtra.ParNoOrdenado<T> (Origen, Destino);
		}

		public T Antipodo (T nodo)
		{
			return ComoPar ().Excepto (nodo);
		}

		public bool Corta (T nodo)
		{
			var cmp = EqualityComparer<T>.Default;
			return cmp.Equals (nodo, Origen) || cmp.Equals (nodo, Destino);
		}

		public bool Existe
		{
			get
			{
				return true;
			}
		}
	}
}