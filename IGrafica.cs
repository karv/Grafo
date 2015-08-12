using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		void AgregaArista(T desde, T hasta);

		ICollection<T> Vecinos(T nodo);

		Graficas.Rutas.IRuta<T> toRuta(IEnumerable<T> seq);

		bool esSimétrico{ get; }
	}
}
