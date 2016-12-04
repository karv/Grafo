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

		[Test]
		public void RemoveNode ()
		{
			var gr = new GrafoVecindad<int> ();
			gr.AddNode (0);
			gr.AddNode (1);
			gr [0, 1] = true;
			Assert.True (gr.Vecinos (0).Contains (1));
			gr.RemoveNode (1);
			Assert.False (gr.Vecinos (0).Contains (1));
		}
	}
}