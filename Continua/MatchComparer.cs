using System;
using System.Collections.Generic;

namespace Graficas.Continua
{
	/// <summary>
	/// Compares points in a continuum.
	/// </summary>
	[Serializable]
	public class MatchComparer<T> : IEqualityComparer<Punto<T>>
	{
		/// <summary>
		/// Gets the node comparer for end points.
		/// </summary>
		public IEqualityComparer<T> NodeComparer { get; }

		/// <param name="comparer">Node comparer. If <c>null</c>, default will be used instead.</param>
		public MatchComparer (IEqualityComparer<T> comparer = null)
		{
			NodeComparer = comparer ?? EqualityComparer<T>.Default;
		}

		/// <summary>
		/// Determines whether two specified <see cref="Punto{T}"/> are in the same <c>place</c>.
		/// </summary>
		/// <param name="x">The first point.</param>
		/// <param name="y">The second point.</param>
		public bool Equals (Punto<T> x, Punto<T> y)
		{
			if (ReferenceEquals (null, x) || ReferenceEquals (null, y))
				return false;

			return x.Coincide (y);
		}

		/// <summary>
		/// Gets the hash code.
		/// </summary>
		public int GetHashCode (Punto<T> obj)
		{
			return obj.EnOrigen ? NodeComparer.GetHashCode (obj.A) :
				NodeComparer.GetHashCode (obj.A) + NodeComparer.GetHashCode (obj.B) + obj.Loc.GetHashCode ();
		}
	}
}