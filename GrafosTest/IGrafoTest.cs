using NUnit.Framework;
using Graficas.Grafo;
using System;
using System.Collections.Generic;
using System.Linq;
using Test;

namespace Test
{
	[TestFixture]
	public abstract class IGrafoTest
	{
		public IGrafo<int> Gr { get; set; }

		static Random r = new Random ();

		public void HacerAleatorio ()
		{
			Gr.Clear ();
			var nods = new List<int> (Gr.Nodos);

			for (int i = 0; i < nods.Count; i++)
				for (int j = 0; j < nods.Count; j++)
					SetValue (i, j, r.Next (2) == 1);
		}

		public void HacerCluster ()
		{
			int clusterSize = r.Next (2, 10);
			Reset (2 * clusterSize);
			for (int i = 0; i < clusterSize; i++)
				for (int j = 0; j < clusterSize; j++)
				{
					SetValue (i, j, true);
					SetValue (i + clusterSize, j + clusterSize, true);
				}
			SetValue (0, clusterSize, true);
			SetValue (clusterSize, 0, true);
		}

		public abstract void Reset (int size);

		[SetUp]
		public void Setup ()
		{
			
			Reset (r.Next (3, 100));
		}

		public abstract void SetValue (int a, int b, bool r);

		public static List<int> GeneralNodos { get; }

		[Test]
		public void SubGrafo ()
		{
			var r = new Random ();
			var hs = new List<int> (Gr.Nodos.Where (z => r.Next (2) == 1));

			var g2 = Gr.Subgrafo (hs);
			Assert.AreEqual (hs.Count, g2.Nodos.Count);
			for (int i = 0; i < hs.Count; i++)
				for (int j = 0; j < hs.Count; j++)
				{
					var ni = hs [i];
					var nj = hs [j];
					Assert.AreEqual (
						Gr [ni, nj].Existe,
						g2 [ni, nj].Existe,
						string.Format ("i == {0}; j == {1}", ni, nj));
				}

			while (Gr.Nodos.Contains (hs [0]))
				hs [0]++;

			Assert.Throws<OperaciónInválidaGrafosException> (delegate
			{
				Gr.Subgrafo (hs);
			});
		}

		[Test]
		public void Clear ()
		{
			SetValue (0, 1, true);
			Assert.True (Gr [0, 1].Existe);
			Gr.Clear ();
			Assert.False (Gr [0, 1].Existe);
			Assert.IsEmpty (Gr.Vecinos (0));
			Assert.IsEmpty (Gr.Vecinos (1));
			Assert.IsEmpty (Gr.Aristas ());
		}

		[Test]
		public void AristasVsGetter ()
		{
			HacerAleatorio ();
			var ar = Gr.Aristas ();
			var ls = new List<int> (Gr.Nodos);
			for (int i = 0; i < ls.Count; i++)
				for (int j = 0; j < ls.Count; j++)
				{
					var a = Gr [i, j];
					if (a.Existe)
					{
						Assert.True (a.Coincide (i, j));
						Assert.True (ar.Contains (a));
					}
					else
					{
						Assert.False (ar.Contains (a));
					}

				}
		}
	}

	public class GrafoTest : IGrafoTest
	{
		public override void Reset (int size)
		{
			var nodos = new int[size];
			for (int i = 0; i < nodos.Length; i++)
				nodos [i] = i;
			Gr = new Grafo<int> (nodos);
		}

		public override void SetValue (int a, int b, bool r)
		{
			var graf = (Grafo<int>)(Gr);
			graf [a, b] = r;
		}
	}

	public class GrafoData : IGrafoTest
	{
		public override void Reset (int size)
		{
			var nodos = new int[size];
			for (int i = 0; i < nodos.Length; i++)
				nodos [i] = i;
			Gr = new Grafo<int, float> (nodos);
		}

		public override void SetValue (int a, int b, bool r)
		{
			var graf = (Grafo<int, float>)(Gr);
			graf.EncuentraArista (a, b).Existe = r;
		}
	}

	public class GrafoVecindad : IGrafoTest
	{
		public override void Reset (int size)
		{
			var nodos = new int[size];
			for (int i = 0; i < nodos.Length; i++)
				nodos [i] = i;
			Gr = new Grafo<int> (nodos);
		}

		public override void SetValue (int a, int b, bool r)
		{
			var graf = (Grafo<int>)(Gr);
			graf [a, b] = r;
		}
	}

}