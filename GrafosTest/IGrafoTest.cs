using NUnit.Framework;
using Graficas.Grafo;
using System;
using System.Collections.Generic;
using System.Linq;
using Test;
using System.Collections;

namespace Test
{
	[TestFixture]
	public abstract class IGrafoTest
	{
		public IGrafo<int> Gr { get; set; }

		public abstract void Reset ();

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
	}

	public class GrafoTest : IGrafoTest
	{
		public override void Reset ()
		{
			var nodos = new int[30];
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
		public override void Reset ()
		{
			var nodos = new int[30];
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
		public override void Reset ()
		{
			var nodos = new int[30];
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