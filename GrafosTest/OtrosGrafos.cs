using NUnit.Framework;
using Graficas.Grafo;
using System.Linq;

namespace Test
{
	[TestFixture]
	public class OtrosGrafos
	{
		[Test]
		public void HardGrafo () // Faltan pruebas
		{
			var g = new HardGrafo<int> ();
			g [0, 1] = true;
			Assert.True (g [0, 1]);
			g.Clear ();
			Assert.False (g [0, 1]);
			g [0, 1] = true;
			var nods = g.Nodos;
			Assert.True (nods.Contains (0));
			Assert.True (nods.Contains (1));
			Assert.True (g [0].Vecindad.Any ());
			Assert.False (g [1].Vecindad.Any ());
		}
	}
}