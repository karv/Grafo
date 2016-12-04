using System;
using System.Collections.Generic;
using Graficas.Aristas;
using Graficas.Grafo;
using Graficas.Grafo.Estáticos;
using Graficas.Rutas;
using NUnit.Framework;

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

		[TestFixtureSetUp]
		public void Setup ()
		{
			ObjetoColl = new HashSet<Objeto> ();
			for (int i = 0; i < size; i++)
				ObjetoColl.Add (i);
		}

		[Test]
		public void Clear ()
		{
			var grafo = new Grafo<Objeto, float> (ObjetoColl);
			grafo [0, 1] = 1;
			grafo.Clear ();

			Assert.IsEmpty (grafo.Aristas ());
		}

		[Test]
		public void SóloLectura ()
		{
			var grafo = new Grafo<Objeto, float> (ObjetoColl);
			grafo [0, 1] = 1;
			var gr2 = new Grafo<Objeto, float> (grafo);
			Assert.Throws<InvalidOperationException> (new TestDelegate (gr2.Clear));
			Assert.Throws<OperaciónAristaInválidaException> (new TestDelegate (delegate
			{
				gr2 [0, 1] = 1;
			}));
			Assert.Throws<OperaciónAristaInválidaException> (new TestDelegate (delegate
			{
				gr2 [0, 2] = 1;
			}));
		}

		[Test]
		public void CtorClonado ()
		{
			var grafo = new Grafo<Objeto, float> (ObjetoColl);
			grafo [0, 1] = 1;
			grafo [0, 2] = 1;
			grafo [0, 3] = 1;

			var clón = new Grafo<Objeto, float> (grafo);
			Assert.True (clón.ExisteArista (0, 1));
			Assert.True (clón.ExisteArista (0, 2));
			Assert.True (clón.ExisteArista (0, 3));
			Assert.False (clón.ExisteArista (1, 2));
		}

		[Test]
		public void Simetría ()
		{
			var grafo = new Grafo<Objeto, float> (ObjetoColl, true);
			Assert.False (grafo.SóloLectura);
			grafo [0, 1] = 1;
			Assert.AreEqual (1, grafo [0, 1]);
			Assert.AreEqual (1, grafo [1, 0]);
			var ar = grafo.EncuentraArista (0, 1);
			ar.Existe = false;
			Assert.False (grafo.ExisteArista (1, 0));
		}

		[Test]
		public void ExisteArista ()
		{
			var grafo = new Grafo<Objeto, float> (ObjetoColl);
			grafo [0, 1] = 1;
			Assert.True (grafo.ExisteArista (0, 1) && !grafo.ExisteArista (1, 0));
		}

		[Test]
		public void Vecindad ()
		{
			var grafo = new Grafo<Objeto, float> (ObjetoColl);
			grafo [0, 1] = 1;
			grafo [0, 2] = 1;
			grafo [1, 2] = 1;

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
			var grafo = new Grafo<Objeto, float> (ObjetoColl);
			const int max = 100;
			for (int i = 0; i < size; i++)
				grafo [0, i] = 1;
			Assert.AreEqual (size, grafo.NumNodos);
			var nods = grafo.Nodos;
			for (int i = 0; i < size; i++)
				Assert.True (nods.Contains (i));
		}

		[Test]
		public void Ruta ()
		{
			// Probar ruta simétrica
			var grafo = new Grafo<Objeto, float> (ObjetoColl, true);
			const int max = 10;
			var enumerador = new Objeto[max];
			for (int i = 0; i < max; i++)
			{
				grafo [i + 1, i] = 1;
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
			grafo = new Grafo<Objeto, float> (ObjetoColl);
			for (int i = 0; i < max; i++)
			{
				grafo [i, i + 1] = 1;
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
			var grafo = new Grafo<Objeto, float> (ObjetoColl);
			grafo [0, 1] = 1;
			var clón = grafo.Clonar ();
			Assert.AreEqual (1, clón [0, 1]);
			Assert.False (clón.ExisteArista (1, 0));
		}

		[Test]
		public void AsReadonly ()
		{
			var grafo = new Grafo<Objeto, float> (ObjetoColl);
			grafo [0, 1] = 1;
			var read = grafo.ComoSóloLectura ();
			Assert.True (read.SóloLectura);
			Assert.AreEqual (1, read [0, 1]);
		}

		[Test]
		public void Subgrafo ()
		{
			var r = new Random ();
			var max = r.Next (3, 100);
			var sim = r.Next (2) == 0;
			var thiscoll = new Objeto[max];
			for (int i = 0; i < max; i++)
				thiscoll [i] = i;
			var original = new Grafo<Objeto, float> (thiscoll, sim);
			for (int i = 0; i < max; i++)
				for (int j = 0; j < max; j++)
					original [i, j] = (float)r.NextDouble ();

			int subcolSize = max / 2;
			var subcol = new Objeto[subcolSize];
			for (int i = 0; i < subcolSize; i++)
				subcol [i] = i;

			var sub = original.Subgrafo (subcol);
			for (int i = 0; i < subcolSize; i++)
				for (int j = 0; j < subcolSize; j++)
				{
					Assert.AreEqual (
						original.ExisteArista (i, j),
						sub.ExisteArista (i, j),
						string.Format (
							"i = {0}\nj = {1}",
							i,
							j));
					if (original.ExisteArista (i, j))
						Assert.AreEqual (original [i, j], sub [i, j]);
				}
		}

		[Test]
		public void Aristas ()
		{
			var gr = new Grafo<Objeto, float> (ObjetoColl);
			gr [0, 1] = 1;
			gr [0, 2] = 1;

			var aris = gr.Aristas ();
			Assert.AreEqual (2, aris.Count);
		}

		[Test]
		public void CaminoÓptimo ()
		{
			const int clusterSize = 2;
			var gr = new Grafo<Objeto, float> (ObjetoColl);
			for (int i = 0; i < clusterSize; i++)
				for (int j = 0; j < clusterSize; j++)
				{
					gr [i, j] = 1f / (i + 1);
					gr [i + clusterSize, j + clusterSize] = 1;
				}
			gr [0, clusterSize] = 1;
			gr [clusterSize, 0] = 1;

			var r = gr.CaminoÓptimo (1, clusterSize + 1, z => z.Data);
			Assert.AreEqual (1, r.NodoInicial);
			Assert.AreEqual (clusterSize + 1, r.NodoFinal);
			foreach (var x in r.Pasos)
				Assert.True (gr.ExisteArista (x.Origen, x.Destino));
			Assert.AreEqual (3, r.NumPasos);
			foreach (var x in r.Pasos)
				Console.WriteLine (string.Format ("{0} -> {1}", x.Origen, x.Destino));

			r = gr.CaminoÓptimo (0, 0, z => z.Data);
			Assert.IsNull (r);

			Assert.Throws<NodoInexistenteException> (new TestDelegate (delegate
			{
				r = gr.CaminoÓptimo (0, -1, z => z.Data);
			}));
		}
	}
}