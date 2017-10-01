using System;
using CE.Collections;

namespace Graficas.Edges
{
	/// <summary>
	/// An edge in a graph.
	/// </summary>
	/// <typeparam name="T">Node value type</typeparam>
	public interface IEdge<T> : IClass<T>
	{
		/// <summary>
		/// Determines if this edge match with the specified endpoints.
		/// </summary>
		bool Match (T origin, T destination);

		/// <summary>
		/// Gets a value determining whether this edge exists.
		/// </summary>
		bool Exists { get; }

		/// <summary>
		/// Converts this object to a <see cref="Tuple"/>.
		/// </summary>
		Tuple<T, T> AsTuple ();

		/// <summary>
		/// Gets the antipodal node from a specified node, relative to this edge.
		/// </summary>
		T Antipode (T node);
	}
}