using NUnit.Framework;
using Graficas.Grafo;
using Graficas.Aristas;
using Graficas.Continuo;
using Graficas.Rutas;

namespace Test
{
	[TestFixture]
	public class TestSerialización
	{
		static void TestSerial<T> (T gr)
			where T : ILecturaGrafo<int>
		{
			Store.BinarySerialization.WriteToBinaryFile ("some.graph", gr);
			var gr2 = Store.BinarySerialization.ReadFromBinaryFile <T> ("some.graph");
			Assert.AreEqual (gr [0, 1], gr2 [0, 1]);
		}

		static void TestSerialPeso<T> (T gr)
			where T : IGrafoPeso<int>
		{
			gr [0, 1] = 1;
			Store.BinarySerialization.WriteToBinaryFile ("some.graph", gr);
			var gr2 = Store.BinarySerialization.ReadFromBinaryFile <T> ("some.graph");
			Assert.AreEqual (gr [0, 1], gr2 [0, 1]);
		}

		[Test]
		public void SerGraf ()
		{
			var gr = new Grafo<int> ();
			gr [0, 1] = 1;
			TestSerial (gr);

			var gr2 = new GrafoAristas<int> ();
			gr2.AgregaArista (new Arista<int> (0, 1, 1));
			TestSerial (gr2);

			var gr3 = new GrafoClan<int> ();
			gr3.AgregaArista (0, 1);
			TestSerial (gr3);

			var gr4 = new HardGrafo<int> (gr);
			TestSerial (gr4);
		}

		[Test]
		public void Cont ()
		{
			var gr = new Grafo<int> ();
			gr [0, 1] = 1;
			var c = new Continuo<int> (gr);
			c.AgregaPunto (0, 1, 0.3f);
			Store.BinarySerialization.WriteToBinaryFile ("continuo", c);
			var c2 = Store.BinarySerialization.ReadFromBinaryFile <Continuo<int>> ("continuo");
			Assert.True (c2.Puntos.Count == c.Puntos.Count);
		}

		[Test]
		public void RutasOpt ()
		{
			var gr = new Grafo<int> ();
			GeneralTest.GeneraGraficaConexa (gr, 5);
			var rut = new ConjuntoRutasÓptimas<int> (gr);
			Store.BinarySerialization.WriteToBinaryFile ("rutas", rut);
			var rut2 = Store.BinarySerialization.ReadFromBinaryFile <ConjuntoRutasÓptimas<int>> ("rutas");
			Assert.NotNull (rut2);
			foreach (var x in gr.Nodos)
			{
				foreach (var y in gr.Nodos)
				{
					var rr = rut2.CaminoÓptimo (x, y);
					Assert.NotNull (rr);
					Assert.AreEqual (x, rr.NodoInicial);
					Assert.AreEqual (y, rr.NodoFinal);
					if (x == y)
						Assert.AreEqual (0, rr.NumPasos);
				}
			}
		}
	}
}