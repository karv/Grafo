using NUnit.Framework;
using Graficas.Grafo;
using Graficas.Aristas;

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
	}
}