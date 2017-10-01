﻿using System;
using System.Collections.Generic;
using Graficas.Edges;

namespace Graficas.Rutas
{
	/// <summary>
	/// Un comparador de Rutas que hace la comparación paso a paso
	/// con un comparador de nodos dado.
	/// </summary>
	public class ComparadorPorPaso<T> : IEqualityComparer<IPath<T>>
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
		public bool Equals (IPath<T> x, IPath<T> y)
		{
			if (x == null || y == null)
				return false;
			if (x.NumPasos != y.NumPasos)
				return false;
			if (!Comparador.Equals (x.NodoInicial, y.NodoInicial))
				return false;
			if (!Comparador.Equals (x.NodoFinal, y.NodoFinal))
				return false;

			var enumX = new List<IStep<T>> (x.Pasos);
			var enumY = new List<IStep<T>> (y.Pasos);

			for (int i = 0; i < x.NumPasos; i++)
			{
				if (!Comparador.Equals (enumX[i].Origin, enumY[i].Origin))
					return false;
				if (!Comparador.Equals (enumX[i].Destination, enumY[i].Destination))
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
		public int GetHashCode (IPath<T> obj)
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