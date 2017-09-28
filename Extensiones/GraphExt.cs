using System.Collections.Generic;
using ListasExtra.Extensiones;
using Graficas.Grafo;
using System;

namespace Graficas.Extensiones
{
	/// <summary>
	/// Métodos extendidos de grafos
	/// </summary>
	public static class GraphExt
	{
		/// Devuelve una colección con las componentes conexas de una gráfica dada
		/// <summary>
		/// </summary>
		/// <param name="gr">Gráfo</param>
		/// <typeparam name="T">Tipo de nodos de la gráfica</typeparam>
		public static ICollection<IGrafo<T>> ComponentesConexas<T> (this IGrafo<T> gr)
			where T : IEquatable<T>
		{
			var nodosRestantes = new HashSet<T> (gr.Nodes);
			HashSet<T> Verdes;
			var ret = new List<IGrafo<T>> ();

			while (nodosRestantes.Count > 0)
			{
				T nodo = nodosRestantes.Aleatorio ();

				// Calcular la nube de nodo
				var nubeActual = new HashSet<T> ();
				nubeActual.Add (nodo);
				Verdes = new HashSet<T> (nubeActual);
				HashSet<T> nubeAgregando;
				do
				{
					nubeAgregando = new HashSet<T> ();
					foreach (var x in Verdes)
					{
						nubeAgregando.UnionWith (gr.Neighborhood (x));
						nubeAgregando.ExceptWith (nubeActual);
						nubeAgregando.ExceptWith (Verdes);
					}
					nubeActual.UnionWith (Verdes);
					Verdes = new HashSet<T> (nubeAgregando);
				}
				while (Verdes.Count > 0);

				ret.Add (gr.Subgraph (nubeActual));
				nodosRestantes.ExceptWith (nubeActual);
			}

			return ret;
		}
	}
}