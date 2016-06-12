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
		public abstract IGrafo<int> ObtenerGrafoNuevo ();

		public static List<int> GeneralNodos { get; }

		[Test]
		public void SubGrafo ()
		{
			var Gr = ObtenerGrafoNuevo ();
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
	}

	public class GrafoTest : IGrafoTest
	{
		public override IGrafo<int> ObtenerGrafoNuevo ()
		{
			var nodos = new int[30];
			for (int i = 0; i < nodos.Length; i++)
				nodos [i] = i;
			return new Grafo<int> (nodos);
		}
	}

	public class GrafoData : IGrafoTest
	{
		public override IGrafo<int> ObtenerGrafoNuevo ()
		{
			var nodos = new int[30];
			for (int i = 0; i < nodos.Length; i++)
				nodos [i] = i;
			return new Grafo<int, float> (nodos);
		}
	}

	public class GrafoVecindad : IGrafoTest
	{
		public override IGrafo<int> ObtenerGrafoNuevo ()
		{
			var nodos = new int[30];
			for (int i = 0; i < nodos.Length; i++)
				nodos [i] = i;
			return new GrafoVecindad<int> (nodos);
		}
	}

}