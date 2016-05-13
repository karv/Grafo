using Graficas.Rutas;

namespace Graficas.Grafo
{
	/// <summary>
	/// Provee método para encontrar ruta óptima entre puntos
	/// </summary>
	public interface IGrafoRutas<T> : ILecturaGrafo<T>
	{
		/// <summary>
		/// Devuelve la ruta óptima entre dos nodos
		/// </summary>
		/// <returns>Devuelve una ruta de menor longitud</returns>
		/// <param name="x">Origen</param>
		/// <param name="y">Final</param>
		IRuta<T> RutaÓptima (T x, T y);
	}
}