using System;
using System.Collections.Generic;
using NUnit.Framework;
using CE.Graph.Grafo.Estáticos;
using System.Linq;
using CE.Graph.Rutas;

namespace Test
{
	/// <summary>
	/// Pruebas de Grafo[2]
	/// </summary>
	[TestFixture]
	public class Graf2
	{
		const int size = 30;
		ICollection<Objeto> ObjetoColl;

		[SetUp]
		public void Setup ()
		{
			ObjetoColl = new HashSet<Objeto> ();
			for (int i = 0; i < size; i++)
				ObjetoColl.Add (i);
		}

		[Test]
		public void Clear ()
		{
			var grafo = new Graph<Objeto, float> (ObjetoColl);
			grafo[0, 1] = 1;
			grafo.Clear ();

			Assert.IsEmpty (grafo.Edges ());
		}

		[Test]
		public void SóloLectura ()
		{
			var grafo = new Graph<Objeto, float> (ObjetoColl);
			grafo[0, 1] = 1;
			var gr2 = new Graph<Objeto, float> (grafo);
			Assert.Throws<InvalidOperationException> (new TestDelegate (gr2.Clear));
			Assert.Throws<InvalidOperationException> (new TestDelegate (delegate
			{ gr2[0, 1] = 1; }));
			Assert.Throws<InvalidOperationException> (new TestDelegate (delegate
			{ gr2[0, 2] = 1; }));
		}

		[Test]
		public void CtorClonado ()
		{
			var grafo = new Graph<Objeto, float> (ObjetoColl);
			grafo[0, 1] = 1;
			grafo[0, 2] = 1;
			grafo[0, 3] = 1;

			var clón = new Graph<Objeto, float> (grafo);
			Assert.True (clón.EdgeExists (0, 1));
			Assert.True (clón.EdgeExists (0, 2));
			Assert.True (clón.EdgeExists (0, 3));
			Assert.False (clón.EdgeExists (1, 2));
		}

		[Test]
		public void Simetría ()
		{
			var grafo = new Graph<Objeto, float> (ObjetoColl, true);
			Assert.False (grafo.IsReadOnly);
			grafo[0, 1] = 1;
			Assert.AreEqual (1, grafo[0, 1]);
			Assert.AreEqual (1, grafo[1, 0]);
			var ar = grafo.GetEdgeSym (0, 1);
			ar.Exists = false;
			Assert.False (grafo.EdgeExists (1, 0));
		}

		[Test]
		public void ExisteArista ()
		{
			var grafo = new Graph<Objeto, float> (ObjetoColl);
			grafo[0, 1] = 1;
			Assert.True (grafo.EdgeExists (0, 1) && !grafo.EdgeExists (1, 0));
		}

		[Test]
		public void Vecindad ()
		{
			var grafo = new Graph<Objeto, float> (ObjetoColl);
			grafo[0, 1] = 1;
			grafo[0, 2] = 1;
			grafo[1, 2] = 1;

			Assert.AreEqual (2, grafo.OutwardNeighborhood (0).Count);
			Assert.AreEqual (1, grafo.OutwardNeighborhood (1).Count);
			Assert.AreEqual (0, grafo.OutwardNeighborhood (2).Count);

			Assert.AreEqual (2, grafo.InwardNeighborhood (2).Count);
			Assert.AreEqual (1, grafo.InwardNeighborhood (1).Count);
			Assert.AreEqual (0, grafo.InwardNeighborhood (0).Count);
		}

		[Test]
		public void Nodos ()
		{
			var grafo = new Graph<Objeto, float> (ObjetoColl);
			for (int i = 0; i < size; i++)
				grafo[0, i] = 1;
			Assert.AreEqual (size, grafo.NodeCount);
			var nods = grafo.Nodes;
			for (int i = 0; i < size; i++)
				Assert.True (Enumerable.Contains<Objeto> (nods, i));
		}

		[Test]
		public void Ruta ()
		{
			// Probar ruta simétrica
			var grafo = new Graph<Objeto, float> (ObjetoColl, true);
			const int max = 10;
			var enumerador = new Objeto[max];
			for (int i = 0; i < max; i++)
			{
				grafo[i + 1, i] = 1;
				enumerador[i] = i;
			}

			var ruta = grafo.ToPath (enumerador);
			Assert.AreEqual (0, ruta.StartNode.Hash);
			Assert.AreEqual (max - 1, ruta.EndNode.Hash);
			Assert.AreEqual (max - 1, ruta.StepCount);

			enumerador[0] = 3;
			Assert.Throws<InvalidPathOperationException> (new TestDelegate (delegate
			{ grafo.ToPath (enumerador); }));

			// Probar ruta asimétrica
			grafo = new Graph<Objeto, float> (ObjetoColl);
			for (int i = 0; i < max; i++)
			{
				grafo[i, i + 1] = 1;
				enumerador[i] = i;
			}

			ruta = grafo.ToPath (enumerador);
			Assert.AreEqual (0, ruta.StartNode.Hash);
			Assert.AreEqual (max - 1, ruta.EndNode.Hash);
			Assert.AreEqual (max - 1, ruta.StepCount);

			enumerador[0] = 3;
			Assert.Throws<InvalidPathOperationException> (new TestDelegate (delegate
			{ grafo.ToPath (enumerador); }));
		}

		[Test]
		public void Clonar ()
		{
			var grafo = new Graph<Objeto, float> (ObjetoColl);
			grafo[0, 1] = 1;
			var clón = grafo.Clone ();
			Assert.AreEqual (1, clón[0, 1]);
			Assert.False (clón.EdgeExists (1, 0));
		}

		[Test]
		public void AsReadonly ()
		{
			var grafo = new Graph<Objeto, float> (ObjetoColl);
			grafo[0, 1] = 1;
			var read = grafo.AsReadonly ();
			Assert.True (read.IsReadOnly);
			Assert.AreEqual (1, read[0, 1]);
		}

		[Test]
		public void Subgrafo ()
		{
			var r = new Random ();
			var max = r.Next (3, 100);
			var sim = r.Next (2) == 0;
			var thiscoll = new Objeto[max];
			for (int i = 0; i < max; i++)
				thiscoll[i] = i;
			var original = new Graph<Objeto, float> (thiscoll, sim);
			for (int i = 0; i < max; i++)
				for (int j = 0; j < max; j++)
					original[i, j] = (float)r.NextDouble ();

			int subcolSize = max / 2;
			var subcol = new Objeto[subcolSize];
			for (int i = 0; i < subcolSize; i++)
				subcol[i] = i;

			var sub = original.GetSubgraph (subcol);
			for (int i = 0; i < subcolSize; i++)
				for (int j = 0; j < subcolSize; j++)
				{
					Assert.AreEqual (
					original.EdgeExists (i, j),
					sub.EdgeExists (i, j),
						string.Format (
							"i = {0}\nj = {1}",
							i,
							j));
					if (original.EdgeExists (i, j))
						Assert.AreEqual (original[i, j], sub[i, j]);
				}
		}

		[Test]
		public void Aristas ()
		{
			var gr = new Graph<Objeto, float> (ObjetoColl);
			gr[0, 1] = 1;
			gr[0, 2] = 1;

			var aris = gr.Edges ();
			Assert.AreEqual (2, aris.Count);
		}
	}
}