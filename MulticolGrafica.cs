using System;
using System.Collections.Generic;

namespace Graficas
{
	/// <summary>
	/// Modela una IMulticolGrafica que es la superposición de varias IGráficas
	/// </summary>
	public class MulticolGrafica<T, V> : IMulticolGrafica<T, V>
	{
		/// <summary>
		/// La asignación de V -> Gráfica
		/// </summary>
		Dictionary <V, IGrafica<T>> _asignación = new Dictionary<V, IGrafica<T>>();


		public MulticolGrafica()
		{
		}

		#region IMulticolGrafica implementation

		public ICollection<T> Vecinos(T nodo, V color)
		{
			IGrafica<T> graf;
			if (_asignación.TryGetValue(color, out graf))
			{
				return graf.Vecinos(nodo);
			}
			else
				return new T[0];
		}

		public ICollection<T> Nodos
		{
			
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}

