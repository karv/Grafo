using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graficas.Rutas;

namespace Graficas
{

	/// <summary>
	/// Promete habilidad para pedir origen y destino (de esta arista).
	/// </summary>
	/// <typeparam name="T">Tipo de nodos</typeparam>
	public interface IArista<T>
	{
		T desde { get; }

		T hasta { get; }
	}

}
