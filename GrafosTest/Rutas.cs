using NUnit.Framework;
using Graficas.Rutas;
using Graficas.Aristas;

namespace Test.Properties
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
	}
}