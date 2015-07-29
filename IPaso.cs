using System;
using System.Collections.Generic;

namespace Graficas
{
	public interface IPaso<T>
	{
		T Origen { get; }

		T Destino{ get; }

		float Peso { get; }
	}
}

