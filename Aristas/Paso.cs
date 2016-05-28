using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

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
			: this (aris.Origen, aris.Destino)
		{
			var ar = aris as AristaPeso<T, float>;
			if (ar != null)
				Peso = ar.Data;
		}

		public Paso (IAristaDirigida<T> aris, float peso)
			: this (aris.Origen, aris.Destino)
		{
			Peso = peso;
		}

		public Paso (IPaso<T> paso)
		{
			Origen = paso.Origen;
			Destino = paso.Destino;
			Peso = paso.Peso;
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

		public override string ToString ()
		{
			return string.Format ("{0} -- [{2}] -> {1}", Origen, Destino, Peso);
		}
	}
}