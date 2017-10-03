using System;
using System.Collections.Generic;
using Graficas.Edges;

namespace Graficas.Rutas
{
	/// <summary>
	/// Step by step node comparer.
	/// </summary>
	public class StepPathComparer<T> : IEqualityComparer<IPath<T>>
	{
		/// <summary>
		/// Gets the node comparer,
		/// </summary>
		public IEqualityComparer<T> Comparador { get; }

		/// <summary>
		/// </summary>
		public StepPathComparer ()
		{
			Comparador = EqualityComparer<T>.Default;
		}

		/// <param name="comparer">Node comparer.</param>
		public StepPathComparer (IEqualityComparer<T> comparer)
		{
			Comparador = comparer ?? throw new ArgumentNullException (nameof (comparer));
		}


		/// <summary>
		/// Determines whether two specified paths are equivalent.
		/// </summary>
		public bool Equals (IPath<T> x, IPath<T> y)
		{
			if (x == null || y == null)
				return false;
			if (x.StepCount != y.StepCount)
				return false;
			if (!Comparador.Equals (x.StartNode, y.StartNode))
				return false;
			if (!Comparador.Equals (x.EndNode, y.EndNode))
				return false;

			var enumX = new List<IStep<T>> (x.Steps);
			var enumY = new List<IStep<T>> (y.Steps);

			for (int i = 0; i < x.StepCount; i++)
			{
				if (!Comparador.Equals (enumX[i].Origin, enumY[i].Origin))
					return false;
				if (!Comparador.Equals (enumX[i].Destination, enumY[i].Destination))
					return false;
			}

			return true;
		}

		/// <Docs>The object for which the hash code is to be returned.</Docs>
		/// <para>Returns a hash code for the specified object.</para>
		/// <returns>A hash code for the specified object.</returns>
		/// <summary>
		/// Gets the hash code.
		/// </summary>
		/// <param name="obj">Object.</param>
		public int GetHashCode (IPath<T> obj)
		{
			if (obj == null)
				return 0;
			var ret = 0;
			foreach (var x in obj.Steps)
				ret += Comparador.GetHashCode (x.Origin) + Comparador.GetHashCode (x.Destination);
			return ret;
		}
	}
}