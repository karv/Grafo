using System;
using Graficas.Continuo;
using Graficas.Grafo;
using Graficas.Rutas;
using Xunit;

namespace Test
{
	public class TestClassIns : IEquatable<TestClassIns>
	{
		public readonly int? Val;

		public bool Equals (TestClassIns other)
		{
			if (other == null)
				return false;
			return Val.HasValue && other.Val.HasValue && Val.Value == other.Val.Value;
		}

		public TestClassIns (int val)
		{
			Val = val;
		}

		public TestClassIns ()
		{
		}

		public static implicit operator TestClassIns (int i)
		{
			return new TestClassIns (i);
		}

		public override string ToString ()
		{
			return Val.ToString ();
		}

		public override int GetHashCode ()
		{
			return Val ?? 0;
		}
	}

	public class ContinuoTest
	{
		public Grafo<TestClassIns, float> Graf;
		Continuo<TestClassIns> Gr;

		public void Iniciar ()
		{
			Graf = new Grafo<TestClassIns, float> (true, false);
			Gr = new Continuo<TestClassIns> (Graf);
		}

		[Fact]
		public void TestMismoIntervalo ()
		{
			Iniciar ();
			Graf [0, 1] = 1;
			Assert.Equal (1, Graf [0, 1]);
			Graf [1, 2] = 1;
			var p1 = Gr.AgregaPunto (0);
			var p2 = Gr.AgregaPunto (1);
			var p3 = Gr.AgregaPunto (2);
			var p4 = Gr.AgregaPunto (0, 1, 0.5f);

			Assert.True (p1.EnMismoIntervalo (p2));
			Assert.True (p2.EnMismoIntervalo (p1));

			Assert.False (p1.EnMismoIntervalo (p3));
			Assert.False (p3.EnMismoIntervalo (p1));

			Assert.True (p3.EnMismoIntervalo (p2));
			Assert.True (p2.EnMismoIntervalo (p3));

			Assert.True (p4.EnMismoIntervalo (p1));
			Assert.True (p4.EnMismoIntervalo (p2));
			Assert.False (p4.EnMismoIntervalo (p3));
			Assert.True (p4.EnMismoIntervalo (p4));

		}

		[Fact]
		public void TestPunto ()
		{
			Iniciar ();
			Graf [0, 1] = 10;

			var pt = new Continuo<TestClassIns>.ContinuoPunto (Gr, 0);
			var pt2 = new Continuo<TestClassIns>.ContinuoPunto (Gr, 0);
			pt.A = 0;
			pt.B = 1;
			pt.Loc = 1;
			Assert.Equal (1, pt.DistanciaAExtremo (0));
			Assert.Equal (9, pt.DistanciaAExtremo (1));
			Assert.True (pt.EnIntervaloInmediato (0, 1));
			Assert.False (pt.EnIntervaloInmediato (0, 2));

			Assert.Throws<IndexOutOfRangeException> (delegate
			{
				pt.DistanciaAExtremo (2);
			});

			Assert.Equal (0, pt2.DistanciaAExtremo (0));
		
			Assert.Equal (2, pt.Vecindad ().Count);
			Assert.Equal (2, Gr.Puntos.Count); // No es 3, porque no debe calcular que se agregó el nodo '1'

			// Los puntos fijos
			var pf1 = Gr.PuntoFijo (1);
			Assert.True (ReferenceEquals (pf1, Gr.PuntoFijo (1)));
			Assert.Equal (2, Gr.Puntos.Count); // No es 3, porque no debe calcular que se agregó el nodo '1'
		}

		public void ProbarRuta ()
		{
			Iniciar ();
			for (int i = 0; i < 10; i++)
			{
				Graf [i, i + 1] = 1;
			}
			Continuo<TestClassIns>.Ruta rta;
			rta = new Continuo<TestClassIns>.Ruta (new Continuo<TestClassIns>.ContinuoPunto (
				Gr,
				0));
			for (int i = 1; i < 9; i++)
				rta.Concat (Graf.EncuentraArista (i - 1, i), rta.NodoFinal.A);
			rta.ConcatFinal (new Continuo<TestClassIns>.ContinuoPunto (Gr, 10));
			rta.NodoFinal.Loc = 0.5f;

			Assert.True (rta.Contiene (new Continuo<TestClassIns>.ContinuoPunto (Gr, 0)));
			Console.WriteLine (Gr.Puntos.Count);
		}

