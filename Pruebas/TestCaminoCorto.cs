﻿using NUnit.Framework;
using Graficas;
using System;

namespace Pruebas
{
	[TestFixture]
	public class TestCaminoCorto
	{
		const int max = 100;

		[Test]
		public void TestCase ()
		{
			var gr = new Grafica<int> ();
			gr.EsSimetrico = true;
			CrearGráficaPeso (gr);


			var ruta = gr.CaminoÓptimo (0, max - 1);
			Console.WriteLine (ruta);
			Console.WriteLine ("Peso: " + ruta.Longitud);

		}

		public static void CrearGráfica (IGrafica<int> gr)
		{
			int a;
			var r = new Random ();
			for (int i = 0; i < max; i++) {

				if (i > 0) {
					a = r.Next (i - 1);
					gr.AgregaArista (i, a);
				}
			}
		}

		public static void CrearGráficaPeso (IGraficaPeso<int> gr)
		{
			int a;
			var r = new Random ();
			for (int i = 0; i < max; i++) {

				if (i > 0) {
					a = r.Next (i - 1);
					gr.SetPeso (i, a, (float)r.NextDouble ());
				}
			}

		}
	}
}
