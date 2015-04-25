using System;
using System.Collections.Generic;

namespace Graficas
{
	/// <summary>
	/// Da herramientas para trabajar con rutas en IGraficas
	/// </summary>
	public static class ExtensionRutas<T>
	{
		public static Ruta<T> MejorRuta(IGrafica<T> graf, T origen, T destino)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Calcula la ruta óptima de un nodo a otro.
		/// </summary>
		/// <param name="x">Nodo inicial.</param>
		/// <param name="y">Nodo final.</param>
		/// <param name="Ignorar">Lista de nodos a evitar.</param>
		/// <returns>Devuelve la ruta de menor <c>Longitud</c>.</returns>
		/// <remarks>Puede ciclar si no existe ruta de x a y.</remarks> // TODO: Arreglar esto.
		public static Ruta<T> CaminoÓptimo(IGrafica<T> graf, T x, T y, List<T> Ignorar)
		{
			throw new NotImplementedException();
		}
	}

	public class Ruta<T>
	{
		public List<T> Paso;

		public static bool operator ==(Ruta<T> left, Ruta<T> right)
		{
			if (left.Paso.Count != right.Paso.Count)
				return false;

			for (int i = 0; i < left.Paso.Count; i++)
			{
				if (!left.Paso[i].Equals(right.Paso[i]))
					return false;
			}
			return true;
		}

		public static bool operator !=(Ruta<T> left, Ruta<T> right)
		{
			return !(left == right);
		}

		public override bool Equals(object obj)
		{
			if (obj is Ruta<T>)
				return ((Ruta<T>)obj) == this;
			else
				return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
