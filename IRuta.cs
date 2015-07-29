using System;
using System.Collections.Generic;

namespace Graficas.Rutas
{
	public interface IRuta<T>: IEnumerable<T>
	{
		T NodoInicial { get; }

		T NodoFinal { get; }

		float Longitud { get; }

		int NumPasos { get; }
	}
}

