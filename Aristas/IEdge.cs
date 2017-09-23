using System;
using CE.Collections;

namespace Graficas.Aristas
{
	/// <summary>
	/// An edge in a graph.
	/// </summary>
	/// <typeparam name="T">Node value type</typeparam>
	public interface IEdge<T> : IClass<T>
	{
		/// <summary>
		/// Si esta arista coincide con extremos
		/// </summary>
		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		bool Match (T origen, T destino);

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
		/// <param name="nodo">Node.</param>
		T Antipode (T nodo);
	}

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