using System.Collections.Generic;

namespace Graficas.Nodos
{
	/// <summary>
	/// A graph node.
	/// </summary>
	public interface INode<T>
	{
		/// <summary>
		/// Gets the name of the node.
		/// </summary>
		T Item { get; }

		/// <summary>
		/// Gets the neighborhood.
		/// </summary>
		IEnumerable<INode<T>> Neighborhood { get; }
	}
}