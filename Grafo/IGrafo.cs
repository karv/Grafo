namespace Graficas.Grafo
{
	/// <summary>
	/// Un grafo en el que se pueden leer y escribir nodos y aristas
	/// </summary>
	public interface IGrafo<T> : ILecturaGrafo<T>
	{
		/// <summary>
		/// Elimina nodos y aristas de este grafo.
		/// </summary>
		void Clear ();

		/// <summary>
		/// La arista correspondiente a un par de puntos
		/// </summary>
		/// <param name="desde">Origen</param>
		/// <param name="hasta">Destino</param>
		new bool this [T desde, T hasta]{ get; set; }
	}
}