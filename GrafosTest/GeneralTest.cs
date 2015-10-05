using Graficas;
using NUnit.Framework;
using System;

namespace Test
{
	[TestFixture]
	public class GeneralTest
	{
		
		public void GeneraGraficaConexa(ILecturaGrafo<int> gr, int cant = 100)
		{
			for (int i = 1; i <= cant; i++)
			{//TODO
				//gr.AgregaArista(0, i);
				//gr.AgregaArista(i, 0);
			}
			Assert.AreEqual(cant + 1, gr.Nodos.Count);

		}

		[Test]
		public void CaminoOptimo()
		{
			var gr = new Grafo<int>();
			gr.EsSimetrico = true;

			GeneraGraficaConexa(gr);
			var ruta = gr.CaminoÓptimo(2, 3);
			Console.WriteLine(ruta);
			Console.WriteLine(ruta.Longitud);

			foreach (var x in ruta.Pasos)
			{
				Console.WriteLine(x);
			}
		}
	}
}

