using System;
using System.Collections.Generic;

namespace Graficas.Continuo
{
	public class ComparadorCoincidencia<T> : IEqualityComparer<Continuo<T>.ContinuoPunto>
		where T : class, IEquatable<T>
	{
		public ComparadorCoincidencia (IEqualityComparer<T> compa = null)
		{
			ComparaNodos = compa ?? EqualityComparer<T>.Default;
		}

		IEqualityComparer<T> ComparaNodos { get; }

		public bool Equals (Continuo<T>.ContinuoPunto x, Continuo<T>.ContinuoPunto y)
		{
			if (ReferenceEquals (null, x) || ReferenceEquals (null, y))
				return false;

			return x.Coincide (y);
		}

		public int GetHashCode (Continuo<T>.ContinuoPunto obj)
		{
			return obj.EnOrigen ? ComparaNodos.GetHashCode (obj.A) : 
				ComparaNodos.GetHashCode (obj.A) + ComparaNodos.GetHashCode (obj.B) + obj.Loc.GetHashCode ();
		}
	}
}