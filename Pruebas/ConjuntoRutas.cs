using NUnit.Framework;
using Graficas;

namespace Pruebas
{
	[TestFixture]
	public class ConjuntoRutas
	{
		readonly Grafica<int> gr = new Grafica<int> ();

		[Test]
		public void PruebaBásica ()
		{
			gr.EsSimetrico = true;
			TestCaminoCorto.CrearGráficaPeso (gr);
			var rutas = new ConjuntoRutasÓptimas<int> (gr);
			var r = rutas.CaminoÓptimo (0, 1);
			System.Console.WriteLine (r);
		}
	}
}