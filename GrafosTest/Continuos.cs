using System.Collections.Generic;
using System.Linq;
using CE.Graph.Continua;
using CE.Graph.Edges;
using CE.Graph.Grafo.Estáticos;
using NUnit.Framework;

namespace Test
{
	[TestFixture]
	public class Continuos
	{
		const int size = 30;
		ICollection<Objeto> ObjetoColl;

		[SetUp]
		public void Setup ()
		{
			ObjetoColl = new HashSet<Objeto> ();
			for (int i = 0; i < size; i++)
				ObjetoColl.Add (i);
		}

		[Test]
		public void ContPuntoBasic ()
		{
			var gr = new Graph<Objeto, float> (ObjetoColl, true);
			var compa = new MatchComparer<Objeto> ();
			gr[0, 1] = 1;
			var cp = new ContinuumGraph<Objeto> (gr);
			var p0 = cp.NodeToPoint (0);
			var p1 = p0.Clone ();
			var p2 = new ContinuumPoint<Objeto> (cp, 0);
			var p3 = new ContinuumPoint<Objeto> (cp, 0, 1, 0.5f);

			Assert.AreEqual (3 + ObjetoColl.Count, cp.Points.Count); // 0 == p0, 1, p1
			p1.Remove ();
			Assert.AreEqual (2 + ObjetoColl.Count, cp.Points.Count); // 0 == p0, 1, p1
			Assert.True (compa.Equals (p2, p0));
			//Assert.AreEqual (p2, p0);

			Assert.True (p0.OnSameInterval (p3));
			Assert.True (ContinuumPoint<Objeto>.OnSameInterval (p0, p3));
			Assert.AreEqual (0.5f, p3.DistanciaAExtremo (0));
			Assert.AreEqual (0.5f, p3.DistanciaAExtremo (1));

			Assert.True (p2.AtNode);
			Assert.False (p3.AtNode);

			Assert.AreEqual (4, p3.Neighborhood ().Count);
		}

		[Test]
		public void ContPuntDinam ()
		{
			const int len = 10;
			var colisionó = new HashSet<ContinuumPoint<Objeto>> ();
			bool seDesplazó = false;
			bool terminóRuta = false;
			var nods = new HashSet<Objeto> ();
			var gr = new Graph<Objeto, float> (ObjetoColl, true);

			for (int i = 0; i < len; i++)
				gr[i, i + 1] = i + 1;

			var cp = new ContinuumGraph<Objeto> (gr);
			var pmov = cp.AddPoint (0);
			pmov.AdvanceTowards (1, 0.3f);
			Assert.True (cp.PointsInEdge (0, 1).Any (z => z.Match (pmov)));
			var pmov2 = cp.AddPoint (0);
			var ruta = new CE.Graph.Continua.Path<Objeto> (pmov2);
			ruta.Concat (new Step<Objeto> (0, 1, 1));
			ruta.Concat (new Step<Objeto> (1, 2, 2));
			ruta.Concat (new Step<Objeto> (2, 3, 3));
			ruta.ConcatFinal (cp.NodeToPoint (3));
			Assert.AreEqual (6, ruta.Length);

			Assert.AreEqual (0.3f, pmov.DistanciaAExtremo (0));
			pmov2.AlColisionar += obj => colisionó.Add (obj);
			pmov2.AlDesplazarse += () => seDesplazó = true;
			pmov2.AlLlegarANodo += () => nods.Add (pmov2.A);

			var rt = false;

			const int partes = 100;
			var avanceParte = ruta.Length / partes;
			while (!rt)
				rt = pmov2.AdvanceTowards (ruta, avanceParte);

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
			var gr = new Graph<Objeto, float> (ObjetoColl, true);

			for (int i = 0; i < len; i++)
				gr[i, i + 1] = i + 1;

			var cp = new ContinuumGraph<Objeto> (gr);

			var ruta = new CE.Graph.Continua.Path<Objeto> (cp.NodeToPoint (0));
			ruta.Concat (new Step<Objeto> (0, 1, gr[0, 1]));
			ruta.Concat (new Step<Objeto> (1, 2, gr[1, 2]));
			ruta.Concat (new Step<Objeto> (2, 3, gr[2, 3]));
			ruta.ConcatFinal (cp.NodeToPoint (3));

			Assert.AreEqual (cp.NodeToPoint (0), ruta.StartNode);
			Assert.AreEqual (cp.NodeToPoint (3), ruta.EndNode);

			Assert.AreEqual (6, ruta.Length);
			Assert.True (ruta.Contains (cp.NodeToPoint (0)));
			Assert.True (ruta.Contains (cp.NodeToPoint (1)));
			Assert.True (ruta.Contains (cp.NodeToPoint (2)));
			Assert.False (ruta.Contains (cp.NodeToPoint (5)));
			var p = cp.AddPoint (0, 1, 0.1f);
			Assert.True (ruta.Contains (p));
		}
	}
}