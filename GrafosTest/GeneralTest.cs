using NUnit.Framework;
using Graficas.Rutas;
using Graficas.Extensiones;
using Graficas.Grafo;
using System;

namespace Test
{
	[TestFixture]
	public class GeneralTest
	{
		
		static public void GeneraGraficaConexa (Grafo<int, bool> gr, int cant = 100)
		{
			for (int i = 1; i <= cant; i++)
			{
				gr [0, i] = true;
				gr [i, 0] = true;
			}
			Assert.AreEqual (cant + 1, gr.Nodos.Count);

		}

		[Test]
		public void CaminoOptimo ()
		{
			// TODO, rehacer 
			var gr = new Grafo<int, float> (true);

			var ruta = gr.CaminoÓptimo (2, 3, z => z.Data);
			Console.WriteLine (ruta);

			foreach (var x in ruta.Pasos)
				Console.WriteLine (x);
		}

		static public void TestToRuta (IGrafo<int> gr)
		{
			var enume = new System.Collections.Generic.List<int> ();
			enume.Add (0);
			enume.Add (1);
			enume.Add (0);
			enume.Add (2);
			enume.Add (0);
			enume.Add (3);
			var ruta = gr.ToRuta (enume);

			Assert.AreEqual (5, ruta.NumPasos);
			Assert.AreEqual (0, ruta.NodoInicial);
			Assert.AreEqual (3, ruta.NodoFinal);

			Console.WriteLine (ruta);
		}

		[Test]
		public void TestEnumToRuta ()
		{
			var gr = new Grafo<int, bool> (true);

			GeneraGraficaConexa (gr);
			TestToRuta (gr);
		}

		static public void TestConexidad (Grafo<int, bool> gr)
		{
			var r = new Random ();
			int numComp = r.Next (5);
			int maxNod = r.Next (20) * numComp;

			for (int i = 0; i < maxNod - numComp; i++)
			{
				gr [i, i + numComp] = true;
			}

			var comp = gr.ComponentesConexas ();
			Assert.AreEqual (numComp, comp.Count);

			foreach (var item in comp)
			{
				Assert.AreEqual (maxNod / numComp, item.Nodos.Count);
				Console.WriteLine ();
				foreach (var z in item.Nodos)
				{
					Console.Write (z + "\t");
				}
			}
		}

		[Test]
		public void TestGrafConexa ()
		{
			var gr = new Grafo<int, bool> (true);
			TestConexidad (gr);
		}

		[Test]
		public void TestSubgrafo ()
		{
			var gr = new HardGrafo<int> ();
			gr [6, 5] = true;
			gr [6, 1] = true;
			gr [1, 5] = true;
			gr [3, 5] = true;
			gr [1, 3] = true;
			gr [7, 3] = true;
			gr [5, 2] = true;
			gr [1, 4] = true;
			gr [1, 7] = true;
			gr [2, 7] = true;
			gr [7, 4] = true;
			gr [5, 7] = true;

			gr [5, 6] = true;
			gr [1, 6] = true;
			gr [5, 1] = true;
			gr [5, 3] = true;
			gr [3, 1] = true;
			gr [3, 7] = true;
			gr [2, 5] = true;
			gr [4, 1] = true;
			gr [7, 1] = true;
			gr [7, 2] = true;
			gr [7, 4] = true;
			gr [7, 5] = true;

			int [] sub = { 1, 3, 5, 7 };
			var subg = gr.Subgrafo (sub);

			foreach (var x in sub)
			{
				Assert.AreEqual (3, subg.AsNodo (x).Vecindad.Count);
			}
		}
	}
}