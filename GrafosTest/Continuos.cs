using NUnit.Framework;
using Graficas.Grafo;
using Graficas.Continuo;
using System.Collections.Generic;
using System.Linq;
using Graficas.Rutas;
using Graficas.Aristas;

namespace Test
{
	[TestFixture]
	public class Continuos
	{
		[Test]
		public void ConjRutas ()
		{
			const int len = 4;
			var gr = new Grafo<Objeto, float> (true);

			for (int i = 0; i < len; i++)
				gr [i, i + 1] = i + 1;

			var cp = new Continuo<Objeto> (gr);
			var ro = new ConjuntoRutasÓptimas<Objeto> (gr);

			var r = Continuo<Objeto>.RutaÓptima (
				        cp.PuntoFijo (0),
				        cp.PuntoFijo (len - 1),
				        ro);

			Assert.AreEqual ((len * (len - 1)) / 2, r.Longitud);
		}

		[Test]
		public void ContPuntoBasic ()
		{
			var gr = new Grafo<Objeto, float> (true);
			gr [0, 1] = 1;
			var cp = new Continuo<Objeto> (gr);
			var p0 = cp.PuntoFijo (0);
			var p1 = p0.Clonar ();
			var p2 = new Continuo<Objeto>.ContinuoPunto (cp, 0);
			var p3 = new Continuo<Objeto>.ContinuoPunto (cp, 0, 1, 0.5f);

			Assert.AreEqual (5, cp.Puntos.Count); // 0 == p0, 1, p1
			p1.Remove ();
			Assert.AreEqual (4, cp.Puntos.Count); // 0 == p0, 1, p1
			Assert.AreEqual (p2, p0);

			Assert.True (p0.EnMismoIntervalo (p3));
			Assert.True (Continuo<Objeto>.ContinuoPunto.EnMismoIntervalo (p0, p3));
			Assert.AreEqual (0.5f, p3.DistanciaAExtremo (0));
			Assert.AreEqual (0.5f, p3.DistanciaAExtremo (1));

			Assert.True (p2.EnOrigen);
			Assert.False (p3.EnOrigen);

			Assert.AreEqual (4, p3.Vecindad ().Count);
		}

		[Test]
		public void ContPuntDinam ()
		{
			const int len = 10;
			var colisionó = new HashSet<Continuo<Objeto>.ContinuoPunto> ();
			bool seDesplazó = false;
			bool terminóRuta = false;
			var nods = new HashSet<Objeto> ();
			var gr = new Grafo<Objeto, float> (true);

			for (int i = 0; i < len; i++)
				gr [i, i + 1] = i + 1;

			var cp = new Continuo<Objeto> (gr);
			var pmov = cp.AgregaPunto (0);
			pmov.AvanzarHacia (1, 0.3f);
			Assert.True (cp.PuntosArista (0, 1).Any (z => z.Equals (pmov)));
			var pmov2 = cp.AgregaPunto (0);
			var ruta = new Continuo<Objeto>.Ruta (pmov2);
			ruta.Concat (new Paso<Objeto> (0, 1, 1));
			ruta.Concat (new Paso<Objeto> (1, 2, 2));
			ruta.Concat (new Paso<Objeto> (2, 3, 3));
			ruta.ConcatFinal (cp.PuntoFijo (3));
			Assert.AreEqual (6, ruta.Longitud);

			Assert.AreEqual (0.3f, pmov.DistanciaAExtremo (0));
			pmov2.AlColisionar += obj => colisionó.Add (obj);
			pmov2.AlDesplazarse += () => seDesplazó = true;
			pmov2.AlLlegarANodo += () => nods.Add (pmov2.A);
			pmov2.AlTerminarRuta += () => terminóRuta = true;

			var rt = pmov2.AvanzarHacia (ruta, ruta.Longitud);

			Assert.True (colisionó.Contains (pmov));
			Assert.True (seDesplazó);
			Assert.True (rt);
			Assert.True (terminóRuta);
			Assert.AreEqual (3, nods.Count);
			for (int i = 0; i < 3; i++)
				Assert.True (nods.Contains (i), i.ToString ());
		}

		[Test]
		public void Rutas ()
		{
			const int len = 4;
			var gr = new Grafo<Objeto, float> (true);

			for (int i = 0; i < len; i++)
				gr [i, i + 1] = i + 1;

			var cp = new Continuo<Objeto> (gr);

			var ruta = new Continuo<Objeto>.Ruta (cp.PuntoFijo (0));
			ruta.Concat (new Paso<Objeto> (0, 1, gr [0, 1]));
			ruta.Concat (new Paso<Objeto> (1, 2, gr [1, 2]));
			ruta.Concat (new Paso<Objeto> (2, 3, gr [2, 3]));
			ruta.ConcatFinal (cp.PuntoFijo (3));

			Assert.AreEqual (cp.PuntoFijo (0), ruta.NodoInicial);
			Assert.AreEqual (cp.PuntoFijo (3), ruta.NodoFinal);

			Assert.AreEqual (6, ruta.Longitud);
			Assert.True (ruta.Contiene (cp.PuntoFijo (0)));
			Assert.True (ruta.Contiene (cp.PuntoFijo (1)));
			Assert.True (ruta.Contiene (cp.PuntoFijo (2)));
			Assert.False (ruta.Contiene (cp.PuntoFijo (5)));
			var p = cp.AgregaPunto (0, 1, 0.1f);
			Assert.True (ruta.Contiene (p));
		}

	}
}