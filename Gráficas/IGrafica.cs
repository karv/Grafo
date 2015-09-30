using System.Collections.Generic;
using Graficas.Rutas;

namespace Graficas
{

	/// <summary>
	/// Promete lista de vecinos.
	/// </summary>
	public interface IGrafica<T>
	{
		ICollection<T> Nodos
		{
			get;
		}

		bool ExisteArista(T desde, T hasta);

		ICollection<IArista<T>> Aristas();

		void AgregaArista(T desde, T hasta);

		ICollection<T> Vecinos(T nodo);

		IRuta<T> ToRuta(IEnumerable<T> seq);

		bool EsSimétrico{ get; }
	}
}
