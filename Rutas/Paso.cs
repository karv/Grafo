namespace Graficas.Rutas
{
	public struct Paso<T>: IPaso<T>
	{
		public T origen;
		public T destino;
		public float peso;

		public Paso(T origen, T destino, float peso)
		{
			this.origen = origen;
			this.destino = destino;
			this.peso = peso;
		}

		public override string ToString()
		{
			return string.Format("{0} --{1}--> {2}", origen, peso, destino);
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

