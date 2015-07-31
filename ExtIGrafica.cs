using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graficas.Rutas;

namespace Graficas
{

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
