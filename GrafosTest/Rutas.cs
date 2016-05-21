using Xunit;
using Graficas.Grafo;
using Graficas.Rutas;

namespace Test
{
	public class HardRutas
	{
		public static HardGrafo<int> HacerGrafo ()
		{
			var gr = new Grafo<int> ();
			ConstruirUnaGraf (gr);
			for (int i = 1; i < 100; i++)
				Assert.True (gr [0, i], string.Format ("valor de i es {0}", i));
			return new HardGrafo<int> (gr);
		}

		[Fact]
		public void TestCtor ()
		{
			var gr = HacerGrafo ();
			var ruta = new HardRuta<int> (gr.AsNodo (0));
			ruta.Concat (1);
			ruta.Concat (0);
			ruta.Concat (2);
			ruta.Concat (0);
			ruta.Concat (3);

			Assert.Equal (5, ruta.NumPasos);
			Assert.Equal (5, ruta.Longitud);
			Assert.Equal (0, ruta.NodoInicial.Objeto);
			Assert.Equal (3, ruta.NodoFinal.Objeto);

			var rev = ruta.Reversa ();
			Assert.Equal (5, rev.NumPasos);
			Assert.Equal (5, rev.Longitud);
			Assert.Equal (3, rev.NodoInicial.Objeto);
			Assert.Equal (0, rev.NodoFinal.Objeto);

			rev.Concat (ruta);
			Assert.Equal (10, rev.NumPasos);
			Assert.Equal (10, rev.Longitud);
			Assert.Equal (3, rev.NodoInicial.Objeto);
			Assert.Equal (3, rev.NodoFinal.Objeto);
		}

		public static void ConstruirUnaGraf (Grafo<int> gr)
		{

			gr.Clear ();

			for (int i = 0; i < 100; i++)
				gr [i, 0] = true;
		}

	}
}