﻿using System;
using System.Collections.Generic;
using System.Linq;
using Graficas.Edges;
using Graficas.Rutas;
using ListasExtra;

namespace Graficas.Grafo.Estáticos
{
	/// <summary>
	/// Representa una gráfica, en el sentido abstracto.
	/// Los nodos son del tipo <c>T</c>; y las aristas almacenan un valor del tipo <c>TData</c>
	/// </summary>
	[Serializable]
	public class Graph<T, TData> : CommonGraph<T>, IGraph<T>
	{
		/// <summary>
		/// Gets or sets the weight of an edge.
		/// </summary>
		public TData this[T x, T y]
		{
			get
			{
				var ar = FindEdge (x, y);
				return ar.Data; // Remark: verificación de existencia lo hace la propiedad Data
			}
			set
			{
				if (IsReadOnly)
					throw new InvalidOperationException ("Graph is readonly.");
				WeightedEdge<T, TData> aris = FindEdge (x, y);
				aris.Exists = true;
				aris.Data = value;
			}
		}

		IEdge<T> IGraph<T>.this[T desde, T hasta] => FindEdge (desde, hasta);
		IEnumerable<T> IGraph<T>.Nodes => NodeArray;

		/// <summary>
		/// Initializes a mutable instance.
		/// </summary>
		/// <param name="nodes">Collection of nodes.</param>
		/// <param name="sym">If set to <c>true</c> is symmetric</param>
		public Graph (ICollection<T> nodes, bool sym = false)
			: base (nodes, sym, false) { }

		/// <summary>
		/// Clones a graph.
		/// </summary>
		/// <param name="readOnly">Sets whether the resulting graph is readonly.</param>
		/// <param name="graph">Source graph.</param>
		public Graph (IGraph<T> graph, bool readOnly = true)
			: base (graph.Nodes, false, readOnly)
		{
			for (int i = 0; i < NodeCount; i++)
				for (int j = 0; j < NodeCount; j++)
				{
					var ori = NodeArray[i];
					var des = NodeArray[j];
					var ari = graph[ori, des].Exists;
					if (ari)
						Data[i, j] = new WeightedEdge<T, TData> (
							ori,
							des,
							default (TData),
							readOnly);
					else
						Data[i, j] = new WeightedEdge<T, TData> (
							ori,
							des,
							readOnly);
				}
		}

		/// <summary>
		/// Initializes an instance.
		/// </summary>
		/// <param name="nodes">Collection of nodes.</param>
		/// <param name="sym">If set to <c>true</c> is symmetric</param>
		/// <param name="readOnly">If set to <c>true</c> sólo lectura.</param>
		protected Graph (ICollection<T> nodes, bool sym, bool readOnly)
			: base (nodes, sym, readOnly) { }

		/// <summary>
		/// Returns the graph generated by a subset of nodes.
		/// </summary>
		/// <param name="nodeSet">Conjunto de nodos para calcular el subgrafo</param>
		/// <remarks>Preserves edges.</remarks>
		public Graph<T, TData> GetSubgraph (IEnumerable<T> nodeSet)
		{
			if (nodeSet.Any (z => !Nodes.Contains (z)))
				throw new ArgumentException ("Specified set is not a subset of Nodes.", nameof (nodeSet));
			var ret = new Graph<T, TData> (
									new HashSet<T> (nodeSet),
									IsSymmetric,
									IsReadOnly);

			for (int i = 0; i < ret.NodeCount; i++)
			{
				// ii  es el índice en this que contiene al i-ésimo elemento de ret.Nodos
				var ii = ret.IndexOf (ret.NodeArray[i]);
				for (int j = 0; j < ret.NodeCount; j++)
				{
					// ij  es el índice en this que contiene al j-ésimo elemento de ret.Nodos
					var ij = ret.IndexOf (ret.NodeArray[j]);
					ret.Data[i, j] = Data[ii, ij];
				}
			}
			return ret;
		}

		/// <summary>
		/// Clones this graph.
		/// </summary>
		/// <param name="readOnly">Determines whether the resulting graph is read only.</param>
		/// <remarks>Preservs links to edges.</remarks>
		public Graph<T, TData> Clone (bool readOnly = false)
		{
			var ret = new Graph<T, TData> (NodeArray, IsSymmetric, readOnly);
			for (int i = 0; i < NodeCount; i++)
				for (int j = 0; j < (IsSymmetric ? i + 1 : NodeCount); j++)
				{
					var x = Data[i, j] as WeightedEdge<T, TData>; // La arista iterando
					if (x.Exists)
					{
						ret.Data[i, j] = new WeightedEdge<T, TData> (
							x.Origin,
							x.Destination,
							x.Data,
							x.Readonly,
							x.IsSymmetric);
					}
				}
			return ret;
		}

