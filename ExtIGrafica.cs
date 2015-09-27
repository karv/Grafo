using System;
using System.Collections.Generic;

namespace Graficas
{

	public static class ExtIGrafica
	{
		public static IEnumerable<IArista<T>> EnumerarNodos<T>(this IGrafica<T> graf) where T : IEquatable<T>
		{
			var ret = new List<Arista<T>>();
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
