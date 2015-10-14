using NUnit.Framework;
using System;
using Graficas.Continuo;
using Graficas;

namespace Test
{
	[TestFixture]
	public class ContinuoTest
	{
		public Grafo<int> Graf;
		Continuo<int> Gr;

		public void Iniciar ()
		{
			Graf = new Grafo<int> ();
			Gr = new Continuo<int> (Graf);
			Graf.EsSimetrico = true;
		}

		[Test]
		public void TestPunto ()
		{
			Iniciar ();
			Graf.AgregaArista (0, 1, 10);

			var pt = new Continuo<int>.ContinuoPunto (Gr, 0);
			var pt2 = new Continuo<int>.ContinuoPunto (Gr, 0);
			pt.A = 0;
			pt.B = 1;
			pt.Loc = 1;
			Assert.AreEqual (1, pt.DistanciaAExtremo (0));
			Assert.AreEqual (9, pt.DistanciaAExtremo (1));
			Assert.True (pt.EnIntervaloInmediato (0, 1));
			Assert.False (pt.EnIntervaloInmediato (0, 2));

			Assert.Throws<IndexOutOfRangeException> (delegate {
				pt.DistanciaAExtremo (2);
			});

			Assert.AreEqual (0, pt2.DistanciaAExtremo (0));
		
			var p3 = new Continuo<int>.ContinuoPunto (Gr);
			p3.A = 0;
			p3.B = 1;
			p3.Loc = 4;
			Assert.AreEqual (3, pt.Vecindad ().Count);
			Assert.AreEqual (3, Gr.Puntos.Count);
		}

		[Test]
		public void ProbarRuta ()
		{
			Iniciar ();
			for (int i = 0; i < 10; i++) {
				Graf [i, i + 1] = 1;
			}
			Continuo<int>.Ruta rta;
			rta = new Continuo<int>.Ruta (new Continuo<int>.ContinuoPunto (Gr, 0));
			//rta.NodoInicial = new Continuo<int>.ContinuoPunto(Gr, 0);
			for (int i = 1; i < 9; i++) {
				rta.Concat (i, 1);
			}
			rta.ConcatFinal (new Continuo<int>.ContinuoPunto (Gr, 10));
			rta.NodoFinal.Loc = 0.5f;

			Assert.True (rta.Contiene (new Continuo<int>.ContinuoPunto (Gr, 0)));
			Console.WriteLine (Gr.Puntos.Count);
		}

		[Test]
		public void Avances ()
		// La salida debe ser Despl Despl Nodo
		{
			Iniciar ();
			for (int i = 0; i < 10; i++) {
				Graf [i, i + 1] = 1;
			}

			var p = new Continuo<int>.ContinuoPunto (Gr, 0);

			p.AlDesplazarse += delegate {
				Console.WriteLine ("Despl");
			};
			p.AlLlegarANodo += delegate {
				Console.WriteLine ("Nodo");
			};

			Assert.False (p.AvanzarHacia (1, 0.7f));
			Assert.True (p.AvanzarHacia (1, 0.7f));
		}

		[Test]
		public void ProbarÓptimaRuta ()
		{
			Iniciar ();
			for (int i = 0; i < 10; i++) {
				Graf [i, i + 1] = 1;
			}

			var inicial = new Continuo<int>.ContinuoPunto (Gr);
			inicial.FromGrafica (0);

			inicial.AlDesplazarse += delegate {
				Console.WriteLine ("Despl a " + inicial.ToString ());
			};
			inicial.AlLlegarANodo += delegate {
				Console.WriteLine ("Nodo: " + inicial.ToString ());
			};
			inicial.AlTerminarRuta += delegate {
				Console.WriteLine ("Ruta terminada: " + inicial.ToString ());
			};

			var final = new Continuo<int>.ContinuoPunto (Gr);
			final.FromGrafica (10);

			var rutas = new ConjuntoRutasÓptimas<int> (Gr.GráficaBase);

			var r = Gr.RutaÓptima (inicial, final, rutas);

			for (int i = 0; i < 100; i++) {
				Assert.False (inicial.AvanzarHacia (r, 0.06f));
				Console.WriteLine (inicial);
			}

			Assert.True (inicial.AvanzarHacia (r, 6));


			Assert.AreEqual (final, inicial);

			Console.WriteLine (r);
		}

		/// <summary>
		/// Pruebas de desplazamiento por ruta óptima
		/// </summary>
		[Test]
		public void DesplRutas ()
		{
			Iniciar ();

			for (int i = 0; i < 100; i++) {
				Graf [i, i + 1] = 1;
			}



		}
	}
}

