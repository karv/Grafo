using System;
using System.Collections.Generic;
using System.Linq;
using Graficas.Edges;
using Graficas.Nodos;
using Graficas.Rutas;

namespace Graficas.Grafo
{
	/// <summary>
	/// Representa el conjunto de nodos de una gráfica.
	/// </summary>
	[Serializable]
	[Obsolete]
	public class HardGrafo<T> : IGrafo<T>
		where T : IEquatable<T>
	{
		readonly HashSet<Nodo<T>> _nodos = new HashSet<Nodo<T>> ();

		/// <summary>
		/// Devuelve el nodo correspondiente a un objeto.
		/// </summary>
		public Nodo<T> AsNodo (T obj)
		{
			foreach (var x in _nodos)
			{
				if (x.Objeto.Equals (obj))
					return x;
			}
			// Si no existe, lo agrego
			var ret = new Nodo<T> (obj);
			_nodos.Add (ret);
			return ret;
		}

		/// <summary>
		/// Devuelve una copia de la colección de las aristas.
		/// </summary>
		public ICollection<HardEdge<T>> Aristas ()
		{
			var ret = new List<HardEdge<T>> ();
			foreach (var x in _nodos)
			{
				foreach (var y in x.Vecindad)
				{
					ret.Add (new HardEdge<T> (x, y));
				}
			}

			return ret;
		}

		ICollection<IEdge<T>> IGrafo<T>.Edges ()
		{
			return Aristas () as ICollection<IEdge<T>>;
		}

		/// <summary>
		/// Convierte una sucesión coherente en ruta
		/// </summary>
		public IRuta<T> ToPath (IEnumerable<T> seq)
		{
			var Nods = new List<Nodo<T>> ();
			foreach (var x in seq)
			{
				Nods.Add (AsNodo (x));
			}
			return new HardRuta<T> (Nods);
		}

		/// <summary>
		/// Calcula el subgrafo generado por un subconjutno de Nodos
		/// </summary>
		/// <param name="conjunto">Conjunto de nodos para calcular el subgrafo</param>
		public HardGrafo<T> Subgrafo (IEnumerable<T> conjunto)
		{
			var ret = new HardGrafo<T> ();
			foreach (T x in conjunto.ToList ())
				ret.Add (x);

			foreach (var x in conjunto.ToList ())
			{
				var nodoX = ret.AsNodo (x);
				foreach (var y in AsNodo (x).Vecindad)
				{
					if (conjunto.Contains (y.Objeto))
					{
						nodoX.Vecindad.Add (y);
					}
				}
			}

			return ret;
		}

		IGrafo<T> IGrafo<T>.Subgraph (IEnumerable<T> conjunto)
		{
			return Subgrafo (conjunto);
		}

		/// <summary>
		/// 
		/// </summary>
		public HardGrafo ()
		{
		}

		/// <param name="graf">Gráfica de dónde copiar la información.</param>
		public HardGrafo (IGrafo<T> graf)
			: this ()
		{
			// Primero crear los nodos
			foreach (var x in graf.Nodes)
			{
				Add (x);
			}

			// Hacer la topología
			foreach (var item in graf.Nodes)
			{
				Nodo<T> nodoDeItem = AsNodo (item);
				foreach (var x in graf.Neighborhood (item))
					nodoDeItem.Vecindad.Add (AsNodo (x));
			}
		}

		/// <summary>
		/// Revisa si existe arista entre dos nodos
		/// </summary>
		/// <param name="desde">Desde.</param>
		/// <param name="hasta">Hasta.</param>
		public bool this[T desde, T hasta]
		{
			get
			{
				return AsNodo (desde).Vecindad.Any (x => x.Objeto.Equals (hasta));
			}
			set
			{
				if (value)
					AsNodo (desde).Vecindad.Add (AsNodo (hasta));
				else
					AsNodo (desde).Vecindad.Remove (AsNodo (hasta));
				limpiarNodos ();
			}
		}

		void limpiarNodos ()
		{
			// _nodos.RemoveWhere (x => !x.Vecindad.Any ());
		}

		#region IGrafica implementation

		IEdge<T> IGrafo<T>.this[T desde, T hasta]
		{
			get
			{
				return new ExistentialEdge<T> (desde, hasta, this[desde, hasta], true);
			}
		}

		ICollection<T> IGrafo<T>.Neighborhood (T nodo)
		{
			var ret = new List<T> ();
			foreach (var x in this[nodo].Vecindad)
			{
				ret.Add (x.Objeto);
			}
			return ret;
		}

		/// <summary>
		/// Devuelve una colección con los nodos de la gráfica
		/// </summary>
		/// <value>The nodos.</value>
		public ICollection<T> Nodes
		{
			get
			{
				return new List<T> (_nodos.Select (x => x.Objeto));
			}
		}

		#endregion

		/// <summary>
		/// Devuelve el nodo correspondiente a un valor
		/// </summary>
		public INodo<T> this[T key]
		{
			get
			{
				return AsNodo (key);
			}
		}

		#region ICollection

		/// <summary>
		/// Agrega un nodo a la gráfica
		/// </summary>
		public void Add (T item)
		{

			if (Contains (item))
				throw new Exception ("Ya se encuentra nodo.");

			_nodos.Add (new Nodo<T> (item));
		}

		/// <summary>
		/// Elimina cada nodo de la gráfica
		/// </summary>
		public void Clear ()
		{
			_nodos.Clear ();
		}

		/// <summary>
		/// Contains the specified node..
		/// </summary>
		/// <param name="item">Item.</param>
		public bool Contains (T item)
		{
			foreach (var x in _nodos)
			{
				if (x.Objeto.Equals (item))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Remove the specified node,
		/// </summary>
		/// <param name="item">Item.</param>
		public void Remove (T item)
		{
			_nodos.Remove (AsNodo (item));
		}

		/// <summary>
		/// Devuelve el número de nodos.
		/// </summary>
		/// <value>The count.</value>
		public int Count
		{
			get
			{
				return _nodos.Count;
			}
		}

		#endregion
	}
}