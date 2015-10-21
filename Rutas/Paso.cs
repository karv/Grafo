namespace Graficas.Rutas
{
	public struct Paso<T>: IPaso<T>
	{
		public Paso (T origen, T destino, float peso)
		{
			Origen = origen;
			Destino = destino;
			Peso = peso;
		}

		public override string ToString ()
		{
			return string.Format ("{0} --[{1}]--> {2}", Origen, Peso, Destino);
		}

		#region IMultiPaso implementation

		public T Origen { get; }

		public T Destino { get; }

		public float Peso { get; }

		#endregion
	}
}