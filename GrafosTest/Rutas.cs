using NUnit.Framework;
using Graficas.Rutas;
using Graficas.Aristas;

namespace Test
{
	[TestFixture]
	public class Rutas
	{
		[Test]
		public void ComparaRuta ()
		{
			const int lon = 100;

			var r1 = new Ruta<int> ();
			var r2 = new Ruta<int> ();
			var r3 = new Ruta<int> ();
			for (int i = 0; i < lon; i++)
			{
				r1.Concat (new Paso<int> (i, i + 1, i));
				r2.Concat (new Paso<int> (i, i + 1, i));
				r3.Concat (new Paso<int> (i, i == lon - 1 ? lon + 1 : i + 1, i));
			}

			var compa = new ComparadorPorPaso<int> ();
			Assert.True (compa.Equals (r1, r2));
			Assert.False (compa.Equals (r1, r3));
			Assert.False (compa.Equals (r2, r3));
		}

		[Test]
		public void TestRutaNula ()
		{
			var rn = Ruta<int>.Nulo;
			rn.Concat (new Paso<int> (0, 1));
			Assert.True (rn.NumPasos == 1);
			Assert.AreEqual (rn.NodoInicial, 0);
			Assert.AreEqual (rn.NodoFinal, 1);

			var rn2 = Ruta<int>.Nulo;
			rn2.Concat (rn);
			Assert.True (rn2.NumPasos == 1);

			rn2.Concat (Ruta<int>.Nulo);
			Assert.True (rn2.NumPasos == 1);

			Assert.True (Ruta<int>.RutaNula (null));
			Assert.True (Ruta<int>.RutaNula (Ruta<int>.Nulo));
			Assert.False (Ruta<int>.RutaNula (rn2));
		}

	}
}