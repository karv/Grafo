using System;
using System.Collections.Generic;
using Graficas.Edges;
using Graficas.Rutas;
using System.Linq;

namespace Graficas.Grafo.Estáticos
{
	/// <summary>
	/// Common implementation for static graphs.
	/// </summary>
	[Serializable]
	public abstract class CommonGraph<T> : IGraph<T>
	{
		/// <summary>
		/// Gets the node comparer.
		/// </summary>
		public IEqualityComparer<T> NodeComparer { get; } = EqualityComparer<T>.Default;

		/// <summary>
		/// Gets a readcoly collection of the nodes.
		/// </summary>
		public IReadOnlyList<T> Nodes => new List<T> (NodeArray).AsReadOnly ();

		/// <summary>
		/// Gets a value indicating whether this graph is readonly.
		/// </summary>
		public bool IsReadOnly { get; }

		/// <summary>
		/// Gets a value indicating whether this graph preserves symmetry under modification.
		/// </summary>
		public bool IsSymmetric { get; }

		/// <summary>
		/// Gets the number of nodes.
		/// </summary>
		public int NodeCount => Nodes.Count;

		/// <summary>
		/// Gets the edges collection as a bidimentional array.
		/// </summary>
		protected ExistentialEdge<T>[,] Data { get; set; }

		/// <summary>
		/// Gets the nodes.
		/// </summary>
		protected T[] NodeArray { get; }

		IEnumerable<T> IGraph<T>.Nodes => throw new NotImplementedException ();

		IEdge<T> IGraph<T>.this[T desde, T hasta] => throw new NotImplementedException ();

		/// <param name="symmetric">The graph is symmetric.</param>
		/// <param name="isReadOnly">Is read only.</param>
		/// <param name="nodes">Collection of nodes.</param>
		protected CommonGraph (IEnumerable<T> nodes,
													 bool symmetric = false,
													 bool isReadOnly = false)
		{
			IsSymmetric = symmetric;
			IsReadOnly = isReadOnly;
			NodeArray = nodes.ToArray ();

			Data = new ExistentialEdge<T>[NodeArray.Length, NodeArray.Length];
			InitData ();
		}

		/// <summary>
		/// Clears the graph. This method won't dereference old edges.
		/// </summary>
		public void Clear ()
		{
			if (IsReadOnly)
				throw new InvalidOperationException ("Grafo es sólo lectura.");
			ClearData ();
			Cleared?.Invoke (this, EventArgs.Empty);
		}

		/// <summary>
		/// Determines whether exists an edge.
		/// </summary>
		public bool EdgeExists (T origin, T destination)
		{
			return GetEdge (origin, destination).Exists;
		}

		/// <summary>
		/// Computes the edge count.
		/// </summary>
		public int EdgeCount ()
		{
			var ret = 0;
			foreach (var d in Data)
				if (d.Exists) ret++;

			return ret;
		}

		/// <summary>
		/// Gets the edge.
		/// </summary>
		public ExistentialEdge<T> GetEdgeSym (T origen, T destino)
		{
			int index0;
			int index1;

			int indexOri = IndexOf (origen);
			int indexDes = IndexOf (destino);

			if (IsSymmetric)
			{
				index1 = Math.Min (indexOri, indexDes);
				index0 = Math.Max (indexOri, indexDes);
			}
			else
			{
				index0 = indexOri;
				index1 = indexDes;
			}

			if (index0 == -1 || index1 == -1)
				throw new NonExistentNodeException ();
			return Data[index0, index1];
		}

		/// <summary>
		/// Gets a set containing the outside neighborhood of a node.
		/// </summary>
		public ISet<T> OutwardNeighborhood (T node)
		{
			ISet<T> ret = new HashSet<T> ();
			var ix = IndexOf (node);
			for (int i = 0; i < NodeCount; i++)
			{
				var ar = GetEdge (ix, i);
				if (ar.Exists)
					ret.Add (NodeArray[i]);
			}

			return ret;
		}

		/// <summary>
		/// Gets a set containing the inside neighborhood of a node.
		/// </summary>
		public ISet<T> InwardNeighborhood (T node)
		{
			ISet<T> ret = new HashSet<T> ();
			var ix = IndexOf (node);
			for (int i = 0; i < NodeCount; i++)
			{
				var ar = GetEdge (i, ix);
				if (ar.Exists)
					ret.Add (NodeArray[i]);
			}

			return ret;
		}
		/// <summary>
		/// Converts the specified sequence into a <see cref="IPath{T}"/>
		/// </summary>
		public IPath<T> ToPath (IEnumerable<T> seq)
		{
			var ret = new Ruta<T> ();
			bool iniciando = true; // Flag que indica que está construyendo el primer nodo (no paso)
			T last = default (T);
			foreach (var x in seq)
			{
				if (iniciando)
				{
					iniciando = false;
					ret = new Ruta<T> ();
				}
				else
				{
					var ar = GetEdge (last, x);
					if (!ar.Exists)
						throw new RutaInconsistenteException ();
					ret.Concat (ar, last);

				}
				last = x;
			}
			return ret;
		}

		/// <summary>
		/// Gets the collection of edges.
		/// </summary>
		public ICollection<ExistentialEdge<T>> Edges ()
		{
			var ret = new HashSet<ExistentialEdge<T>> ();
			for (int i = 0; i < NodeCount; i++)
				// Si es simétrico, no repetir aristas.
				for (int j = 0; j < (IsSymmetric ? i + 1 : NodeCount); j++)
					if (Data[i, j].Exists)
						ret.Add (Data[i, j]);
			return ret;
		}

		/// <summary>
		/// Gets a new edge for the specified end points.
		/// </summary>
		protected abstract ExistentialEdge<T> GenerateNewEdge (T origin, T destination);

		/// <summary>
		/// Clears the edges.
		/// </summary>
		protected virtual void ClearData ()
		{
			foreach (var x in Data)
				x.Exists = false;
		}

		/// <summary>
		/// Gets the edge.
		/// </summary>
		/// <param name="origin">Index of the origin node.</param>
		/// <param name="destination">Index of the destination node.</param>
		protected ExistentialEdge<T> GetEdge (int origin, int destination)
		{
			if (destination < origin || !IsSymmetric)
				return Data[origin, destination];
			return Data[destination, origin];
		}

		/// <summary>
		/// Gets the index of the specified node.
		/// </summary>
		protected int IndexOf (T node)
		{
			return Array.FindIndex (NodeArray, z => NodeComparer.Equals (node, z));
		}

		/// <summary>
		/// Gets the edge.
		/// </summary>
		protected abstract ExistentialEdge<T> GetEdge (T origin, T destination);

		/// <summary>
		/// Gets a subgraph generated by the specified node collection.
		/// </summary>
		protected abstract IGraph<T> Subgraph (IEnumerable<T> nodeSubset);

		void InitData ()
		{

			for (int i = 0; i < NodeCount; i++)
				for (int j = 0; j < NodeCount; j++)
				{
					var aris = GenerateNewEdge (NodeArray[i], NodeArray[j]);
					Data[i, j] = aris;
				}
		}

		IEnumerable<IEdge<T>> IGraph<T>.Edges () => Edges ();

		ICollection<T> IGraph<T>.Neighborhood (T node) => OutwardNeighborhood (node);

		IGraph<T> IGraph<T>.Subgraph (IEnumerable<T> nodeSubset) => Subgraph (nodeSubset);

		/// <summary>
		/// Occurs when the graph is cleared.
		/// </summary>
		public event EventHandler Cleared;
	}
}