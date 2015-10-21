using System;
using System.Collections.Generic;
using Graficas;
using Graficas.Rutas;
using System.Linq;

namespace Graficas.Nodos
{
	/// <summary>
	/// Representa el conjunto de nodos de una gráfica.
	/// </summary>
	public class ConjuntoNodos<T> :  IGrafo<T> // TEST todo
		where T : IEquatable<T>
	{
		HashSet<Nodo<T>> _nodos = new HashSet<Nodo<T>>();

		Nodo<T> AsNodo(T obj)
		{
			foreach (var x in _nodos)
			{
				if (x.Objeto.Equals(obj))
					return x;
			}
			// Si existe, lo agrego
			var ret = new Nodo<T>(obj);
			_nodos.Add(ret);
			return ret;
		}

		public ICollection<HardArista<T>> Aristas()
		{
			var ret = new List<HardArista<T>>();
			foreach (var x in _nodos)
			{
				foreach (var y in x.Vecindad)
				{
					ret.Add(new HardArista<T>(x, y));
				}
			}

			return ret;
		}

		ICollection<IArista<T>> ILecturaGrafo<T>.Aristas()
		{
			return Aristas() as ICollection<IArista<T>>;
		}

		public IRuta<T> ToRuta(IEnumerable<T> seq)
		{
			var Nods = new List<Nodo<T>>();
			foreach (var x in seq)
			{
				Nods.Add(AsNodo(x));
			}
			return new HardRuta<T>(Nods);
		}

		public ILecturaGrafo<T> Subgrafo(IEnumerable<T> conjunto)
		{
			var ret = new ConjuntoNodos<T>();
			foreach (var x in conjunto)
			{
				ret.Add(x);
			}

			foreach (var x in conjunto)
			{
				var nodoX = ret.AsNodo(x);
				foreach (var y in AsNodo(x).Vecindad)
				{
					if (conjunto.Contains(y.Objeto))
					{
						nodoX.Vecindad.Add(y);
					}
				}
			}

			return ret;
		}

		public ConjuntoNodos()
		{
		}

		/// <param name="graf">Gráfica de dónde copiar la información.</param>
		public ConjuntoNodos(IGrafo<T> graf) : this()
		{
			// Primero crear los nodos
			foreach (var x in graf.Nodos)
			{
				Add(x);
			}

			// Hacer la topología
			foreach (var item in graf.Nodos)
			{
				Nodo<T> nodoDeItem = AsNodo(item);
				foreach (var x in graf.Vecinos(item))
				{
					nodoDeItem.Vecindad.Add(AsNodo(x));
				}
			}
		}

		public bool this [T desde, T hasta]
		{
			get
			{
				foreach (var x in AsNodo(desde).Vecindad)
				{
					if (x.Objeto.Equals(hasta))
						return true;
				}
				return false;
			}
			set
			{
				if (value)
					AsNodo(desde).Vecindad.Add(AsNodo(hasta));
				else
					AsNodo(desde).Vecindad.Remove(AsNodo(hasta));
			}
		}

		//List<IHardNodo<T>> _nodos;

		#region IGrafica implementation

		ICollection<T> ILecturaGrafo<T>.Vecinos(T nodo)
		{
			var ret = new List<T>();
			foreach (var x in this[nodo].Vecindad)
			{
				ret.Add(x.Objeto);
			}
			return ret;
		}

		public ICollection<T> Nodos
		{
			get
			{
				var ret = new List<T>(_nodos.Count);
				foreach (var x in _nodos)
				{
					ret.Add(x.Objeto);
				}
				return ret;
			}
		}


		#endregion


		public INodo<T> this [T Key]
		{
			get
			{
				return AsNodo(Key);
			}
		}

		#region ICollection

		public void Add(T item)
		{
			
			if (Contains(item))
				throw new Exception("Ya se encuentra nodo.");

			_nodos.Add(new Nodo<T>(item));
		}

		public void Clear()
		{
			_nodos.Clear();
		}

		public bool Contains(T item)
		{
			foreach (var x in _nodos)
			{
				if (x.Objeto.Equals(item))
					return true;
			}
			return false;
		}

		public void Remove(T item)
		{
			_nodos.Remove(AsNodo(item));
		}

		public int Count
		{
			get
			{
				return _nodos.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		#endregion
	}

}