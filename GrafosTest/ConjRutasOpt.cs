﻿using System;
using NUnit.Framework;
using Graficas.Grafo;
using Graficas.Rutas;
using System.Collections.Generic;

namespace Test
{
	[TestFixture]
	public class ConjRutasOpt
	{
		const int size = 30;
		ICollection<int> ObjetoColl;

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
			var r = new Random ();
			const double compacidad = 0.1;
			var g = new Grafo<int> (ObjetoColl, true);
			for (int i = 1; i < numNods; i++)
			{
				var conexNodo = r.Next (i - 1);
				g [i, conexNodo] = true;
				for (int j = 0; j < i; j++)
					g [i, j] |= r.NextDouble () < compacidad;
			}
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