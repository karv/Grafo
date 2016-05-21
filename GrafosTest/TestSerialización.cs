using Graficas.Grafo;
using Graficas.Continuo;
using Graficas.Rutas;
using Xunit;

namespace Test
{
	public class TestSerialización
	{
		static void TestSerial<T> (T gr)
			where T : IGrafo<int>
		{
			Store.BinarySerialization.WriteToBinaryFile ("some.graph", gr);
			var gr2 = Store.BinarySerialization.ReadFromBinaryFile <T> ("some.graph");
			Assert.Equal (gr [0, 1], gr2 [0, 1]);
		}

		static void TestSerialPeso<T> (T gr)
			where T : Grafo<int, float>
		{
			gr [0, 1] = 1;
			Store.BinarySerialization.WriteToBinaryFile ("some.graph", gr);
			var gr2 = Store.BinarySerialization.ReadFromBinaryFile <T> ("some.graph");
			Assert.Equal (gr [0, 1], gr2 [0, 1]);
		}

		[Fact]
		public void SerGraf ()
		{
			var gr = new Grafo<int, float> (false, false);
			gr [0, 1] = 1;
			TestSerial (gr);

			var gr3 = new GrafoClan<int> ();
			gr3.AgregaArista (0, 1);
			TestSerial (gr3);

			var gr4 = new HardGrafo<int> (gr);
			TestSerial (gr4);
		}

		[Fact]
		public void Cont ()
		{
			var gr = new Grafo<int, float> (false, false);
			gr [0, 1] = 1;
			var c = new Continuo<int> (gr);
			c.AgregaPunto (0, 1, 0.3f);
			Store.BinarySerialization.WriteToBinaryFile ("continuo", c);
			var c2 = Store.BinarySerialization.ReadFromBinaryFile <Continuo<int>> ("continuo");
			Assert.True (c2.Puntos.Count == c.Puntos.Count);
		}

		[Fact]
		public void RutasOpt ()
		{
		}
	}
}