using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using CE.Graph.Grafo;
using CE.Graph.Grafo.Estáticos;

namespace Test
{
	[TestFixture]
	public abstract class IGrafoTest
	{
		public IGraph<int> Gr { get; set; }

		public int RealNumNodos { get; set; }

		static Random r = new Random ();

		public void HacerAleatorio ()
		{
			Gr.Clear ();
			var nods = new List<int> (Gr.Nodes);

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

		void HacerLineal ()
		{
			Reset ();
			var n = Gr.NodeCount;
			for (int i = 0; i < n - 1; i++)
				SetValue (i, i + 1, true);
		}

		public abstract void Reset (int size);

		public void Reset ()
		{
			RealNumNodos = r.Next (3, 100);
			Reset (RealNumNodos);
		}

		[SetUp]
		public void Setup ()
		{
			Reset ();
		}

		public abstract void SetValue (int a, int b, bool r);

		public static List<int> GeneralNodos { get; }

		[Test]
		public void SubGrafo ()
		{
			var hs = new List<int> (Gr.Nodes.Where (z => r.Next (2) == 1));

			var g2 = Gr.Subgraph (hs);
			Assert.AreEqual (hs.Count, g2.NodeCount);
			for (int i = 0; i < hs.Count; i++)
				for (int j = 0; j < hs.Count; j++)
				{
					var ni = hs[i];
					var nj = hs[j];
					Assert.AreEqual (
						Gr[ni, nj].Exists,
						g2[ni, nj].Exists,
						string.Format ("i == {0}; j == {1}", ni, nj));
				}

			while (Gr.Nodes.Contains (hs[0]))
				hs[0]++;

			Assert.Throws<ArgumentException> (delegate
			{ Gr.Subgraph (hs); });
		}

		[Test]
		public void Clear ()
		{
			SetValue (0, 1, true);
			Assert.True (Gr[0, 1].Exists);
			Gr.Clear ();
			Assert.False (Gr[0, 1].Exists);
			Assert.IsEmpty (Gr.Neighborhood (0));
			Assert.IsEmpty (Gr.Neighborhood (1));
			Assert.IsEmpty (Gr.Edges ());
		}

		[Test]
		public void AristasVsGetter ()
		{
			HacerAleatorio ();
			var ar = Gr.Edges ();
			var ls = new List<int> (Gr.Nodes);
			for (int i = 0; i < ls.Count; i++)
				for (int j = 0; j < ls.Count; j++)
				{
					var a = Gr[i, j];
					if (a.Exists)
					{
						Assert.True (a.Match (i, j));
						Assert.True (ar.Contains (a));
					}
					else
					{
						Assert.False (ar.Contains (a));
					}

				}
		}

		[Test]
		public void Path ()
		{
			HacerLineal ();
			var n = Gr.NodeCount;
			var rr = new int[n];
			for (int i = 0; i < n; i++)
				rr[i] = i;
			var ruta = Gr.ToPath (rr);

			Assert.AreEqual (0, ruta.StartNode);
			Assert.AreEqual (n - 1, ruta.EndNode);
			Assert.AreEqual (n - 1, ruta.StepCount);
			foreach (var x in ruta.Steps)
			{
				var ar = Gr[x.Origin, x.Destination];
				Assert.True (ar.Exists);
			}
		}

		[Test]
		public void Nodos ()
		{
			Assert.AreEqual (RealNumNodos, Gr.NodeCount);
			for (int i = 0; i < RealNumNodos; i++)
				Assert.True (Gr.Nodes.Contains (i));
		}
	}

	public class GrafoTest : IGrafoTest
	{
		public override void Reset (int size)
		{
			var nodos = new int[size];
			for (int i = 0; i < nodos.Length; i++)
				nodos[i] = i;
			Gr = new Graph<int> (nodos);
		}

		public override void SetValue (int a, int b, bool r)
		{
			var graf = (Graph<int>)(Gr);
			graf[a, b] = r;
		}
	}

	public class GrafoData : IGrafoTest
	{
		public override void Reset (int size)
		{
			var nodos = new int[size];
			for (int i = 0; i < nodos.Length; i++)
				nodos[i] = i;
			Gr = new Graph<int, float> (nodos);
		}

		public override void SetValue (int a, int b, bool r)
		{
			var graf = (Graph<int, float>)(Gr);
			graf.FindEdge (a, b).Exists = r;
		}
	}

	public class GrafoVecindad : IGrafoTest
	{
		public override void Reset (int size)
		{
			var nodos = new int[size];
			for (int i = 0; i < nodos.Length; i++)
				nodos[i] = i;
			Gr = new Graph<int> (nodos);
		}

		public override void SetValue (int a, int b, bool r)
		{
			var graf = (Graph<int>)(Gr);
			graf[a, b] = r;
		}
	}
}