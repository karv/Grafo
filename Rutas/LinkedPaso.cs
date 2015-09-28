namespace Graficas.Rutas
{
	/// <summary>
	/// Es un paso cuyo peso se va calculando dependiende de una gráfica
	/// </summary>
	public struct LinkedPaso<T>:IPaso<T>
	{
		readonly IGraficaPeso<T> graf;

		public LinkedPaso(T origen, T destino, IGraficaPeso<T> graf)
		{
			Origen = origen;
			Destino = destino;
			this.graf = graf;
		}

		#region IPaso implementation

		public T Origen { get; }

		public T Destino { get; }

		public float Peso
		{
			get
			{
				return graf.Peso(Origen, Destino);
			}
		}

		#endregion
	}
}

