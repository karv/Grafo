using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graficas.Rutas;

namespace Graficas
{

	/// <summary>
	/// Provee método para encontrar ruta óptima entre puntos
	/// </summary>
	public interface IGraficaRutas<T> : IGrafica<T>
	{
		IRuta<T> RutaOptima(T x, T y);
	}

}
