using System;
using System.Collections.Generic;
using Graficas;

namespace Graficas.Nodos
{
	/// <summary>
	/// Representa el conjunto de nodos de una gráfica.
	/// </summary>
	public class ConjuntoNodos<T> :  IEnumerable<IHardNodo<T>>, IGrafica<T>, ICollection<T>
	// where T : IEquatable<T>
	{
		List<IHardNodo<T>> _dat = new List<IHardNodo<T>>();

		public ConjuntoNodos()
		{
			//_nodos = new List<IHardNodo<T>>();
		}

		/// <param name="graf">Gráfica de dónde copiar la información.</param>
		public ConjuntoNodos(IGrafica<T> graf) : this()
		{
			// Primero crear los nodos
			foreach (var x in graf.Nodos)
			{
				Add(x);
			}
			foreach (var x in _dat)
			{
				foreach (var y in graf.Vecinos(x.getObjeto))
				{
					x.getSucc.Add(this[y]);
				}
			}
		}

		//List<IHardNodo<T>> _nodos;

		#region IGrafica implementation

		ICollection<T> IGrafica<T>.Vecinos(T nodo)
		{
			var ret = new List<T>();
			foreach (var x in this[nodo].getSucc)
			{
				ret.Add(x.getObjeto);
			}
			return ret;
		}

		ICollection<T> IGrafica<T>.Nodos
		{
			get
			{
				return _dat.ConvertAll(x => x.getObjeto);
			}
		}

		bool IGrafica<T>.ExisteArista(T desde, T hasta)
		{
			throw new NotImplementedException();
		}

		void IGrafica<T>.AgregaArista(T desde, T hasta)
		{
			throw new NotImplementedException();
		}

		Graficas.Rutas.IRuta<T> IGrafica<T>.ToRuta(IEnumerable<T> seq)
		{
			throw new NotImplementedException();
		}

		bool IGrafica<T>.EsSimétrico
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region IEnumerable implementation

		public IEnumerator<IHardNodo<T>> GetEnumerator()
		{
			return _dat.GetEnumerator();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return _dat.ConvertAll(x => x.getObjeto).GetEnumerator();
		}

		#endregion

		#region IEnumerable implementation

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _dat.GetEnumerator();
		}

		#endregion

		public IHardNodo<T> this [T Key]
		{
			get
			{
				return _dat.Find(x => x.getObjeto.Equals(Key));
			}
		}

		#region ICollection

		public void Add(T item)
		{
			
			if (Contains(item))
				throw new Exception("Ya se encuentra nodo.");

			_dat.Add(new HardNodo<T>(item));
		}

		public void Clear()
		{
			_dat.Clear();
		}

		public bool Contains(T item)
		{
			return _dat.Exists(x => x.getObjeto.Equals(item));
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(T item)
		{
			return _dat.RemoveAll(x => x.getObjeto.Equals(item)) > 0;
		}

		public int Count
		{
			get
			{
				return _dat.Count;
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

	public interface IHardNodo<T> : INodo<T>
	{
		new ICollection<IHardNodo<T>> getSucc { get; }
		//void SetSucc (IEnumerator<IHardNodo> )
	}
}

