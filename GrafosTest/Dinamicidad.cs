using System;
using CE.Graph.Grafo.Dinámicos;
using NUnit.Framework;

namespace Test
{
	[TestFixture]
	public class Dinamicidad
	{
		[Test]
		public void AddNode ()
		{
			var gr = new NeighborGraph<int> (true);
			gr.AddNode (0);
			Assert.Throws<InvalidOperationException> (delegate
			{ gr[1, 0] = true; });
			gr.AddNode (1);
			gr[1, 0] = true;
			Assert.True (gr[1, 0]);
		}

		[Test]
		public void RemoveNode ()
		{
			var gr = new NeighborGraph<int> ();
			gr.AddNode (0);
			gr.AddNode (1);
			gr[0, 1] = true;
			Assert.True (gr.Neighborhood (0).Contains (1));
			gr.RemoveNode (1);
			Assert.False (gr.Neighborhood (0).Contains (1));
		}
	}
}