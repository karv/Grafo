using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Graficas.Aristas;
using Graficas.Rutas;

namespace Graficas.Grafo.Dinámicos
{
	/// <summary>
	/// Representa un grafo visto como una función que a cada nodo le asigna su vecindad.
	/// </summary>
	public class GrafoVecindad<TNode> : IGrafo<TNode>
	{
		#region Ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="Graficas.Grafo.Dinámicos.GrafoVecindad{TNode}"/> class.
		/// </summary>
		/// <param name="simétrico">If set to <c>true</c> simétrico.</param>
		/// <param name="comparador">Comparador.</param>
		public GrafoVecindad (bool simétrico = false,
		                      IEqualityComparer<TNode> comparador = null)
		{
			Comparador = comparador ?? EqualityComparer<TNode>.Default;
			Simétrico = simétrico;
			Vecindad = new Dictionary<TNode, HashSet<TNode>> (Comparador);
		}

		#endregion

		#region Interno

		/// <summary>
		/// Devuelve el comparador que se usa para los nodos
		/// </summary>
		/// <value>The comparador.</value>
		public IEqualityComparer<TNode> Comparador { get; }

		/// <summary>
		/// El diccionario que asigna a cada nodo su vecindad
		/// </summary>
		/// <value>The vecindad.</value>
		protected Dictionary<TNode, HashSet<TNode>> Vecindad { get; }

		/// <summary>
		/// Devuelve un <c>bool</c> que indica si este grafo es tratato como simétrico
		/// </summary>
		/// <value><c>true</c> if simétrico; otherwise, <c>false</c>.</value>
		public bool Simétrico { get; }

		#endregion

		#region Grafo

		/// <summary>
		/// Elimina aristas.
		/// </summary>
		public void ClearEdges ()
		{
			foreach (var x in Vecindad.Values)
				x.Clear ();
		}

		/// <summary>
		/// Elimina nodos y aristas.
		/// </summary>
		public void Clear ()
		{
			Vecindad.Clear ();
		}

		/// <summary>
		/// Colección de aristas existentes.
		/// </summary>
		/// <remarks>No preserva referencia </remarks>
		public ICollection<IArista<TNode>> Aristas ()
		{
			var ret = new HashSet<IArista<TNode>> ();
			foreach (var x in Vecindad)
				foreach (var y in x.Value)
					ret.Add (new AristaBool<TNode> (x.Key, y, true, true, Simétrico));
			return ret;
		}

		/// <summary>
		/// Devuelve el <see cref="HashSet{TNode}"/> que contiene la información interna de la vecindad de un nodo.
		/// </summary>
		/// <param name="node">Nodo</param>
		/// <exception cref="T:Graficas.Grafo.NodoInexistenteException">Ocurre cuando el nodo no está en el grafo</exception>
		protected HashSet<TNode> ReferencePreservingNeighborhood (TNode node)
		{
			try
			{
				return Vecindad [node];
			}
			catch (KeyNotFoundException ex)
			{
				var m = string.Format (
					        "Cannot get node {0}.", node);
				throw new NodoInexistenteException (m, ex);
			}
		}

		/// <summary>
		/// Devuelve una copia de la vecindad de un nodo dado.
		/// </summary>
		/// <param name="nodo">Nodoa a considedad su vecindad</param>
		public ICollection<TNode> Vecinos (TNode nodo)
		{
			return new HashSet<TNode> (
				ReferencePreservingNeighborhood (nodo),
				Comparador);
		}

		/// <summary>
		/// Convierte una sucesión consistente de nodos a una ruta
		/// </summary>
		/// <returns>The ruta.</returns>
		/// <param name="seq">Sucesión consistente.</param>
		public IRuta<TNode> ToRuta (IEnumerable<TNode> seq)
		{
			var ret = new Ruta<TNode> ();
			bool iniciando = true; // Flag que indica que está construyendo el primer nodo (no paso)
			TNode last = default(TNode);
			foreach (var x in seq)
			{
				if (iniciando)
				{
					iniciando = false;
					ret = new Ruta<TNode> ();
				}
				else
				{
					if (!Vecinos (last).Contains (x))
						throw new RutaInconsistenteException ("La sucesión dada no representa una ruta.");
					ret.Concat (new Paso<TNode> (last, x));
				}
				last = x;
			}
			return ret;
		}

