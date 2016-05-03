using Graficas.Grafo;

namespace Graficas.Rutas
{
	/// <summary>
	/// Es un paso cuyo peso se va calculando dependiende de una gráfica
	/// </summary>
	public struct LinkedPaso<T> : IPaso<T>
	{
		readonly ILecturaGrafoPeso<T> graf;

		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		/// <param name="graf">Grafo asociado</param>
		public LinkedPaso (T origen, T destino, ILecturaGrafoPeso<T> graf)
		{
			Origen = origen;
			Destino = destino;
			this.graf = graf;
		}

		#region IPaso implementation

		/// <summary>
		/// Origen
		/// </summary>
		public T Origen { get; }

		/// <summary>
		/// Destino
		/// </summary>
		public T Destino { get; }

		/// <summary>
		/// Peso
		/// </summary>
		public float Peso
		{
			get
			{
				return graf [Origen, Destino];
			}
		}

		#endregion
	}
}