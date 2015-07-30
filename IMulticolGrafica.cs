using System;
using System.Collections.Generic;

namespace Graficas
{
	/// <summary>
	/// Representa una gráfica de muchos 'colores'
	/// </summary>
	public interface IMulticolGrafica<T, V>: IGrafica<T>
	{

		ICollection<T> Vecinos(T nodo, V color);

		void AgregaColor(V color);

		IGrafica<T> GraficaColor(V color);

		IEnumerable<V> getColoresArista(IArista<T> aris);
	}
}

