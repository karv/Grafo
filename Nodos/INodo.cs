using System.Collections.Generic;

namespace Graficas.Nodos
{
	/// <summary>
	/// Un nodo de un grafo
	/// </summary>
	public interface INodo<T>
	{
		/// <summary>
		/// Devuelve el objeto asociado al nodo
		/// </summary>
		/// <value>The objeto.</value>
		T Objeto { get; }

		/// <summary>
		/// Devuelve la vecindad de este nodo
		/// </summary>
		IEnumerable<INodo<T>> Vecindad { get; }
	}
}