		/// <summary>
		/// Gets a readonly copy, which preservers edge references.
		/// </summary>
		public Graph<T, TData> AsReadonly ()
		{
			var ret = new Graph<T, TData> (NodeArray, IsSymmetric, true);
			for (int i = 0; i < NodeCount; i++)
				for (int j = 0; j < (IsSymmetric ? i + 1 : NodeCount); j++)
					ret.Data[i, j] = Data[i, j];
			return ret;
		}

		/// <summary>
		/// Gets the edge by specifing the end points.
		/// </summary>
		public WeightedEdge<T, TData> FindEdge (T origen, T destino)
		{
			return (WeightedEdge<T, TData>)GetEdgeSym (origen, destino);
		}

		/// <summary>
		/// </summary>
		[Obsolete ("Into a new class.")]
		public IPath<T> CaminoÓptimo (T x, T y, Func<WeightedEdge<T, TData>, float> peso)
		{
			if (x.Equals (y))
				return null;
			var ign = new HashSet<T> { x, y };
			return CaminoÓptimo (x, y, peso, ign);
		}

		/// <summary>
		/// Gets a subgraph generated by the specified node collection.
		/// </summary>
		protected override IGraph<T> Subgraph (IEnumerable<T> nodeSubset) => GetSubgraph (nodeSubset);

		/// <summary>
		/// Gets a new edge for the specified end points.
		/// </summary>
		protected override ExistentialEdge<T> GenerateNewEdge (T origin, T destination)
		{
			return new WeightedEdge<T, TData> (origin, destination, IsReadOnly, IsSymmetric);
		}

		/// <summary>
		/// Gets the edge.
		/// </summary>
		protected override ExistentialEdge<T> GetEdge (T origin, T destination)
		{
			return FindEdge (origin, destination);
		}

		/// <summary>
		/// Calcula la ruta óptima de un nodo a otro.
		/// </summary>
		/// <param name="x">Nodo inicial.</param>
		/// <param name="y">Nodo final.</param>
		/// <param name="ignorar">Lista de nodos a evitar.</param>
		/// <param name="peso">Función que asigna a cada arista su peso</param>
		/// <returns>Devuelve la ruta de menor <c>Longitud</c>.</returns>
		/// <remarks>Puede ciclar si no existe ruta de x a y.</remarks> 
		/// <remarks>Devuelve ruta vacía (no nula) si origen es destino </remarks>
		/// <remarks>Devuelve null si toda ruta de x a y toca a ignorar</remarks>
		[Obsolete]
		Path<T> CaminoÓptimo (T x,
													T y,
													Func<WeightedEdge<T, TData>, float> peso,
													ISet<T> ignorar)
		{
			Path<T> ret = null;
			float longRet = 0;

			var arisXY = FindEdge (x, y);
			if (arisXY.Exists)
				return new Path<T> (x, y);

			var consideradNodos = new HashSet<T> (InwardNeighborhood (y));
			consideradNodos.ExceptWith (ignorar);

			if (!consideradNodos.Any ())
				return null;
			foreach (var v in consideradNodos)
			{
				var ignorarRecursivo = new HashSet<T> (ignorar) { v };
				var mejorRuta = CaminoÓptimo (x, v, peso, ignorarRecursivo);
				if (mejorRuta != null)
				{
					var últAris = FindEdge (v, y);
					if (!últAris.Exists)
						throw new Exception ("Unknown exception.");
					mejorRuta.Concat (últAris, v);
					float longÚlt = 0;
					foreach (var p in mejorRuta.Pasos)
						longÚlt += peso (FindEdge (p.Origin, p.Destination));
					if (ret == null || longÚlt < longRet)
					{
						ret = mejorRuta;
						longRet = longÚlt;
					}
				}
			}

			return ret;
		}

