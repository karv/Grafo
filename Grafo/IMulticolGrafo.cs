﻿using System.Collections.Generic;
using Graficas.Aristas;

namespace Graficas.Grafo
{
	/// <summary>
	/// Representa una gráfica de muchos 'colores'
	/// </summary>
	public interface IMulticolGrafo<TNodo, TColor>: ILecturaGrafo<TNodo>
	{
		/// <summary>
		/// Vecinos de un nodo de un color específico.
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		/// <param name="color">Color.</param>
		ICollection<TNodo> Vecinos (TNodo nodo, TColor color);

		/// <summary>
		/// Agrega un color
		/// </summary>
		/// <param name="color">Color a agregar</param>
		/// <param name="grafo">Grafo enlazado al color</param>
		void AgregaColor (TColor color, ILecturaGrafo<TNodo> grafo);

		/// <summary>
		/// Devuelve el grafo asociado a un color.
		/// </summary>
		/// <returns>El grafo de cierto color.</returns>
		/// <param name="color">Color.</param>
		ILecturaGrafo<TNodo> GrafoColor (TColor color);

		/// <summary>
		/// Devuelve los colores que existen consistentemente con una arista
		/// </summary>
		/// <returns>Enumeracion de colores</returns>
		/// <param name="aris">Arista</param>
		IEnumerable<TColor> ColoresArista (IArista<TNodo> aris);
	}
}