using System;
using System.Collections.Generic;
using Graficas.Aristas;

namespace Graficas.Rutas
{
	/// <summary>
	/// Un comparador de Rutas que hace la comparación paso a paso
	/// con un comparador de nodos dado.
	/// </summary>
	public class ComparadorPorPaso<T> : IEqualityComparer<IRuta<T>>
	{
		/// <summary>
		/// </summary>
		public ComparadorPorPaso ()
		{
			Comparador = EqualityComparer<T>.Default;
		}

		/// <param name="comparador">Comparador de nodos</param>
		public ComparadorPorPaso (IEqualityComparer<T> comparador)
		{
			if (comparador == null)
				throw new ArgumentException (
					"No se puede usar null como comparador",
					"comparador");
			Comparador = comparador;
		}

		IEqualityComparer<T> Comparador { get; }

		/// <summary>
		/// Revisa si dos rutas son equivalentes.
		/// </summary>
		/// <param name="x">Primera ruta</param>
		/// <param name="y">Segunda ruta</param>
		public bool Equals (IRuta<T> x, IRuta<T> y)
		{
			if (x == null || y == null)
				return false;
			if (x.NumPasos != y.NumPasos)
				return false;
			if (!Comparador.Equals (x.NodoInicial, y.NodoInicial))
				return false;
			if (!Comparador.Equals (x.NodoFinal, y.NodoFinal))
				return false;

			var enumX = new List<IPaso<T>> (x.Pasos);
			var enumY = new List<IPaso<T>> (y.Pasos);

			for (int i = 0; i < x.NumPasos; i++)
			{
				if (!Comparador.Equals (enumX [i].Origen, enumY [i].Origen))
					return false;
				if (!Comparador.Equals (enumX [i].Destino, enumY [i].Destino))
					return false;
			}

			return true;
		}

		/// <Docs>The object for which the hash code is to be returned.</Docs>
		/// <para>Returns a hash code for the specified object.</para>
		/// <returns>A hash code for the specified object.</returns>
		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <param name="obj">Object.</param>
		public int GetHashCode (IRuta<T> obj)
		{
			if (obj == null)
				return 0;
			var ret = 0;
			foreach (var x in obj.Pasos)
				ret += Comparador.GetHashCode (x.Origin) + Comparador.GetHashCode (x.Destination);
			return ret;
		}
	}
}