using System;
using Graficas.Grafo;
using NUnit.Framework;
using Graficas.Aristas;
using Graficas.Rutas;

namespace Test
{
	/// <summary>
	/// Pruebas de Grafo[2]
	/// </summary>
	[TestFixture]
	public class Graf2
	{
		[Test]
		public void Clear ()
		{
			var grafo = new Grafo<Objeto, float> ();
			grafo [0, 1] = 1;
			grafo.Clear ();

			Assert.IsEmpty (grafo.Aristas ());
			Assert.IsEmpty (grafo.Nodos);
		}

		[Test]
		public void SóloLectura ()
		{
			var grafo = new Grafo<Objeto, float> ();
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
			var grafo = new Grafo<Objeto, float> ();
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
			var grafo = new Grafo<Objeto, float> (true);
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
			var grafo = new Grafo<Objeto, float> ();
			grafo [0, 1] = 1;
			Assert.True (grafo.ExisteArista (0, 1) && !grafo.ExisteArista (1, 0));
		}

		[Test]
		public void Vecindad ()
		{
			var grafo = new Grafo<Objeto, float> ();
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
			var grafo = new Grafo<Objeto, float> ();
			const int max = 100;
			for (int i = 0; i < max; i++)
				grafo [0, i] = 1;
			Assert.AreEqual (max, grafo.NumNodos);
			var nods = grafo.Nodos;
			for (int i = 0; i < max; i++)
				Assert.True (nods.Contains (i));
		}

		[Test]
		public void Ruta ()
		{
			// Probar ruta simétrica
			var grafo = new Grafo<Objeto, float> (true);
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
			grafo = new Grafo<Objeto, float> ();
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
			var grafo = new Grafo<Objeto, float> ();
			grafo [0, 1] = 1;
			var clón = grafo.Clonar ();
			Assert.AreEqual (1, clón [0, 1]);
			Assert.False (clón.ExisteArista (1, 0));
		}

		[Test]
		public void AsReadonly ()
		{
			var grafo = new Grafo<Objeto, float> ();
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
			var original = new Grafo<Objeto, float> (sim);
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
					Assert.AreEqual (original [i, j], sub [i, j]);
		}

		[Test]
		public void Aristas ()
		{
			var gr = new Grafo<Objeto, float> ();
			gr [0, 1] = 1;
			gr [0, 2] = 1;

			var aris = gr.Aristas ();
			Assert.AreEqual (2, aris.Count);
		}
	}
}