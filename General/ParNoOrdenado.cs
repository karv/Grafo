using System;
using System.Collections.Generic;

namespace Graficas.Misc
{
	public class ParNoOrdenado<T> : Tuple<T, T>, IEquatable<Tuple<T, T>>, IEquatable<ParNoOrdenado<T>>
	{
		public class Comparador : IEqualityComparer<ParNoOrdenado<T>>
		{
			#region IEqualityComparer implementation

			// Analysis disable MemberHidesStaticFromOuterClass
			bool IEqualityComparer<ParNoOrdenado<T>>.Equals(ParNoOrdenado<T> x, ParNoOrdenado<T> y)
			// Analysis restore MemberHidesStaticFromOuterClass
			{
				return x.Equals(y);
			}

			int IEqualityComparer<ParNoOrdenado<T>>.GetHashCode(ParNoOrdenado<T> obj)
			{
				return base.GetHashCode();
			}

			#endregion
			
		}

		public ParNoOrdenado(T item0, T item1) : base(item0, item1)
		{
		}

		#region IEquatable implementation

		public bool Equals(Tuple<T, T> other)
		{
			return 
				(Item1.Equals(other.Item1) && Item2.Equals(other.Item2)) ||
			(Item1.Equals(other.Item2) && Item2.Equals(other.Item1));
		}

		public bool Equals(ParNoOrdenado<T> other)
		{
			return 
				(Item1.Equals(other.Item1) && Item2.Equals(other.Item2)) ||
			(Item1.Equals(other.Item2) && Item2.Equals(other.Item1));
		}

		#endregion
	}
}

