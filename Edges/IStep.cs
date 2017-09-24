namespace Graficas.Edges
{
	/// <summary>
	/// Represents a step in a graph path.	
	/// </summary>
	public interface IStep<T> : IDirectedEdge<T>
	{
		/// <summary>
		/// Weight of the step.
		/// </summary>
		float Weight { get; }
	}
}