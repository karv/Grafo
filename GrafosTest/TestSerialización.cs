using NUnit.Framework;
using Graficas.Grafo;
using Graficas.Continuo;

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