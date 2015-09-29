using Graficas.Rutas;

namespace Graficas
{

	/// <summary>
	/// Provee método para encontrar ruta óptima entre puntos
	/// </summary>
	public interface IGraficaRutas<T> : IGrafica<T>
	{
		IRuta<T> RutaÓptima(T x, T y);
	}

}
