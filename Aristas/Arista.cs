using System;

namespace Graficas.Aristas
{
	[Serializable]
	public class Arista<T> : IArista<T>
	{
		/// <summary>
		/// Devuelve el origen de la arista
		/// </summary>
		/// <value>The origen.</value>
		public T Origen { get; }

		/// <summary>
		/// Devuelve el destino de la arista
		/// </summary>
		/// <value>The destino.</value>
		public T Destino { get; }

		/// <summary>
		/// Devuelve el peso de la arista
		/// </summary>
		/// <value>The peso.</value>
		public float Peso { get; }

		public Arista (T origen, T destino, float peso)
		{
			Origen = origen;
			Destino = destino;
			Peso = peso;
		}
	}
}