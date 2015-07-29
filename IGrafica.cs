﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graficas
{

	/// <summary>
	/// Promete lista de vecinos.
	/// </summary>
	public interface IGrafica<T>
	{
		ICollection<T> Nodos
		{
			get;
		}

		ICollection<T> Vecinos(T nodo);

		Graficas.Rutas.IRuta<T> toRuta(IEnumerable<T> seq);
	}

	/// <summary>
	/// Promete habilidad para pedir origen y destino (de esta arista).
	/// </summary>
	/// <typeparam name="T">Tipo de nodos</typeparam>
	public interface IArista<T>
	{
		T desde { get; }

		T hasta { get; }
	}


	public static class ExtIGrafica
	{
		public static IEnumerable<IArista<T>> EnumerarNodos<T>(this IGrafica<T> graf) where T : IEquatable<T>
		{
			List<Arista<T>> ret = new List<Arista<T>>();
			foreach (T x in graf.Nodos)
			{
				foreach (T y in graf.Vecinos(x))
				{
					ret.Add(new Arista<T>(x, y));
				}
			}
			return (IEnumerable<IArista<T>>)ret;
		}
	}

}
