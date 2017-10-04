using System;
using System.Collections.Generic;
using System.Linq;
using CE.Graph.Edges;
using CE.Graph.Grafo;
using CE.Graph.Grafo.Estáticos;
using CE.Graph.Rutas;

namespace CE.Graph.Continua
{

	/// <summary>
	/// Representa un continuo producido por una IGrafica
	/// </summary>
	[Serializable]
	public class ContinuumGraph<T>
	{
		/// <summary>
		/// Graph producing this continnum.
		/// </summary>
		public Graph<T, float> GrafoBase { get; }

		/// <summary>
		/// Observable points.
		/// </summary>
		public List<ContinuumPoint<T>> Points { get; } = new List<ContinuumPoint<T>> ();

		/// <summary>
		/// Gets the comparer for nodes.
		/// </summary>
		public IEqualityComparer<T> NodeComparer => GrafoBase.NodeComparer;

		/// <summary>
		/// Gets the comparer for points.
		/// </summary>
		public IEqualityComparer<ContinuumPoint<T>> PointComparer { get; }

		/// <summary>
		/// Add a new observable point and returns it.
		/// </summary>
		/// <param name="a">An endpoint.</param>
		/// <param name="b">Second endpoint.</param>
		/// <param name="loc">Distance to the frist endpoint.</param>
		public ContinuumPoint<T> AddPoint (T a, T b, float loc)
		{
			if (ReferenceEquals (a, null))
				throw new ArgumentNullException (nameof (a));
			var ret = new ContinuumPoint<T> (this, a, b, loc) { A = a, B = b, Loc = loc };
			return ret;
		}

		/// <summary>
		/// Add a new observable point and returns it.
		/// </summary>
		/// <param name="a">Graph node</param>
		public ContinuumPoint<T> AddPoint (T a)
		{
			return new ContinuumPoint<T> (this, a);
		}

		/// <summary>
		/// Gets the observable points in an edge.
		/// </summary>
		/// <param name="edge0">First endpoint.</param>
		/// <param name="edge1">Second endpoint.</param>
		[Obsolete]
		public IEnumerable<ContinuumPoint<T>> PuntosArista (T edge0, T edge1)
		{
			var aris = new Tuple<T, T> (edge0, edge1);
			return PuntosArista (aris);
		}

		/// <summary>
		/// Gets the observable points in an edge.
		/// </summary>
		/// <param name="edge">Arista.</param>
		[Obsolete]
		public IEnumerable<ContinuumPoint<T>> PuntosArista (IEdge<T> edge)
		{
			return PuntosArista (edge.AsTuple ());
		}

		/// <summary>
		/// Gets a collection of points contained in a specified edge.
		/// </summary>
		public ICollection<ContinuumPoint<T>> PointsInEdge (T p1, T p2)
		{
			return Points.FindAll (x => x.OnEdge (p1, p2));
		}

		/// <summary>
		/// Gets a collection of points contained in a specified edge.
		/// </summary>
		public ICollection<ContinuumPoint<T>> PointsInEdge (IEdge<T> edge)
		{
			var tuple = edge.AsTuple ();
			return PointsInEdge (tuple.Item1, tuple.Item2);
		}

		[Obsolete]
		IEnumerable<ContinuumPoint<T>> PuntosArista (Tuple<T, T> arista)
		{
			return Points.Where (x => x.EndPoints.Equals (arista));
		}

		/// <summary>
		/// Gets the point associated to a graph node.
		/// </summary>
		public ContinuumPoint<T> NodeToPoint (T node)
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
		public ContinuumGraph (Graph<T, float> gráfica)
		{
			GrafoBase = gráfica.IsReadOnly ? gráfica : gráfica.AsReadonly ();
			PointComparer = new MatchComparer<T> (NodeComparer);
			_fixedPoints = new Dictionary<T, ContinuumPoint<T>> (NodeComparer);
			foreach (var x in gráfica.Nodes)
				_fixedPoints.Add (x, AddPoint (x));
		}

		readonly Dictionary<T, ContinuumPoint<T>> _fixedPoints;
	}
}