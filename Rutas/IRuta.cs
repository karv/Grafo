using System.Collections.Generic;
using Graficas.Edges;
using System;

namespace Graficas.Rutas
{
	/// <summary>
	/// Una ruta de un grafo
	/// </summary>
	public interface IRuta<T>
	{
		/// <summary>
		/// Devuelve el nodo inicial
		/// </summary>
		T NodoInicial { get; }

		/// <summary>
		/// Devuelve el nodo final
		/// </summary>
		/// <value>The nodo final.</value>
		T NodoFinal { get; }

		/// <summary>
		/// Devuelve el número de pasos
		/// </summary>
		int NumPasos { get; }

		/// <summary>
		/// Devuelve la longitud de la ruta
		/// </summary>
		/// <value>The longitud.</value>
		float Longitud { get; }

		/// <summary>
		/// Concatena esta ruta con un paso
		/// </summary>
		/// <param name="paso">Paso con qué concatenar</param>
		void Concat (IStep<T> paso);

		/// <summary>
		/// Concatena esta ruta con otra ruta
		/// </summary>
		/// <param name="ruta">Ruta con qué concatenar</param>
		void Concat (IRuta<T> ruta);

		/// <summary>
		/// Enumera los pasos de la ruta
		/// </summary>
		IEnumerable<IStep<T>> Pasos { get; }
	}

	/// <summary>
	/// Extensiones para rutas
	/// </summary>
	public static class RutaExt
	{
		/// <summary>
		/// Devuelve la longitud de esta ruta, dada una función de peso
		/// </summary>
		/// <param name="ruta">Ruta.</param>
		/// <param name="peso">Función de peso</param>
		/// <typeparam name="T">Nodos de ruta</typeparam>
		[Obsolete ("IRuta cuenta con propiedad Longitud.get")]
		public static float Longitud<T> (this IRuta<T> ruta,
																		 Func<IEdge<T>, float> peso)
			where T : IEquatable<T>
		{
			float ret = 0;
			foreach (var x in ruta.Pasos)
			{
				ret += peso (x);
			}
			return ret;
		}
	}
}