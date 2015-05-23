using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graficas
{
	/// <summary>
	/// Promete la habilidad de decidir el peso de cada arista en una gráfica.
	/// </summary>
	/// <typeparam name="T">Tipo de nodos.</typeparam>
	interface IGraficaPeso<T> : IGrafica<T>
	{
		/// <summary>
		/// El peso de una arista.
		/// </summary>
		/// <param name="desde">Origen de la arista</param>
		/// <param name="hasta">Destino de la arista</param>
		/// <returns>Devuelve el peso (float) de la arista.</returns>
		float Peso(T desde, T hasta);
	}
}
