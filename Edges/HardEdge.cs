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
		public Nodo<T> Origin { get; }

		/// <summary>
		/// Gets the destination.
		/// </summary>
		public Nodo<T> Destination { get; }

		/// <summary>
		/// Gets 1 if exists, 0 otherwise.
		/// </summary>
		public float Weight => Origin.Vecindad.Contains (Destination) ? 1 : 0;

		/// <summary>
		/// The existence state of this edge.
		/// </summary>
		public bool Exists => Origin.Vecindad.Contains (Destination);

		/// <param name="fromNode">Origin.</param>
		/// <param name="toNode">Destination.</param>
		public HardEdge (Nodo<T> fromNode, Nodo<T> toNode)
		{
			Origin = fromNode;
			Destination = toNode;
		}

		/// <summary>
		/// Determines whether this edge has the specified end points-
		/// </summary>
		public bool Match (T origen, T destino) => Origin.Objeto.Equals ((origen)) && Destination.Objeto.Equals ((destino));


		/// <summary>
		/// Gets a new <see cref="Tuple"/> that represents this edge.
		/// </summary>
		public Tuple<T, T> AsTuple () => new Tuple<T, T> (Origin.Objeto, Destination.Objeto);

		/// <summary>
		/// Gets the antipodal node from a specified node, relative to this edge.
		/// </summary>
		public T Antipode (T nodo) => nodo.Equals (Destination.Objeto) ? Origin.Objeto : Destination.Objeto;

		/// <summary>
		/// Determines whether this edge contains the specified node as end point.
		/// </summary>
		public bool Contains (T nodo) => nodo.Equals ((Origin.Objeto)) || nodo.Equals ((Destination.Objeto));

		#region IArista

		T IDirectedEdge<T>.Origin { get { return Origin.Objeto; } }

		T IDirectedEdge<T>.Destination { get { return Destination.Objeto; } }

		#endregion
	}
}