		[Fact]
		public void Avances ()
		// La salida debe ser Despl Despl Nodo
		{
			string outp = "";
			Iniciar ();
			for (int i = 0; i < 10; i++)
			{
				Graf [i, i + 1] = 1;
				Assert.Equal (1, Graf [i, i + 1]);
				Assert.Equal (1, Graf [i + 1, i]);
			}

			var p = new Continuo<TestClassIns>.ContinuoPunto (Gr, 0);

			p.AlDesplazarse += delegate
			{
				Console.WriteLine ("Despl");
				outp += "d";
			};
			p.AlLlegarANodo += delegate
			{
				Console.WriteLine ("Nodo");
				outp += "n";
			};

			var rr = Graf [0, 1];
			Assert.False (p.AvanzarHacia (1, 0.7f));
			Assert.True (p.AvanzarHacia (1, 0.7f));
			Assert.Equal ("ddn", outp);
		}

		[Fact]
		public void ProbarÓptimaRuta ()
		{
			Iniciar ();
			for (int i = 0; i < 10; i++)
				Graf [i, i + 1] = 1;

			Assert.Equal (11, Graf.NumNodos);
			var inicial = new Continuo<TestClassIns>.ContinuoPunto (Gr, 0);

			inicial.AlDesplazarse += delegate
			{
				//Console.WriteLine("Despl a " + inicial);
			};
			inicial.AlLlegarANodo += delegate
			{
				Console.WriteLine ("Nodo: " + inicial);
			};
			inicial.AlTerminarRuta += delegate
			{
				Console.WriteLine ("Ruta terminada: " + inicial);
			};

			var final = new Continuo<TestClassIns>.ContinuoPunto (Gr, 10);

			var rutas = new ConjuntoRutasÓptimas<TestClassIns> (Gr.GráficaBase);

			var r = Continuo<TestClassIns>.RutaÓptima (inicial, final, rutas);

			Assert.False (inicial.AvanzarHacia (r, 0));
			Console.WriteLine (inicial);

			for (int i = 0; i < 100; i++)
			{
				Assert.False (inicial.AvanzarHacia (r, 0.06f));
				Console.WriteLine (inicial);
			}

			Assert.True (inicial.AvanzarHacia (r, 6));


			Assert.Equal (final, inicial);

			Console.WriteLine (r);
		}

		[Fact]
		public void ProbarEventoColisión ()
		{
			Iniciar ();
			int i;
			for (i = 0; i < 10; i++)
			{
				Graf [0, i + 1] = 1;
			}

			var p1 = Gr.AgregaPunto (0);
			var p2 = Gr.AgregaPunto (1);

			p1.AlColisionar += obj => Console.WriteLine (string.Format (
				"Colisión: {0} con {1}; tiempo {2}",
				p1,
				obj,
				i));

			for (i = 0; i < 100; i++)
			{
				p1.AvanzarHacia (1, 0.01f);
				p2.AvanzarHacia (0, 0.001f);
			}
			Assert.Equal (2, Gr.Puntos.Count);
		}

		[Fact]
		public void RecrearError ()
		{
			Iniciar ();
			for (int i = 0; i < 10; i++)
				Graf [i, i + 1] = 1;

			var p0 = Gr.AgregaPunto (0, 1, 0);
			var p1 = Gr.AgregaPunto (1, 2, 0);
			var cc = new ConjuntoRutasÓptimas<TestClassIns> (Graf);
			var rr = Continuo<TestClassIns>.RutaÓptima (p0, p1, cc);
			var lon = rr.Longitud;
			Assert.False (float.IsInfinity (lon));
		}
	}
}