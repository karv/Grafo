using System.Collections.Generic;
using System;

namespace Graficas.Continuo
{
	// THINK ¿No serializarlo, y construirlo cada vez que un grafo es deserializado?
	[Serializable]
	public class ComparadorCoincidencia<T> : IEqualityComparer<Punto<T>>
	{
		public ComparadorCoincidencia (IEqualityComparer<T> compa = null)
		{
			ComparaNodos = compa ?? EqualityComparer<T>.Default;
		}

		/// <summary>
		/// Devuelve el comparador de nodos que se usa para comparar extremos.
		/// </summary>
		public IEqualityComparer<T> ComparaNodos { get; }

		public bool Equals (Punto<T> x, Punto<T> y)
		{
			if (ReferenceEquals (null, x) || ReferenceEquals (null, y))
				return false;

			return x.Coincide (y);
		}

		public int GetHashCode (Punto<T> obj)
		{
			return obj.EnOrigen ? ComparaNodos.GetHashCode (obj.A) : 
				ComparaNodos.GetHashCode (obj.A) + ComparaNodos.GetHashCode (obj.B) + obj.Loc.GetHashCode ();
		}
	}
}