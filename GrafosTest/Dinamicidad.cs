using Graficas.Grafo.Dinámicos;
using NUnit.Framework;
using System;
using Graficas.Grafo;

namespace Test
{
	[TestFixture]
	public class Dinamicidad
	{
		[Test]
		public void AddNode ()
		{
			var gr = new GrafoVecindad<int> (true);
			gr.AddNode (0);
			Assert.Throws<NodoInexistenteException> (delegate
			{
				gr [1, 0] = true;
			});
			gr.AddNode (1);
			gr [1, 0] = true;
			Assert.True (gr [1, 0]);
		}
	}
}