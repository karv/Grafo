using System.Collections.Generic;
using ListasExtra.Extensiones;
using Graficas.Grafo;

namespace Graficas.Extensiones
{
	/// <summary>
	/// Métodos extendidos de grafos
	/// </summary>
	public static class Extensiones
	{
		/// <summary>
		/// Devuelve una colección con las componentes conexas de una gráfica dada
		/// </summary>
		/// <param name="gr">Gráfo</param>
		/// <typeparam name="T">Tipo de nodos de la gráfica</typeparam>
		public static ICollection<ILecturaGrafo<T>> ComponentesConexas<T> (this ILecturaGrafo<T> gr)
		{
			var nodosRestantes = new HashSet<T> (gr.Nodos);
			HashSet<T> Verdes;
			var ret = new List<ILecturaGrafo<T>> ();

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
						nubeAgregando.UnionWith (gr.Vecinos (x));
						nubeAgregando.ExceptWith (nubeActual);
						nubeAgregando.ExceptWith (Verdes);
					}
					nubeActual.UnionWith (Verdes);
					Verdes = new HashSet<T> (nubeAgregando);
				}
				while (Verdes.Count > 0);

				ret.Add (gr.Subgrafo (nubeActual));
				nodosRestantes.ExceptWith (nubeActual);
			}

			return ret;
		}

	}
}