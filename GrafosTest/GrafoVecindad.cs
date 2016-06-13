using NUnit.Framework;
using System.Collections.Generic;
using Graficas.Grafo;
using System;
using System.Linq;

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
			// Analysis restor	e ObjectCreationAsStatement
		}

		[Test]
		public void Clear ()
		{
			var nods = new int?[30];
			for (int i = 0; i < nods.Length; i++)
				nods [i] = i;
			var g = new GrafoVecindad<int?> (nods, true, new TestComparer ());

			g [0, 1] = true;
			g.Clear ();
			Assert.IsEmpty (g.Vecinos (0));
			Assert.IsEmpty (g.Vecinos (1));
		}

		[Test]
		public void AristasGet ()
		{
			var nods = new int?[30];
			for (int i = 0; i < nods.Length; i++)
				nods [i] = i;
			var r = new Random ();
			var rndN = r.Next (2);
			var g = new GrafoVecindad<int?> (nods, rndN == 1, new TestComparer ());

			Assert.AreEqual (0, g.Aristas ().Count);
			g [0, 1] = true;
			Assert.AreEqual (1 + rndN, g.Aristas ().Count); // Si es simétrica debe devolver dos
		}

		[Test]
		public void Vecinos ()
		{
			var nods = new int?[30];
			for (int i = 0; i < nods.Length; i++)
				nods [i] = i;
			var r = new Random ();
			var rndN = r.Next (2);
			var g = new GrafoVecindad<int?> (nods, rndN == 1, new TestComparer ());

			g [0, 1] = true;
			Assert.AreEqual (1, g.Vecinos (0).Count);
			Assert.AreEqual (rndN, g.Vecinos (1).Count);
			Assert.Throws<NodoInexistenteException> (delegate
			{
				g.Vecinos (nods.Length);
			});
		}

		[Test]
		public void SubGrafo ()
		{
			var nods = new int?[30];
			for (int i = 0; i < nods.Length; i++)
				nods [i] = i;
			var r = new Random ();
			var g = new GrafoVecindad<int?> (nods, false, new TestComparer ());

			for (int i = 0; i < nods.Count (); i++)
				for (int j = 0; j < nods.Count (); j++)
					g [i, j] = r.Next (2) == 0;

			var subnods = new int?[15];
			for (int i = 0; i < subnods.Length; i++)
				subnods [i] = 2 * i;
			var g2 = g.Subgrafo (subnods);
			Assert.AreEqual (subnods.Length, g2.Nodos.Count);
			for (int i = 0; i < subnods.Length; i++)
				for (int j = 0; j < subnods.Count (); j++)
				{
					var ni = subnods [i];
					var nj = subnods [j];
					Assert.AreEqual (
						g [ni, nj],
						g2 [ni, nj],
						string.Format ("i == {0}; j == {1}", ni, nj));
				}

			subnods [0] = nods.Length;
			Assert.Throws<ArgumentException> (delegate
			{
				g.Subgrafo (subnods);
			});
		}
	}
}