namespace Graficas.Aristas
{
	/// <summary>
	/// Representa un paso en una ruta de un grafo.
	/// </summary>
	public interface IStep<T> : IDirectedEdge<T>
	{
		/// <summary>
		/// Peso del paso
		/// </summary>
		float Peso { get; }
	}
}