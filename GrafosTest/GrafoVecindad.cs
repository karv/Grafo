using NUnit.Framework;
using System.Collections.Generic;
using Graficas.Grafo;
using System;

namespace Test
{
	public class TestComparer : IEqualityComparer<int?>
	{
		public bool Equals (int? x, int? y)
		{
			if (!x.HasValue || !y.HasValue)
				return false;
			return x.Value == y.Value;
		}

		public int GetHashCode (int? obj)
		{
			return obj.HasValue ? obj.Value : 0;
		}
	}

	[TestFixture]
	public class GrafoVecindadTest
	{
		[Test]
		public void Ctor ()
		{
			var r = new Random ();
			var nods = new int?[r.Next (100)];
			for (int i = 0; i < nods.Length; i++)
				nods [i] = 0;
			// Analysis disable ObjectCreationAsStatement
			new GrafoVecindad<int?> (nods, false, new TestComparer ());
			new GrafoVecindad<int?> (nods, true, new TestComparer ());


			for (int i = 0; i < nods.Length; i++)
				nods [i] = i;
			new GrafoVecindad<int?> (nods, false, new TestComparer ());
			new GrafoVecindad<int?> (nods, true, new TestComparer ());
			// Analysis restore ObjectCreationAsStatement
		}
	}
}