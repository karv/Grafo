using System;

namespace Graficas.Rutas
{
	/// <summary>
	/// Es un paso cuyo peso se va calculando dependiende de una gráfica
	/// </summary>
	public struct LinkedPaso<T>:IPaso<T>
	{
		T origen;
		T destino;
		IGraficaPeso<T> graf;

		public LinkedPaso(T origen, T destino, IGraficaPeso<T> graf)
		{
			this.origen = origen;
			this.destino = destino;
			this.graf = graf;
		}

		#region IPaso implementation

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
				return graf.Peso(origen, destino);
			}
		}

		#endregion
	}
}

