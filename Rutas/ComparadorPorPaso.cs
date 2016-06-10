using System;
using System.Collections.Generic;
using Graficas.Aristas;

namespace Graficas.Rutas
{
	public class ComparadorPorPaso<T> : IEqualityComparer<IRuta<T>>
	{
		public ComparadorPorPaso ()
		{
			Comparador = EqualityComparer<T>.Default;
		}

		public ComparadorPorPaso (IEqualityComparer<T> comparador)
		{
			Comparador = comparador;
		}

		IEqualityComparer<T> Comparador { get; }

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

		public int GetHashCode (IRuta<T> obj)
		{
			if (obj == null)
				return 0;
			var ret = 0;
			foreach (var x in obj.Pasos)
				ret += Comparador.GetHashCode (x.Origen) + Comparador.GetHashCode (x.Destino);
			return ret;
		}
	}
}