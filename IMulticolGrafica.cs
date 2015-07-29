using System;
using System.Collections.Generic;

namespace Graficas
{
	/// <summary>
	/// Representa una gráfica de muchos 'colores'
	/// </summary>
	public interface IMulticolGrafica<T, V>
	{
		ICollection<T> Nodos
		{
			get;
		}

		ICollection<T> Vecinos(T nodo, V color);
	}
}

