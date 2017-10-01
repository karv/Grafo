using System;
using Graficas.Nodos;

namespace Graficas.Edges
{
	/// <summary>
	/// Edge that preserve references to nodes.
	/// </summary>
	public class HardEdge<T> : IDirectedEdge<T>
		where T : IEquatable<T>
	{
		/// <summary>
		/// Gets the origin.
		/// </summary>
		public Node<T> Origin { get; }

		/// <summary>
		/// Gets the destination.
		/// </summary>
		public Node<T> Destination { get; }

		/// <summary>
		/// Gets 1 if exists, 0 otherwise.
		/// </summary>
		public float Weight => Origin.Neighborhood.Contains (Destination) ? 1 : 0;

		/// <summary>
		/// The existence state of this edge.
		/// </summary>
		public bool Exists => Origin.Neighborhood.Contains (Destination);

		/// <param name="fromNode">Origin.</param>
		/// <param name="toNode">Destination.</param>
		public HardEdge (Node<T> fromNode, Node<T> toNode)
		{
			Origin = fromNode;
			Destination = toNode;
		}

		/// <summary>
		/// Determines whether this edge has the specified end points-
		/// </summary>
		public bool Match (T origen, T destino) => Origin.Item.Equals ((origen)) && Destination.Item.Equals ((destino));


		/// <summary>
		/// Gets a new <see cref="Tuple"/> that represents this edge.
		/// </summary>
		public Tuple<T, T> AsTuple () => new Tuple<T, T> (Origin.Item, Destination.Item);

		/// <summary>
		/// Gets the antipodal node from a specified node, relative to this edge.
		/// </summary>
		public T Antipode (T nodo) => nodo.Equals (Destination.Item) ? Origin.Item : Destination.Item;

		/// <summary>
		/// Determines whether this edge contains the specified node as end point.
		/// </summary>
		public bool Contains (T nodo) => nodo.Equals ((Origin.Item)) || nodo.Equals ((Destination.Item));

		#region IArista

		T IDirectedEdge<T>.Origin { get { return Origin.Item; } }

		T IDirectedEdge<T>.Destination { get { return Destination.Item; } }

		#endregion
	}
}