using System;
using Graficas.Grafo;
using NUnit.Framework;
using Graficas.Aristas;
using Graficas.Rutas;

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
		[Test]
		public void Clear ()
		{
			var grafo = new Grafo<Objeto> ();
			grafo [0, 1] = true;
			grafo.Clear ();

			Assert.IsEmpty (grafo.Aristas ());
			Assert.IsEmpty (grafo.Nodos);
		}

		[Test]
		public void SóloLectura ()
		{
			var grafo = new Grafo<Objeto> ();
			grafo [0, 1] = true;
			var gr2 = new Grafo<Objeto> (grafo);
			Assert.Throws<InvalidOperationException> (new TestDelegate (gr2.Clear));
			Assert.Throws<OperaciónAristaInválidaException> (new TestDelegate (delegate
			{
				gr2 [0, 1] = false;
			}));
			Assert.Throws<OperaciónAristaInválidaException> (new TestDelegate (delegate
			{
				gr2 [0, 2] = true;
			}));
		}

		[Test]
		public void CtorClonado ()
		{
			var grafo = new Grafo<Objeto> ();
			grafo [0, 1] = true;
			grafo [0, 2] = true;
			grafo [0, 3] = true;

			var clón = new Grafo<Objeto> (grafo);
			Assert.AreEqual (true, clón [0, 1]);
			Assert.AreEqual (true, clón [0, 2]);
			Assert.AreEqual (true, clón [0, 3]);
			Assert.AreEqual (false, clón [1, 2]);
		}

		[Test]
		public void Simetría ()
		{
			var grafo = new Grafo<Objeto> (true);
			Assert.False (grafo.SóloLectura);
			grafo [0, 1] = true;
			Assert.True (grafo [0, 1]);
			Assert.True (grafo [1, 0]);
			var ar = grafo.EncuentraArista (0, 1);
			ar.Existe = false;
			Assert.False (grafo [1, 0]);
		}

		[Test]
		public void ExisteArista ()
		{
			var grafo = new Grafo<Objeto> ();
			grafo [0, 1] = true;
			Assert.True (grafo.ExisteArista (0, 1) && !grafo.ExisteArista (1, 0));
		}

		[Test]
		public void Vecindad ()
		{
			var grafo = new Grafo<Objeto> ();
			grafo [0, 1] = true;
			grafo [0, 2] = true;
			grafo [1, 2] = true;

			Assert.AreEqual (2, grafo.Vecino (0).Count);
			Assert.AreEqual (1, grafo.Vecino (1).Count);
			Assert.AreEqual (0, grafo.Vecino (2).Count);

			Assert.AreEqual (2, grafo.AntiVecino (2).Count);
			Assert.AreEqual (1, grafo.AntiVecino (1).Count);
			Assert.AreEqual (0, grafo.AntiVecino (0).Count);
		}

		[Test]
		public void Nodos ()
		{
			var grafo = new Grafo<Objeto> ();
			const int max = 100;
			for (int i = 0; i < max; i++)
				grafo [0, i] = true;
			Assert.AreEqual (max, grafo.NumNodos);
			var nods = grafo.Nodos;
			for (int i = 0; i < max; i++)
				Assert.True (nods.Contains (i));
		}

		[Test]
		public void Ruta ()
		{
			// Probar ruta simétrica
			var grafo = new Grafo<Objeto> (true);
			const int max = 10;
			var enumerador = new Objeto[max];
			for (int i = 0; i < max; i++)
			{
				grafo [i + 1, i] = true;
				enumerador [i] = i;
			}

			var ruta = grafo.ToRuta (enumerador);
			Assert.AreEqual (0, ruta.NodoInicial);
			Assert.AreEqual (max - 1, ruta.NodoFinal);
			Assert.AreEqual (max - 1, ruta.NumPasos);

			enumerador [0] = 3;
			Assert.Throws<RutaInconsistenteException> (new TestDelegate (delegate
			{
				grafo.ToRuta (enumerador);
			}));

			// Probar ruta asimétrica
			grafo = new Grafo<Objeto> ();
			for (int i = 0; i < max; i++)
			{
				grafo [i, i + 1] = true;
				enumerador [i] = i;
			}

			ruta = grafo.ToRuta (enumerador);
			Assert.AreEqual (0, ruta.NodoInicial);
			Assert.AreEqual (max - 1, ruta.NodoFinal);
			Assert.AreEqual (max - 1, ruta.NumPasos);

			enumerador [0] = 3;
			Assert.Throws<RutaInconsistenteException> (new TestDelegate (delegate
			{
				grafo.ToRuta (enumerador);
			}));
		}

		[Test]
		public void Clonar ()
		{
			var grafo = new Grafo<Objeto> ();
			grafo [0, 1] = true;
			var clón = grafo.Clonar ();
			Assert.True (clón [0, 1]);
			Assert.False (clón [1, 0]);
		}

		[Test]
		public void AsReadonly ()
		{
			var grafo = new Grafo<Objeto> ();
			grafo [0, 1] = true;
			var read = grafo.ComoSóloLectura ();
			Assert.True (read.SóloLectura);
			Assert.True (read [0, 1]);
		}

		[Test]
		public void Subgrafo ()
		{
			var r = new Random ();
			var max = r.Next (3, 100);
			var sim = r.Next (2) == 0;
			var original = new Grafo<Objeto> (sim);
			for (int i = 0; i < max; i++)
				for (int j = 0; j < max; j++)
					original [i, j] = (r.Next (2) == 0);

			int subcolSize = max / 2;
			var subcol = new Objeto[subcolSize];
			for (int i = 0; i < subcolSize; i++)
				subcol [i] = i;

			var sub = original.Subgrafo (subcol);
			for (int i = 0; i < subcolSize; i++)
				for (int j = 0; j < subcolSize; j++)
					Assert.AreEqual (original [i, j], sub [i, j]);
		}

		[Test]
		public void Aristas ()
		{
			var gr = new Grafo<Objeto> ();
			gr [0, 1] = true;
			gr [0, 2] = true;

			var aris = gr.Aristas ();
			Assert.AreEqual (2, aris.Count);
		}

		[Test]
		public void CaminoÓptimo ()
		{
			const int clusterSize = 6;
			var gr = new Grafo<Objeto> ();
			for (int i = 0; i < clusterSize; i++)
				for (int j = 0; j < clusterSize; j++)
				{
					gr [i, j] = true;
					gr [i + clusterSize, j + clusterSize] = true;
				}
			gr [0, clusterSize] = true;
			gr [clusterSize, 0] = true;

			var r = gr.CaminoÓptimo (1, clusterSize + 1);
			Assert.AreEqual (1, r.NodoInicial);
			Assert.AreEqual (clusterSize + 1, r.NodoFinal);
			foreach (var x in r.Pasos)
				Assert.True (gr.ExisteArista (x.Origen, x.Destino));
			Assert.AreEqual (3, r.NumPasos);
			foreach (var x in r.Pasos)
				Console.WriteLine (string.Format ("{0} -> {1}", x.Origen, x.Destino));

			r = gr.CaminoÓptimo (0, 0);
			Assert.IsNull (r);

			r = gr.CaminoÓptimo (0, -1);
			Assert.IsNull (r);
		}
	}
}