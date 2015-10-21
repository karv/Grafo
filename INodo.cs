using System.Collections.Generic;


namespace Graficas
{
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