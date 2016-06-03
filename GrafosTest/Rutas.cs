using System;
using NUnit.Framework;
using Graficas.Rutas;
using Graficas.Aristas;

namespace Test
{
	[TestFixture]
	public class Rutas
	{
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
		}
	}
}