using System.Collections.Generic;
using Graficas.Rutas;
using Graficas.Aristas;

namespace Graficas.Grafo
{
	/// <summary>
	/// Representa un grafo en el que se pueden leer los nodos y aristas
	/// </summary>
	public interface ILecturaGrafo<T>
	{
		/// <summary>
		/// Nodos
		/// </summary>
		/// <value>The nodos.</value>
		ICollection<T> Nodos { get; }

		/// <summary>
		/// Arista existente
		/// </summary>
		/// <param name="desde">Origen</param>
		/// <param name="hasta">Destino</param>
		bool this [T desde, T hasta] { get; }

		/// <summary>
		/// Colección de aristas
		/// </summary>
		ICollection<IArista<T>> Aristas ();

		/// <summary>
		/// Revisa si existe una arista entre dos nodos
		/// </summary>
		/// <returns><c>true</c>, si existe una arista<c>false</c> otherwise.</returns>
		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		bool ExisteArista (T origen, T destino);

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
		ILecturaGrafo<T> Subgrafo (IEnumerable<T> conjunto);
	}
}