		/// <summary>
		/// Selecciona pseudoaleatoriamente una sublista de tamaño fijo de una lista dada.
		/// </summary>
		/// <param name="r"></param>
		/// <param name="n">Número de elementos a seleccionar.</param>
		/// <param name="lista">Lista de dónde seleccionar la sublista.</param>
		/// <returns>Devuelve una lista con los elementos seleccionados.</returns>
		[Obsolete ("Not the place for this function.")]
		List<object> SeleccionaPeso (Random r, int n, ListaPeso<object> lista)
		{
			List<object> ret;
			float Suma = 0;
			float rn;
			if (n == 0)
				return new List<object> ();
			else
			{
				ret = SeleccionaPeso (r, n - 1, lista);

				foreach (var x in ret)
				{
					lista[x] = 0;
				}

				// Ahora seleecionar uno.
				Suma = 0;
				rn = (float)r.NextDouble () * lista.SumaTotal ();

				foreach (var x in lista.Keys)
				{
					Suma += lista[x];
					if (Suma >= rn)
					{
						ret.Add (x);
						return ret;
					}
				}
				return null;
			}
		}

		IGraph<T> IGraph<T>.Subgraph (IEnumerable<T> conjunto) => GetSubgraph (conjunto);
		IEnumerable<IEdge<T>> IGraph<T>.Edges () => Data.Cast<WeightedEdge<T, TData>> ().Where (x => x.Exists);
		ICollection<T> IGraph<T>.Neighborhood (T nodo) => OutwardNeighborhood (nodo);
	}

	/// <summary>
	/// An unweighted graph.
	/// </summary>
	public class Grafo<T> : CommonGraph<T>, IGraph<T>
	{
		/// <summary>
		/// Gets or sets indicating whether there is an edge bewteen two edges.
		/// </summary>
		public bool this[T x, T y]
		{
			get => GetEdgeSym (x, y).Exists;
			set
			{
				if (IsReadOnly)
					throw new InvalidOperationException ("Graph is readonly.");
				GetEdgeSym (x, y).Exists = value;
			}
		}

		IEdge<T> IGraph<T>.this[T desde, T hasta] => GetEdgeSym (desde, hasta);
		IEnumerable<T> IGraph<T>.Nodes => NodeArray;

		/// <summary>
		/// Initializes a mutable graph.
		/// </summary>
		/// <param name="nodes">Collection of nodes.</param>
		/// <param name="sym">If set to <c>true</c> es symmetric.</param>
		public Grafo (ICollection<T> nodes, bool sym = false)
			: base (nodes, sym, false)
		{
		}

		/// <summary>
		/// Clones a graph.
		/// </summary>
		/// <param name="sóloLectura">If set to <c>true</c> this graph is read-only.</param>
		/// <param name="graf">Source graph.</param>
		public Grafo (IGraph<T> graf, bool sóloLectura = true)
			: base (graf.Nodes, false, sóloLectura)
		{
			for (int i = 0; i < NodeCount; i++)
				for (int j = 0; j < NodeCount; j++)
				{
					var ori = NodeArray[i];
					var des = NodeArray[j];
					Data[i, j] = new ExistentialEdge<T> (
						ori,
						des,
						graf[ori, des].Exists,
						sóloLectura);
				}
		}

		/// <summary>
		/// Initializes a graph
		/// </summary>
		/// <param name="nodes">Collection of nodes.</param>
		/// <param name="sym">If set to <c>true</c> es symmetric.</param>
		/// <param name="readOnly">Specified a value indicating whether this graph is read-only.</param>
		protected Grafo (ICollection<T> nodes, bool sym, bool readOnly)
			: base (nodes, sym, readOnly)
		{
		}

		/// <summary>
		/// Clones this graph.
		/// </summary>
		/// <param name="sóloLectura">Sets whether the returning graph is read-only.</param>
		/// <remarks>Edges are deferenced.</remarks>
		public Grafo<T> Clonar (bool sóloLectura = false)
		{
			var ret = new Grafo<T> (NodeArray, IsSymmetric, sóloLectura);
			for (int i = 0; i < NodeCount; i++)
				for (int j = 0; j < (IsSymmetric ? i + 1 : NodeCount); j++)
				{
					var x = Data[i, j]; // La arista iterando
					ret.Data[i, j] = new ExistentialEdge<T> (
						x.Origin,
						x.Destination,
						x.Exists,
						x.Readonly,
						x.IsSymmetric);
				}
			return ret;
		}

		/// <summary>
		/// Gets a clone preserving references.
		/// </summary>
		/// <returns>Source graph.</returns>
		public Grafo<T> AsReadonly () => Clonar (true);

		/// <summary>
		/// Gets a subgraph generated by the specified node collection.
		/// </summary>
		public Grafo<T> GetSubgraph (ICollection<T> nodeSubset)
		{
			var ret = new Grafo<T> (nodeSubset, IsSymmetric, IsReadOnly);

			if (nodeSubset.Any (x => !NodeArray.Contains (x)))
				throw new ArgumentException (string.Format ("{1} must be a subset of {0}.", nameof (Nodes), nameof (nodeSubset)));

			foreach (var x in new List<T> (nodeSubset))
				foreach (var y in new List<T> (nodeSubset))
					ret[x, y] = this[x, y];
			return ret;
		}

