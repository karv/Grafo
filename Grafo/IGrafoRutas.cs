using Graficas.Rutas;

namespace Graficas.Grafo
{
	/// <summary>
	/// Provee método para encontrar ruta óptima entre puntos
	/// </summary>
	public interface IGrafoRutas<T> : ILecturaGrafo<T>
	{
		IRuta<T> RutaÓptima (T x, T y);
	}
}