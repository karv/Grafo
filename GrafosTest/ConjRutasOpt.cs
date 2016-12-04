using System;
using NUnit.Framework;
using Graficas.Grafo.Estáticos;
using Graficas.Rutas;
using System.Collections.Generic;

namespace Test
{
	[TestFixture]
	public class ConjRutasOpt
	{
		const int size = 30;
		ICollection<int> ObjetoColl;

		public static Grafo<T, float> HacerConexo<T> (IList<T> nodos,
		                                              float compacidad = 0.1f)
		{
			var r = new Random ();
			var g = new Grafo<T, float> (nodos, true);
			var numNods = nodos.Count;
			for (int i = 1; i < numNods; i++)
			{
				var conexNodo = r.Next (i - 1);
				g [nodos [i], nodos [conexNodo]] = (float)r.NextDouble ();
				for (int j = 0; j < i; j++)
				{
					var ar = g.EncuentraArista (nodos [i], nodos [j]);
					if (!ar.Existe && r.NextDouble () < compacidad)
					{
						ar.Existe = true;
						ar.Data = (float)r.NextDouble ();
					}
				}
			}

			foreach (var x in nodos)
				Assert.True (g.Vecino (x).Count > 0, x.ToString ());
			return g;
		}

		[TestFixtureSetUp]
		public void Setup ()
		{
			ObjetoColl = new HashSet<int> ();
			for (int i = 0; i < size; i++)
				ObjetoColl.Add (i);
		}

		[Test]
		public void General ()
		{
			const int numNods = 3;
			var nods = new int[numNods];
			for (int i = 0; i < numNods; i++)
				nods [i] = i;
			var g = HacerConexo (nods);
			var rutas = new ConjuntoRutasÓptimas<int> ();
			rutas.Calcular (g);
			for (int i = 0; i < numNods; i++)
			{
				for (int j = 0; j < numNods; j++)
				{
					if (i != j)
					{
						var rt = rutas.CaminoÓptimo (i, j);
						Assert.NotNull (rt, string.Format ("¿No hay camino de {0} a {1}?", i, j));
						Assert.AreEqual (i, rt.NodoInicial);
						Assert.AreEqual (j, rt.NodoFinal);
					}
				}
			}
		}
	}
}