using System;
using System.Collections.Generic;
using ListasExtra.Extensiones;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

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
		public static ICollection<ILecturaGrafo<T>> ComponentesConexas<T>(this ILecturaGrafo<T> gr) //TEST
		{
			var nodosRestantes = new HashSet<T>(gr.Nodos);
			var ret = new List<ILecturaGrafo<T>>();

			while (nodosRestantes.Count > 0)
			{
				T nodo = nodosRestantes.Aleatorio();

				// Calcular la nube de nodo
				var nubeActual = new HashSet<T>();
				nubeActual.Add(nodo);
				HashSet<T> nubeAgregando;
				do
				{
					nubeAgregando = new HashSet<T>();
					foreach (var x in nubeActual)
					{
						nubeAgregando.UnionWith(gr.Vecinos(x));
					}
					nubeActual.UnionWith(nubeAgregando);
				} while (nubeAgregando.Count > 0);

				ret.Add(gr.Subgrafo(nubeActual));
				nodosRestantes.ExceptWith(nubeActual);
			}

			return ret;
		}

	}
}

