using System;
using System.Security.Cryptography;

namespace Graficas.Rutas
{
	public struct Paso<T>: IPaso<T>
	{
		readonly T origen;
		readonly T destino;
		readonly float peso;

		public Paso(T origen, T destino, float peso)
		{
			this.origen = origen;
			this.destino = destino;
			this.peso = peso;
		}

		#region IMultiPaso implementation

		public T Origen
		{
			get
			{
				return origen;
			}
		}

		public T Destino
		{
			get
			{
				return destino;
			}
		}

		public float Peso
		{
			get
			{
				return peso;
			}
		}

		#endregion
	}
}

