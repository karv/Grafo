using Graficas.Aristas;
using System.Collections.Generic;
using Graficas.Rutas;
using System;

namespace Graficas.Grafo
{
	/// <summary>
	/// Un grafo en el que se pueden leer y escribir nodos y aristas
	/// </summary>
	public interface IGrafo<T>
		where T : IEquatable<T>
	{
		/// <summary>
		/// Elimina nodos y aristas.
		/// </summary>
		void Clear ();

		/// <summary>
		/// La arista correspondiente a un par de puntos
		/// </summary>
		/// <returns> Una arista no nula </returns>
		/// <param name="desde">Origen</param>
		/// <param name="hasta">Destino</param>
		/// <remarks> Debe devolver una (¿nueva?) arista en caso de que no exista como tal, 
		/// diciendo que esa arista no existe.
		/// NUNCA devuelve null</remarks>
		IArista<T>  this [T desde, T hasta]{ get; }

		/// <summary>
		/// Nodos
		/// </summary>
		/// <value>The nodos.</value>
		ICollection<T> Nodos { get; }

		/// <summary>
		/// Colección de aristas
		/// </summary>
		ICollection<IArista<T>> Aristas ();

		/// <summary>
		/// Colección de vecinos de un nodo
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		ICollection<T> Vecinos (T nodo);

		/// <summary>
		/// Convierte una sucesión consistente de nodos a una ruta
		/// </summary>
		/// <returns>The ruta.</returns>
		/// <param name="seq">Sucesión consistente.</param>
		IRuta<T> ToRuta (IEnumerable<T> seq);

		/// <summary>
		/// Calcula el subgrafo generado por un subconjutno de Nodos
		/// </summary>
		/// <param name="conjunto">Conjunto de nodos para calcular el subgrafo</param>
		IGrafo<T> Subgrafo (IEnumerable<T> conjunto);
	}
}