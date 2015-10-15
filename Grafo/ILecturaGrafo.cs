using System.Collections.Generic;
using Graficas.Rutas;

namespace Graficas
{

	/// <summary>
	/// Una grafo sólo lectura
	/// </summary>
	public interface ILecturaGrafo<T>
	{
		ICollection<T> Nodos
		{
			get;
		}

		bool this [T desde, T hasta]{ get; }

		ICollection<IArista<T>> Aristas();

		ICollection<T> Vecinos(T nodo);

		IRuta<T> ToRuta(IEnumerable<T> seq);

		/// <summary>
		/// Calcula el subgrafo generado por un subconjutno de Nodos
		/// </summary>
		/// <param name="conjunto">Conjunto de nodos para calcular el subgrafo</param>
		ILecturaGrafo<T> Subgrafo(IEnumerable<T> conjunto);
	}
}