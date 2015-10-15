using System.Collections.Generic;

namespace Graficas.Rutas
{
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
		/// Devuelve la longitud, módulo su grafo subyacente, de esta ruta
		/// </summary>
		float Longitud { get; }

		/// <summary>
		/// Devuelve el número de pasos
		/// </summary>
		int NumPasos { get; }

		/// <summary>
		/// Construye uan ruta como ésta, en sentido inverso.
		/// </summary>
		IRuta<T> Reversa();

		/// <summary>
		/// Concatena esta ruta
		/// </summary>
		/// <param name="paso">Paso con qué concatenar</param>
		void Concat(IPaso<T> paso);

		/// <summary>
		/// Concat the specified ruta.
		/// </summary>
		/// <param name="ruta">Ruta con qué concatenar</param>
		void Concat(IRuta<T> ruta);

		/// <summary>
		/// Concatena esta ruta
		/// </summary>
		/// <param name="nodo">Nodo con qué concatenar</param>
		/// <param name="peso">distancia de el Nodo final a este nuevo nodo</param>
		void Concat(T nodo, float peso);

		/// <summary>
		/// Enumera los pasos de la ruta
		/// </summary>
		IEnumerable<IPaso<T>> Pasos { get; }

	}
}

