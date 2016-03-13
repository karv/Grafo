using NUnit.Framework;
using Graficas;

namespace Test
{
	[TestFixture]
	public class HardRutas
	{
		public static HardGrafo<int> HacerGrafo ()
		{
			var gr = new HardGrafo<int> ();
			ConstruirUnaGraf (gr);
			return gr;
		}

		[Test]
		public void TestCtor ()
		{
			var gr = HacerGrafo ();
			var ruta = new HardRuta<int> (gr.AsNodo (0));
			ruta.Concat (1);
			ruta.Concat (0);
			ruta.Concat (2);
			ruta.Concat (0);
			ruta.Concat (3);

			Assert.AreEqual (5, ruta.NumPasos);
			Assert.AreEqual (5, ruta.Longitud);
			Assert.AreEqual (0, ruta.NodoInicial.Objeto);
			Assert.AreEqual (3, ruta.NodoFinal.Objeto);

			var rev = ruta.Reversa ();
			Assert.AreEqual (5, rev.NumPasos);
			Assert.AreEqual (5, rev.Longitud);
			Assert.AreEqual (3, rev.NodoInicial.Objeto);
			Assert.AreEqual (0, rev.NodoFinal.Objeto);

			rev.Concat (ruta);
			Assert.AreEqual (10, rev.NumPasos);
			Assert.AreEqual (10, rev.Longitud);
			Assert.AreEqual (3, rev.NodoInicial.Objeto);
			Assert.AreEqual (3, rev.NodoFinal.Objeto);
		}

		public static void ConstruirUnaGraf (IGrafo<int> gr)
		{

			gr.Clear ();
			for (int i = 0; i < 100; i++)
			{
				gr [0, i] = true;
				gr [i, 0] = true;
			}
		}

	}
}