using System;
using System.Collections.Generic;
using System.Linq;
using Graficas.Edges;
using Graficas.Grafo;
using Graficas.Grafo.Estáticos;
using Graficas.Rutas;

namespace Graficas.Continua
{

	/// <summary>
	/// Representa un continuo producido por una IGrafica
	/// </summary>
	[Serializable]
	public class GraphContinuum<T>
	{
		/// <summary>
		/// Graph producing this continnum.
		/// </summary>
		public Grafo<T, float> GrafoBase { get; }

		/// <summary>
		/// Observable points.
		/// </summary>
		public List<Punto<T>> Points { get; } = new List<Punto<T>> ();

		/// <summary>
		/// Gets the comparer for nodes.
		/// </summary>
		public IEqualityComparer<T> NodeComparer => GrafoBase.Comparador;

		/// <summary>
		/// Gets the comparer for points.
		/// </summary>
		public IEqualityComparer<Punto<T>> PointComparer { get; }

		/// <summary>
		/// Add a new observable point and returns it.
		/// </summary>
		/// <param name="a">An endpoint.</param>
		/// <param name="b">Second endpoint.</param>
		/// <param name="loc">Distance to the frist endpoint.</param>
		public Punto<T> AddPoint (T a, T b, float loc)
		{
			if (ReferenceEquals (a, null))
				throw new ArgumentNullException (nameof (a));
			var ret = new Punto<T> (this, a, b, loc) { A = a, B = b, Loc = loc };
			return ret;
		}

		/// <summary>
		/// Add a new observable point and returns it.
		/// </summary>
		/// <param name="a">Graph node</param>
		public Punto<T> AddPoint (T a)
		{
			return new Punto<T> (this, a);
		}

		/// <summary>
		/// Gets the observable points in an edge.
		/// </summary>
		/// <param name="edge0">First endpoint.</param>
		/// <param name="edge1">Second endpoint.</param>
		[Obsolete]
		public IEnumerable<Punto<T>> PuntosArista (T edge0, T edge1)
		{
			var aris = new Tuple<T, T> (edge0, edge1);
			return PuntosArista (aris);
		}

		/// <summary>
		/// Gets the observable points in an edge.
		/// </summary>
		/// <param name="edge">Arista.</param>
		[Obsolete]
		public IEnumerable<Punto<T>> PuntosArista (IEdge<T> edge)
		{
			return PuntosArista (edge.AsTuple ());
		}

		/// <summary>
		/// Gets a collection of points contained in a specified edge.
		/// </summary>
		public ICollection<Punto<T>> PointsInEdge (T p1, T p2)
		{
			return Points.FindAll (x => x.EnIntervaloInmediato (p1, p2));
		}

		/// <summary>
		/// Gets a collection of points contained in a specified edge.
		/// </summary>
		public ICollection<Punto<T>> PointsInEdge (IEdge<T> edge)
		{
			var tuple = edge.AsTuple ();
			return PointsInEdge (tuple.Item1, tuple.Item2);
		}

		[Obsolete]
		IEnumerable<Punto<T>> PuntosArista (Tuple<T, T> arista)
		{
			return Points.Where (x => x.Extremos.Equals (arista));
		}

		/// <summary>
		/// Gets the point associated to a graph node.
		/// </summary>
		public Punto<T> NodeToPoint (T node)
		{
			try
			{
				return _fixedPoints[node];
			}
			catch (KeyNotFoundException ex)
			{
				throw new NonExistentNodeException (
					string.Format (
						"Cannot find node {0} in {1}.",
						node,
						GrafoBase),
					ex);
			}
		}

		/// <param name="gráfica">Base graph</param>
		public GraphContinuum (Grafo<T, float> gráfica)
		{
			GrafoBase = gráfica.SóloLectura ? gráfica : gráfica.ComoSóloLectura ();
			PointComparer = new MatchComparer<T> (NodeComparer);
			_fixedPoints = new Dictionary<T, Punto<T>> (NodeComparer);
			foreach (var x in gráfica.Nodos)
				_fixedPoints.Add (x, AddPoint (x));
		}

		/// <summary>
		/// Gets the shortest path between two observable points.
		/// </summary>
		/// <param name="origin">Punto inicial.</param>
		/// <param name="destination">Punto final.</param>
		/// <param name="routes">Optimal routes collection</param>
		public static Ruta<T> OptimalPath (Punto<T> origin,
																			 Punto<T> destination,
																			 ConjuntoRutasÓptimas<T> routes)
		{
			// TODO: several issues
			var ruta = routes.CaminoÓptimo (origin.A, destination.A);
			var ret = new Ruta<T> (origin);
			ret.Concat (ruta);
			ret.ConcatFinal (destination);
			return ret;
		}

		readonly Dictionary<T, Punto<T>> _fixedPoints;
	}
}