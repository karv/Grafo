using System;

namespace Graficas.Aristas
{
	/// <summary>
	/// Representa un paso en una ruta de un grafo.
	/// </summary>
	public interface IPaso<T> : IAristaDirigida<T>
	{
		/// <summary>
		/// Peso del paso
		/// </summary>
		float Peso { get; }
	}
}