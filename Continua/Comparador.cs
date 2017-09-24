using System.Collections.Generic;
using System;

namespace Graficas.Continua
{
	// THINK ¿No serializarlo, y construirlo cada vez que un grafo es deserializado?
	/// <summary>
	/// Comparador de coincidencia de puntos en un continuo.
	/// </summary>
	[Serializable]
	public class ComparadorCoincidencia<T> : IEqualityComparer<Punto<T>>
	{
		/// <param name="compa">El comparador de nodos</param>
		/// <remarks>Usar comparador null que se use el comparador Default </remarks>
		public ComparadorCoincidencia (IEqualityComparer<T> compa = null)
		{
			ComparaNodos = compa ?? EqualityComparer<T>.Default;
		}

		/// <summary>
		/// Devuelve el comparador de nodos que se usa para comparar extremos.
		/// </summary>
		public IEqualityComparer<T> ComparaNodos { get; }

		/// <summary>
		/// Revisa si dos puntos coinciden
		/// </summary>
		/// <param name="x">Punto</param>
		/// <param name="y">Punto</param>
		public bool Equals (Punto<T> x, Punto<T> y)
		{
			if (ReferenceEquals (null, x) || ReferenceEquals (null, y))
				return false;

			return x.Coincide (y);
		}

		/// <Docs>The object for which the hash code is to be returned.</Docs>
		/// <para>Returns a hash code for the specified object.</para>
		/// <returns>A hash code for the specified object.</returns>
		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <param name="obj">Object.</param>
		public int GetHashCode (Punto<T> obj)
		{
			return obj.EnOrigen ? ComparaNodos.GetHashCode (obj.A) :
				ComparaNodos.GetHashCode (obj.A) + ComparaNodos.GetHashCode (obj.B) + obj.Loc.GetHashCode ();
		}
	}
}