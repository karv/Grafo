namespace Graficas.Rutas
{
	/// <summary>
	/// Un paso como estructura independiente y de sólo lectura
	/// </summary>
	public struct Paso<T> : IPaso<T>
	{
		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		/// <param name="peso">Peso.</param>
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

		#region Paso

		public T Origen { get; }

		public T Destino { get; }

		public float Peso { get; }

		#endregion
	}
}