using System;
using System.Collections.Generic;
using System.Linq;
using CE.Graph.Grafo.Estáticos;
using NUnit.Framework;
using CE.Graph.Rutas;

namespace Test
{
	[Serializable]
	public class Objeto : IEquatable<Objeto>
	{
		public readonly int Hash;

		public Objeto (int hash)
		{
			Hash = hash;
		}

		public override bool Equals (object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals (this, obj))
				return true;
			if (obj.GetType () != typeof (Objeto))
				return false;
			var other = (Objeto)obj;
			return Equals (other);
		}

		public bool Equals (Objeto other)
		{
			if (other == null)
				return false;
			return Hash == other.Hash;
		}

		public override int GetHashCode ()
		{
			unchecked
			{
				return Hash;
			}
		}

		public static implicit operator int (Objeto obj)
		{
			return obj.Hash;
		}

		public static implicit operator Objeto (int i)
		{
			return new Objeto (i);
		}

		public override string ToString ()
		{
			return Hash.ToString ();
		}
	}

	/// <summary>
	/// Pruebas de Grafo[1]
	/// </summary>
	[TestFixture]
	public class Graf1
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
			var grafo = new Graph<Objeto> (ObjetoColl);
			grafo[0, 1] = true;
			grafo.Clear ();

			Assert.IsEmpty (grafo.Edges ());
		}

		[Test]
		public void SóloLectura ()
		{
			var grafo = new Graph<Objeto> (ObjetoColl);
			grafo[0, 1] = true;
			var gr2 = new Graph<Objeto> (grafo);
			Assert.Throws<InvalidOperationException> (new TestDelegate (gr2.Clear));
			Assert.Throws<InvalidOperationException> (new TestDelegate (delegate
			{
				gr2[0, 1] = false;
			}));
			Assert.Throws<InvalidOperationException> (new TestDelegate (delegate
			{
				gr2[0, 2] = true;
			}));
		}

		[Test]
		public void CtorClonado ()
		{
			var grafo = new Graph<Objeto> (ObjetoColl);
			grafo[0, 1] = true;
			grafo[0, 2] = true;
			grafo[0, 3] = true;

			var clón = new Graph<Objeto> (grafo);
			Assert.AreEqual (true, clón[0, 1]);
			Assert.AreEqual (true, clón[0, 2]);
			Assert.AreEqual (true, clón[0, 3]);
			Assert.AreEqual (false, clón[1, 2]);
		}

		[Test]
		public void Simetría ()
		{
			var grafo = new Graph<Objeto> (ObjetoColl, true);
			Assert.False (grafo.IsReadOnly);
			grafo[0, 1] = true;
			Assert.True (grafo[0, 1]);
			Assert.True (grafo[1, 0]);
			var ar = grafo.GetEdgeSym (0, 1);
			ar.Exists = false;
			Assert.False (grafo[1, 0]);
		}

		[Test]
		public void ExisteArista ()
		{
			var grafo = new Graph<Objeto> (ObjetoColl);
			grafo[0, 1] = true;
			Assert.True (grafo.EdgeExists (0, 1) && !grafo.EdgeExists (1, 0));
		}

		[Test]
		public void Vecindad ()
		{
			var grafo = new Graph<Objeto> (ObjetoColl);
			grafo[0, 1] = true;
			grafo[0, 2] = true;
			grafo[1, 2] = true;

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
			var grafo = new Graph<Objeto> (ObjetoColl);
			for (int i = 0; i < size; i++)
				grafo[0, i] = true;
			Assert.AreEqual (size, grafo.NodeCount);
			var nods = grafo.Nodes;
			for (int i = 0; i < size; i++)
				Assert.True (Enumerable.Contains<Objeto> (nods, i));
		}

		[Test]
		public void Ruta ()
		{
			// Probar ruta simétrica
			var grafo = new Graph<Objeto> (ObjetoColl, true);
			const int max = 10;
			var enumerador = new Objeto[max];
			for (int i = 0; i < max; i++)
			{
				grafo[i + 1, i] = true;
				enumerador[i] = i;
			}

			var ruta = grafo.ToPath (enumerador);
			Assert.AreEqual (0, ruta.StartNode);
			Assert.AreEqual (max - 1, ruta.EndNode);
			Assert.AreEqual (max - 1, ruta.StepCount);

			enumerador[0] = 3;
			Assert.Throws<InvalidPathOperationException> (new TestDelegate (delegate
			{
				grafo.ToPath (enumerador);
			}));

			// Probar ruta asimétrica
			grafo = new Graph<Objeto> (ObjetoColl);
			for (int i = 0; i < max; i++)
			{
				grafo[i, i + 1] = true;
				enumerador[i] = i;
			}

			ruta = grafo.ToPath (enumerador);
			Assert.AreEqual (0, ruta.StartNode);
			Assert.AreEqual (max - 1, ruta.EndNode);
			Assert.AreEqual (max - 1, ruta.StepCount);

			enumerador[0] = 3;
			Assert.Throws<InvalidPathOperationException> (new TestDelegate (delegate
			{ grafo.ToPath (enumerador); }));
		}

		[Test]
		public void Clonar ()
		{
			var grafo = new Graph<Objeto> (ObjetoColl);
			grafo[0, 1] = true;
			var clón = grafo.Clonar ();
			Assert.True (clón[0, 1]);
			Assert.False (clón[1, 0]);
		}

		[Test]
		public void AsReadonly ()
		{
			var grafo = new Graph<Objeto> (ObjetoColl);
			grafo[0, 1] = true;
			var read = grafo.AsReadonly ();
			Assert.True (read.IsReadOnly);
			Assert.True (read[0, 1]);
		}

		[Test]
		public void Subgrafo ()
		{
			var r = new Random ();
			var max = r.Next (3, size);
			var sim = r.Next (2) == 0;
			var original = new Graph<Objeto> (ObjetoColl, sim);
			for (int i = 0; i < max; i++)
				for (int j = 0; j < max; j++)
					original[i, j] = (r.Next (2) == 0);

			int subcolSize = max / 2;
			var subcol = new Objeto[subcolSize];
			for (int i = 0; i < subcolSize; i++)
				subcol[i] = i;

			var sub = original.GetSubgraph (subcol);
			for (int i = 0; i < subcolSize; i++)
				for (int j = 0; j < subcolSize; j++)
					Assert.AreEqual (original[i, j], sub[i, j]);
		}

		[Test]
		public void Aristas ()
		{
			var gr = new Graph<Objeto> (ObjetoColl);
			gr[0, 1] = true;
			gr[0, 2] = true;

			var aris = gr.Edges ();
			Assert.AreEqual (2, aris.Count);
		}
	}
}