using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Graficas.Edges;
using Graficas.Rutas;

namespace Graficas.Grafo.Dinámicos
{
	/// <summary>
	/// A graph implemented as a collection of neighborhoods.
	/// </summary>
	public class NeighborGraph<TNode> : IGraph<TNode>
	{
		/// <summary>
		/// Gets a value indicating whether the specified edge exists.
		/// </summary>
		public bool this[TNode origin, TNode destination]
		{
			get
			{
				bool ret = Neighbor[origin].Contains (destination);
				return ret;
			}
			set
			{
				SetEdge (origin, destination, value);
				if (IsSymmetric)
					SetEdge (destination, origin, value);
			}
		}

		/// <summary>
		/// Gets a readonly collection of the nodes.
		/// </summary>
		public ReadOnlyCollection<TNode> Nodes => new List<TNode> (Neighbor.Keys).AsReadOnly ();

		/// <summary>
		/// Gets the node count.
		/// </summary>
		public int NodeCount => Neighbor.Keys.Count;

		/// <summary>
		/// Gets the node comparer.
		/// </summary>
		public IEqualityComparer<TNode> NodeComparer { get; }

		/// <summary>
		/// Determines whether each edge is symmetrical
		/// </summary>
		/// <value><c>true</c> if simétrico; otherwise, <c>false</c>.</value>
		public bool IsSymmetric { get; }

		/// <summary>
		/// Assignment from nodes to neghborhoods
		/// </summary>
		protected Dictionary<TNode, HashSet<TNode>> Neighbor { get; }

		IEdge<TNode> IGraph<TNode>.this[TNode desde, TNode hasta] => Arista (desde, hasta);

		IEnumerable<TNode> IGraph<TNode>.Nodes => Nodes;

		/// <summary>
		/// Initializes a new instance of the <see cref="Dinámicos.NeighborGraph{TNode}"/> class.
		/// </summary>
		/// <param name="symmetrical">If set to <c>true</c> simétrico.</param>
		/// <param name="comparador">Node comparer</param>
		public NeighborGraph (bool symmetrical = false,
													IEqualityComparer<TNode> comparador = null)
		{
			NodeComparer = comparador ?? EqualityComparer<TNode>.Default;
			IsSymmetric = symmetrical;
			Neighbor = new Dictionary<TNode, HashSet<TNode>> (NodeComparer);
		}

		/// <summary>
		/// Removes all the edges.
		/// </summary>
		public void ClearEdges ()
		{
			foreach (var x in Neighbor.Values)
				x.Clear ();
		}

		/// <summary>
		/// Remove edges and nodes.
		/// </summary>
		public void Clear ()
		{
			Neighbor.Clear ();
		}

		/// <summary>
		/// Gets the collection of edges.
		/// </summary>
		/// <remarks>Reference is nor preserved.</remarks>
		public IEnumerable<IEdge<TNode>> Edges ()
		{
			foreach (var x in Neighbor)
				foreach (var y in x.Value)
					yield return new ExistentialEdge<TNode> (x.Key, y, true, true, IsSymmetric);
		}

		/// <summary>
		/// Gets the edge count.
		/// </summary>
		public int EdgeCount ()
		{
			var ret = 0;
			foreach (var z in Neighbor)
				ret += z.Value.Count;
			return ret;
		}

		/// <summary>
		/// Gets a collection determining whether a specified node is in this graph.
		/// </summary>
		public bool ExistNode (TNode node) => Neighbor.ContainsKey (node);

		/// <summary>
		/// Add a node.
		/// </summary>
		public void AddNode (TNode node)
		{
			if (ExistNode (node))
				throw new InvalidOperationException ("Cannot add an existing node");

			Neighbor.Add (node, new HashSet<TNode> (NodeComparer));
		}

		/// <summary>
		/// Remove a node, and all edges about this node.
		/// </summary>
		/// <param name="node">Nodo</param>
		public void RemoveNode (TNode node)
		{
			Neighbor.Remove (node);
			foreach (var nodo in Neighbor.Values)
				nodo.Remove (node);
			// Remove backwise edges.
		}

		/// <summary>
		/// Gets a copy of the collection of neighbors.
		/// </summary>
		/// <param name="node">Graph node</param>
		public ICollection<TNode> Neighborhood (TNode node)
		{
			return new HashSet<TNode> (
				ReferencePreservingNeighborhood (node),
				NodeComparer);
		}

		/// <summary>
		/// Gets a clone of the specified edge.
		/// </summary>
		public ExistentialEdge<TNode> Arista (TNode origin, TNode destination)
		{
			bool ret = Neighbor[origin].Contains (destination);
			return new ExistentialEdge<TNode> (origin, destination, ret, true, IsSymmetric);
		}

		/// <summary>
		/// Convers a sequence of nodes into a <see cref="IPath{TNode}"/>
		/// </summary>
		public IPath<TNode> ToPath (IEnumerable<TNode> seq)
		{
			var ret = new Path<TNode> ();
			bool iniciando = true; // Flag que indica que está construyendo el primer nodo (no paso)
			TNode last = default (TNode);
			foreach (var x in seq)
			{
				if (iniciando)
				{
					iniciando = false;
					ret = new Path<TNode> ();
				}
				else
				{
					if (!Neighborhood (last).Contains (x))
						throw new InvalidPathOperationException ("La sucesión dada no representa una ruta.");
					ret.Concat (new Step<TNode> (last, x));
				}
				last = x;
			}
			return ret;
		}

		/// <summary>
		/// Computes and returns the subgraph relative to the specified node set.
		/// </summary>
		/// <param name="set">Collection of nodes.</param>
		public NeighborGraph<TNode> Subgraph (IEnumerable<TNode> @set)
		{
			if (@set == null)
				throw new ArgumentNullException (nameof (@set));
			try
			{
				var ret = new NeighborGraph<TNode> (IsSymmetric, NodeComparer);
				foreach (var c in @set)
				{
					ret.AddNode (c);
					foreach (var n in ReferencePreservingNeighborhood (c).Where (z => @set.Contains (z)))
						ret.ReferencePreservingNeighborhood (c).Add (n);
				}
				return ret;
			}
			catch (Exception ex)
			{
				var m = string.Format ("Cannot get subgraph. {0}.\n" +
								"Is the specified set a subset of the nodes of this graph?", @set);
				throw new ArgumentException (m, nameof (@set), ex);
			}
		}

		/// <summary>
		/// Gets the neighborhood of a specified node.
		/// </summary>
		protected HashSet<TNode> ReferencePreservingNeighborhood (TNode node)
		{
			try
			{
				return Neighbor[node];
			}
			catch (KeyNotFoundException ex)
			{
				var m = string.Format ("Cannot get node {0}.", node);
				throw new NonExistentNodeException (m, ex);
			}
		}

		/// <summary>
		/// Removes an edge.
		/// </summary>
		protected void RemoveEdge (TNode origin, TNode destination)
		{
			var vec = Neighbor[origin];
			vec.Remove (destination);
		}

		/// <summary>
		/// Adds an edge.
		/// </summary>
		protected void AddEdge (TNode origin, TNode destination)
		{
			var vec = ReferencePreservingNeighborhood (origin);
			vec.Add (destination);
		}

		IGraph<TNode> IGraph<TNode>.Subgraph (IEnumerable<TNode> conjunto)
		{
			return Subgraph (conjunto);
		}

		void SetEdge (TNode desde, TNode hasta, bool valor)
		{
			if (valor)
				AddEdge (desde, hasta);
			else
				RemoveEdge (desde, hasta);
		}
	}
}