		/// <summary>
		/// Devuelve a ruta de menor longitud entre dos puntos.
		/// </summary>
		[Obsolete]
		public IPath<T> CaminoÓptimo (T x, T y)
		{
			if (x.Equals (y))
				return null;
			var ign = new HashSet<T> { x, y };
			return CaminoÓptimo (x, y, ign);
		}

		/// <summary>
		/// Gets a subgraph generated by the specified node collection.
		/// </summary>
		protected override IGraph<T> Subgraph (IEnumerable<T> nodeSubset) => GetSubgraph (nodeSubset.ToArray ());

		/// <summary>
		/// Gets a new edge for the specified end points.
		/// </summary>
		protected override ExistentialEdge<T> GenerateNewEdge (T origin, T destination)
		{
			return new ExistentialEdge<T> (origin, destination, false, IsReadOnly, IsSymmetric);
		}

		/// <summary>
		/// Gets the edge.
		/// </summary>
		protected override ExistentialEdge<T> GetEdge (T origin, T destination)
		{
			return GetEdgeSym (origin, destination);
		}

		IGraph<T> IGraph<T>.Subgraph (IEnumerable<T> conjunto)
		{
			return GetSubgraph (new HashSet<T> (conjunto));
		}

		ICollection<T> IGraph<T>.Neighborhood (T nodo)
		{
			return OutwardNeighborhood (nodo);
		}

		IEnumerable<IEdge<T>> IGraph<T>.Edges ()
		{
			return Edges ();
		}

		/// <summary>
		/// Calcula la ruta óptima de un nodo a otro.
		/// </summary>
		/// <param name="x">Nodo inicial.</param>
		/// <param name="y">Nodo final.</param>
		/// <param name="ignorar">Lista de nodos a evitar.</param>
		/// <returns>Devuelve la ruta de menor <c>Longitud</c>.</returns>
		/// <remarks>Puede ciclar si no existe ruta de x a y.</remarks> 
		/// <remarks>Devuelve ruta vacía (no nula) si origen es destino </remarks>
		/// <remarks>Devuelve null si toda ruta de x a y toca a ignorar</remarks>
		[Obsolete]
		Path<T> CaminoÓptimo (T x, T y, ISet<T> ignorar)
		{
			Path<T> ret = null;

			var arisXY = GetEdgeSym (x, y);
			if (arisXY.Exists)
				return new Path<T> (x, y);

			var consideradNodos = new HashSet<T> (InwardNeighborhood (y));
			consideradNodos.ExceptWith (ignorar);

			if (!consideradNodos.Any ())
				return null;
			foreach (var v in consideradNodos)
			{
				var ignorarRecursivo = new HashSet<T> (ignorar) { v };
				var mejorRuta = CaminoÓptimo (x, v, ignorarRecursivo);
				if (mejorRuta != null)
				{
					var últAris = GetEdgeSym (v, y);
					if (!últAris.Exists)
						throw new Exception ();
					mejorRuta.Concat (últAris, v);
					if (ret == null || mejorRuta.StepCount < ret.StepCount)
						ret = mejorRuta;
				}
			}

			return ret;
		}

		/// <summary>
		/// Selecciona pseudoaleatoriamente una sublista de tamaño fijo de una lista dada.
		/// </summary>
		/// <param name="r"></param>
		/// <param name="n">Número de elementos a seleccionar.</param>
		/// <param name="lista">Lista de dónde seleccionar la sublista.</param>
		/// <returns>Devuelve una lista con los elementos seleccionados.</returns>
		[Obsolete]
		List<object> SeleccionaPeso (Random r, int n, ListaPeso<object> lista)
		{
			List<object> ret;
			float Suma = 0;
			float rn;
			if (n == 0)
				return new List<object> ();

			ret = SeleccionaPeso (r, n - 1, lista);

			foreach (var x in ret)
			{
				lista[x] = 0;
			}

			// Ahora seleecionar uno.
			Suma = 0;
			rn = (float)r.NextDouble () * lista.SumaTotal ();

			foreach (var x in lista.Keys)
			{
				Suma += lista[x];
				if (Suma >= rn)
				{
					ret.Add (x);
					return ret;
				}
			}
			return null;
		}
	}
}