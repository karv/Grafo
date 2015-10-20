using Graficas;
using NUnit.Framework;
using System;
using Graficas.Rutas;
using Graficas.Extensiones;

namespace Test
{
	[TestFixture]
	public class GeneralTest
	{
		
		public void GeneraGraficaConexa(IGrafo<int> gr, int cant = 100)
		{
			for (int i = 1; i <= cant; i++)
			{
				gr[0, i] = true;
				gr[i, 0] = true;
			}
			Assert.AreEqual(cant + 1, gr.Nodos.Count);

		}

		[Test]
		public void CaminoOptimo()
		{
			var gr = new Grafo<int>();
			gr.EsSimétrico = true;

			GeneraGraficaConexa(gr);
			var ruta = gr.CaminoÓptimo(2, 3);
			Console.WriteLine(ruta);
			Console.WriteLine(ruta.Longitud);

			foreach (var x in ruta.Pasos)
			{
				Console.WriteLine(x);
			}
		}

		[Test]
		public void TestReversa()
		{
			var gr = new Grafo<int>();
			gr.EsSimétrico = true;

			GeneraGraficaConexa(gr);

			var ruta = new Ruta<int>(0);
			ruta.Concat(1, 1);
			ruta.Concat(0, 1);
			ruta.Concat(2, 1);
			ruta.Concat(0, 1);
			ruta.Concat(3, 1);

			var reversa = ruta.Reversa();
			Assert.AreEqual(ruta.NumPasos, reversa.NumPasos);
			Assert.AreEqual(ruta.Longitud, reversa.Longitud);
			Assert.AreEqual(ruta.NodoInicial, reversa.NodoFinal);
			Assert.AreEqual(reversa.NodoInicial, ruta.NodoFinal);

			Console.WriteLine(string.Format("Normal: \t{0}\nReversa:\t{1}", ruta, reversa));
		}

		[Test]
		public void TestEnumToRuta()
		{
			var gr = new Grafo<int>();
			gr.EsSimétrico = true;

			GeneraGraficaConexa(gr);

			var enume = new System.Collections.Generic.List<int>();
			enume.Add(0);
			enume.Add(1);
			enume.Add(0);
			enume.Add(2);
			enume.Add(0);
			enume.Add(3);
			var ruta = gr.ToRuta(enume);

			Assert.AreEqual(5, ruta.NumPasos);
			Assert.AreEqual(5, ruta.Longitud);
			Assert.AreEqual(0, ruta.NodoInicial);
			Assert.AreEqual(3, ruta.NodoFinal);

			Console.WriteLine(ruta);
		}

		[Test]
		public void TestGrafConexa()
		{
			var gr = new Grafo<int>();
			gr.EsSimétrico = true;
			const int maxNod = 99;
			const int numComp = 3;

			for (int i = 0; i < maxNod - numComp; i++)
			{
				gr[i, i + numComp] = 1;
			}

			var comp = gr.ComponentesConexas();
			Assert.AreEqual(numComp, comp.Count);

			foreach (var item in comp)
			{
				Assert.AreEqual(maxNod / numComp, item.Nodos.Count);
				Console.WriteLine();
				foreach (var z in item.Nodos)
				{
					Console.Write(z + "\t");
				}
			}
		}
	}
}