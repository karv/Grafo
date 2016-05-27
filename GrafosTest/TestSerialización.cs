using NUnit.Framework;
using Graficas.Grafo;
using Graficas.Continuo;
using Graficas.Rutas;

namespace Test
{
	[TestFixture]
	public class TestSerialización
	{
		static void TestSerial<T> (T gr)
			where T : IGrafo<Objeto>
		{
			Store.BinarySerialization.WriteToBinaryFile ("some.graph", gr);
			var gr2 = Store.BinarySerialization.ReadFromBinaryFile <T> ("some.graph");
			Assert.AreEqual (gr [0, 1].Existe, gr2 [0, 1].Existe);
		}

		static void TestSerialPeso<T> (T gr)
			where T : Grafo<Objeto, float>
		{
			gr [0, 1] = 1;
			Store.BinarySerialization.WriteToBinaryFile ("some.graph", gr);
			var gr2 = Store.BinarySerialization.ReadFromBinaryFile <T> ("some.graph");
			Assert.AreEqual (gr [0, 1], gr2 [0, 1]);
		}

		[Test]
		public void SerGraf ()
		{
			var gr = new Grafo<Objeto, float> ();
			gr [0, 1] = 1;
			TestSerial (gr);

			var gr4 = new HardGrafo<Objeto> (gr);
			TestSerial (gr4);
		}

		[Test]
		public void PathSet ()
		{
			var gr = new Grafo<Objeto, float> ();
			gr [0, 1] = 1;
			gr [1, 2] = 2;
			gr [2, 3] = 3;

			var rr = new ConjuntoRutasÓptimas<Objeto> (gr);
			var zero3 = rr.CaminoÓptimo (0, 3);

			Store.BinarySerialization.WriteToBinaryFile ("some.graph", rr);
			var rr2 = Store.BinarySerialization.ReadFromBinaryFile <ConjuntoRutasÓptimas<Objeto>> ("some.graph");
			var copia = rr2.CaminoÓptimo (0, 3);
			Assert.AreEqual (zero3.NumPasos, copia.NumPasos);
			Assert.AreEqual (zero3.NodoInicial, copia.NodoInicial);
			Assert.AreEqual (zero3.NodoFinal, copia.NodoFinal);
		}

		[Test]
		public void Cont ()
		{
			var gr = new Grafo<Objeto, float> (true);
			gr [0, 1] = 1;
			var c = new Continuo<Objeto> (gr);
			c.AgregaPunto (0, 1, 0.3f);
			Store.BinarySerialization.WriteToBinaryFile ("continuo", c);
			var c2 = Store.BinarySerialization.ReadFromBinaryFile <Continuo<Objeto>> ("continuo");
			Assert.True (c2.Puntos.Count == c.Puntos.Count);
		}

	}
}