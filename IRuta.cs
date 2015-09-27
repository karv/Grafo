using System;
using System.Collections.Generic;

namespace Graficas.Rutas
{
	public interface IRuta<T>
	{
		T NodoInicial { get; }

		T NodoFinal { get; }

		float Longitud { get; }

		int NumPasos { get; }

		IRuta<T> Reversa();

		void Concat(IPaso<T> paso);

		void Concat(IRuta<T> ruta);

		void Concat(T nodo, float peso);

		IEnumerable<IPaso<T>> Pasos { get; }

	}
}

