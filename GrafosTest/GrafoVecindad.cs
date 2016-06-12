using NUnit.Framework;
using System.Collections.Generic;
using Graficas.Grafo;

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
			var nods = new int?[20];
			for (int i = 0; i < nods.Length; i++)
				nods [i] = 0;
			var g = new GrafoVecindad<int?> (nods, false, new TestComparer ());
			Assert.AreEqual (g.);
		}
	}
}