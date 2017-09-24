namespace Graficas.Edges
{
	/// <summary>
	/// An edge is a direction.
	/// </summary>
	public interface IDirectedEdge<T> : IEdge<T>
	{
		/// <summary>
		/// Gets or sets the origin.
		/// </summary>
		T Origin { get; }

		/// <summary>
		/// Devuelve el destino de la arista
		/// </summary>
		T Destination { get; }
	}
}