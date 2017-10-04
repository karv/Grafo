using System;

namespace CE.Graph.Edges
{
	/// <summary>
	/// Representa una arista de valor booleano
	/// </summary>
	[Serializable]
	public class ExistentialEdge<TNodo> : IDirectedEdge<TNodo>
	{
		/// <summary>
		/// Gets or sets the origin.
		/// </summary>
		public TNodo Origin
		{
			get => _origin;
			set
			{
				if (Readonly)
					throw new InvalidOperationException ("Edge is read only.");
				_origin = value;
			}
		}

		/// <summary>
		/// Gets or sets the destination.
		/// </summary>
		public TNodo Destination
		{
			get
			{
				return _destination;
			}
			set
			{
				if (Readonly)
					throw new InvalidOperationException ("Edge is readonly.");
				_destination = value;
			}
		}

		/// <summary>
		/// Gets a value determining whether this edge exists.
		/// </summary>
		public bool Exists
		{
			get => _exists;
			set
			{
				if (Readonly)
					throw new InvalidOperationException ("Edge is readonly.");
				_exists = value;
			}
		}

		/// <summary>
		/// Gets <c>true</c> if this is a symetrical edge.
		/// </summary>
		public bool IsSymmetric { get; }

		/// <summary>
		/// Indica si la arista es sólo lectura.
		/// </summary>
		public bool Readonly { get; }

		/// <param name="origin">Origin node</param>
		/// <param name="destination">Destination node.</param>
		/// <param name="exists">If set to <c>true</c>, it exists.</param>
		/// <param name="readOnly">If <c>true</c>, this edge cannot be modified.</param>
		/// <param name="isSymmetric">If <c>true</c>, this edge is commutative.</param>
		public ExistentialEdge (
			TNodo origin,
			TNodo destination,
			bool exists = true,
			bool readOnly = false,
			bool isSymmetric = false)
		{
			Origin = origin;
			Destination = destination;
			Exists = exists;
			Readonly = readOnly;
			IsSymmetric = isSymmetric;
		}

		/// <summary>
		/// Determines whether this is an edge with specified vertices.
		/// </summary>
		public bool Match (TNodo origin, TNodo destination)
		{
			return (_origin.Equals (origin) && _destination.Equals (destination)) ||
			((IsSymmetric) && (_origin.Equals (destination) && _destination.Equals (origin)));
		}

		/// <summary>
		/// Converts this object to a <see cref="Tuple"/>.
		/// </summary>
		public Tuple<TNodo, TNodo> AsTuple ()
		{
			return new Tuple<TNodo, TNodo> (Origin, Destination);
		}

		/// <summary>
		/// Gets the antipodal node from a specified node, relative to this edge.
		/// </summary>
		/// <param name="nodo">Node.</param>
		public TNodo Antipode (TNodo nodo) => nodo.Equals (Origin) ? Destination : Origin;

		/// <summary>
		/// Devuelve si esta arista toca a un nodo dado
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		public bool Contains (TNodo nodo) => nodo.Equals (Origin) || nodo.Equals (Destination);

		/// <returns>A <see cref="System.String"/> that represents the current class</returns>
		public override string ToString ()
		{
			string formato = "{0} " + (IsSymmetric ? "<" : "-") + "-> {1}";
			return string.Format (formato, Origin, Destination);
		}

		TNodo _origin;
		TNodo _destination;
		bool _exists;
	}
}