		/// <summary>
		/// Calcula el subgrafo generado por un subconjutno de Nodos
		/// </summary>
		/// <param name="conjunto">Conjunto de nodos para calcular el subgrafo</param>
		public GrafoVecindad<TNode> Subgrafo (IEnumerable<TNode> conjunto)
		{
			if (conjunto == null)
				throw new ArgumentNullException ("conjunto");
			try
			{
				var ret = new GrafoVecindad<TNode> (Simétrico, Comparador);
				foreach (var c in conjunto)
				{
					ret.AddNode (c);
					foreach (var n in ReferencePreservingNeighborhood (c).Where (z => conjunto.Contains (z)))
						ret.ReferencePreservingNeighborhood (c).Add (n);
				}					
				return ret;
			}
			catch (Exception ex)
			{
				var m = string.Format ("No se puede calcular el subgrafo de esta clase respecto a {0}.\n" +
				        "¿Es el argumento un subconjunto de Nodos?", conjunto);
				throw new ArgumentException (m, "conjunto", ex);
			}
		}

		IGrafo<TNode> IGrafo<TNode>.Subgrafo (IEnumerable<TNode> conjunto)
		{
			return Subgrafo (conjunto);
		}

		/// <summary>
		/// El valor de existencia de la arista correspondiente a un par de puntos
		/// </summary>
		/// <param name="desde">Origen</param>
		/// <param name="hasta">Destino</param>
		public bool this [TNode desde, TNode hasta]
		{
			get
			{
				bool ret = Vecindad [desde].Contains (hasta);
				return ret;
			}
			set
			{
				establecerArista (desde, hasta, value);
				if (Simétrico)
					establecerArista (hasta, desde, value);
			}
		}

		void establecerArista (TNode desde, TNode hasta, bool valor)
		{
			if (valor)
				AgregaArista (desde, hasta);
			else
				EliminaArista (desde, hasta);
		}

		/// <summary>
		/// Elimina una arista
		/// </summary>
		/// <param name="desde">Desde.</param>
		/// <param name="hasta">Hasta.</param>
		protected void EliminaArista (TNode desde, TNode hasta)
		{
			var vec = Vecindad [desde];
			vec.Remove (hasta);
		}

		/// <summary>
		/// Agrega una arista
		/// </summary>
		/// <param name="desde">Desde.</param>
		/// <param name="hasta">Hasta.</param>
		protected void AgregaArista (TNode desde, TNode hasta)
		{
			var vec = ReferencePreservingNeighborhood (desde);
			vec.Add (hasta);
		}

		/// <summary>
		/// Devuelve una arista que representa al estado de la arista en el grafo
		/// </summary>
		/// <param name="desde">Desde.</param>
		/// <param name="hasta">Hasta.</param>
		public AristaBool<TNode> Arista (TNode desde, TNode hasta)
		{
			bool ret = Vecindad [desde].Contains (hasta);
			return new AristaBool<TNode> (desde, hasta, ret, true, Simétrico);
		}

		IArista<TNode> IGrafo<TNode>.this [TNode desde, TNode hasta]
		{
			get
			{
				return Arista (desde, hasta);
			}
		}

		/// <summary>
		/// Devuelve una colección sólo lectura de sus nodos
		/// </summary>
		/// <value>The nodos.</value>
		public ReadOnlyCollection<TNode> Nodos
		{
			get { return new List<TNode> (Vecindad.Keys).AsReadOnly (); }
		}

		ICollection<TNode> IGrafo<TNode>.Nodos
		{
			get { return Nodos; }
		}

		/// <summary>
		/// Devuelve un valor que indica si un nodo pertenece al grafo
		/// </summary>
		/// <param name="node">Nodo</param>
		public bool ExistNode (TNode node)
		{
			return Vecindad.ContainsKey (node);
		}

		#endregion

		#region Dinamicidad

		/// <summary>
		/// Agrega un nodo aislado al grafo
		/// </summary>
		/// <param name="node">Nodo</param>
		public void AddNode (TNode node)
		{
			if (ExistNode (node))
				throw new InvalidOperationException ("Cannot add an existing node");

			Vecindad.Add (node, new HashSet<TNode> (Comparador));
		}

		/// <summary>
		/// Agrega un nodo al grafo, determinando su vecindad
		/// </summary>
		/// <param name="node">Nodo</param>
		/// <param name="vecindad">Vecindad</param>
		public void AddNode (TNode node, IEnumerable<TNode> vecindad)
		{
			if (ExistNode (node))
				throw new InvalidOperationException ("Cannot add an existing node");

			Vecindad.Add (node, new HashSet<TNode> (vecindad, Comparador));
		}

		/// <summary>
		/// Elimina un nodo del grafo
		/// </summary>
		/// <param name="node">Nodo</param>
		public void RemoveNode (TNode node)
		{
			Vecindad.Remove (node);
			foreach (var nodo in Vecindad.Values)
				nodo.Remove (node);
		}

		#endregion
	}
}