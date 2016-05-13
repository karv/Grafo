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

		/// <summary>
		/// </summary>
		public override string ToString ()
		{
			return string.Format ("{0} --[{1}]--> {2}", Origen, Peso, Destino);
		}

		#region Paso

		/// <summary>
		/// Devuelve el origen
		/// </summary>
		/// <value>The origen.</value>
		public T Origen { get; }

		/// <summary>
		/// Devuelve el destino
		/// </summary>
		/// <value>The destino.</value>
		public T Destino { get; }

		/// <summary>
		/// Devuelve el peso o longitud del paso
		/// </summary>
		/// <value>The peso.</value>
		public float Peso { get; }

		#endregion
	}
}