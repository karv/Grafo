using System;
using System.Collections.Generic;

namespace Graficas.Edges
{
	/// <summary>
	/// A generic implementation of an immutable step.
	/// </summary>
	[Serializable]
	public struct Step<T> : IStep<T>
	{
		/// <summary>
		/// Gets or sets the origin.
		/// </summary>
		public T Origin { get; }

		/// <summary>
		/// Devuelve el destino de la arista.
		/// </summary>
		public T Destination { get; }

		/// <summary>
		/// Weight of this step.
		/// </summary>
		public float Weight { get; }

		bool IEdge<T>.Exists => true;

		/// <summary>
		/// Initializes a new step.
		/// </summary>
		public Step (T origon, T destination, float weight = 1)
		{
			Origin = origon;
			Destination = destination;
			Weight = weight;
		}

		/// <summary>
		/// Initializes a new step.
		/// </summary>
		/// <param name="edge">A directed edge to clone.</param>
		public Step (IDirectedEdge<T> edge)
			: this (edge.Origin, edge.Destination)
		{
			if (edge is WeightedEdge<T, float> ar)
			{
				if (!ar.Exists)
					throw new ArgumentException ("Cannot initialize a new step from an non-existent edge.", nameof (edge));
				Weight = ar.Data;
			}
		}

		/// <summary>
		/// Initializes a new step.
		/// </summary>
		/// <param name="edge">A directed edge to clone.</param>
		/// <param name="weight">Peso del paso</param>
		public Step (IDirectedEdge<T> edge, float weight)
			: this (edge.Origin, edge.Destination)
		{
			Weight = weight;
		}

		/// <summary>
		/// Initializes a new step.
		/// </summary>
		/// <param name="step">Step to clone.</param>
		public Step (IStep<T> step) : this (step.Origin, step.Destination, step.Weight) { }

		/// <summary>
		/// Determines if this edge matches with the specified endpoints.
		/// </summary>
		public bool Match (T origen, T destino)
		{
			var cmp = EqualityComparer<T>.Default;
			return cmp.Equals (origen, Origin) && cmp.Equals (destino, Destination);
		}

		/// <summary>
		/// Devuelve un par que representa a la arista.
		/// </summary>
		/// <returns>The par.</returns>
		public Tuple<T, T> AsTuple ()
		{
			return new Tuple<T, T> (Origin, Destination);
		}

		/// <summary>
		/// Gets the antipodal node from a specified node, relative to this edge.
		/// </summary>
		/// <param name="node">Node.</param>
		public T Antipode (T node)
		{
			return AsPair ().Excepto (node);
		}

		/// <summary>
		/// Determines whether this edge contains an specified node as an endpoint.
		/// </summary>
		public bool Contains (T node)
		{
			var cmp = EqualityComparer<T>.Default;
			return cmp.Equals (node, Origin) || cmp.Equals (node, Destination);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current paso/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current struct.</returns>
		public override string ToString ()
		{
			return string.Format ("{0} -- [{2}] -> {1}", Origin, Destination, Weight);
		}

		ListasExtra.ParNoOrdenado<T> AsPair () => new ListasExtra.ParNoOrdenado<T> (Origin, Destination);
	}
}