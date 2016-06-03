using System;
using NUnit.Framework;
using Graficas.Grafo;
using Graficas.Rutas;
using System.Collections.Generic;

namespace Test
{
	[TestFixture]
	public class ConjRutasOpt
	{
		public static Grafo<T, float> HacerConexo<T> (IList<T> nodos,
		                                              float compacidad = 0.1f)
			where T : IEquatable<T>
		{
			var r = new Random ();
			var g = new Grafo<T, float> (true);
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

		[Test]
		public void General ()
		{
			const int numNods = 3;
			var nods = new int[numNods];
			for (int i = 0; i < numNods; i++)
				nods [i] = i;
			var g = HacerConexo (nods);
			var rutas = new ConjuntoRutasÓptimas<int> (g);
			for (int i = 0; i < numNods; i++)
			{
				for (int j = 0; j < numNods; j++)
				{
					if (i != j)
					{
						var rt = rutas.CaminoÓptimo (i, j);
						Assert.NotNull (rt);
						Assert.AreEqual (i, rt.NodoInicial);
						Assert.AreEqual (j, rt.NodoFinal);
					}
				}
			}
		}
	}
}