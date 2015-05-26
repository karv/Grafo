using System;
using System.Collections.Generic;

namespace Graficas
{
	public class Nodo<T>: INodo<T>
	{
		public Nodo(T obj)
		{
			_obj = obj;
			_succ = new List<T>();
		}

		/// <summary>
		/// Construye un nodo que esté sincronizado a un "nodo" de una gráfica dada.
		/// </summary>
		/// <param name="obj">Objeto del nodo.</param>
		/// <param name="graf">Gráfica donde se encuentra el nodo.</param>
		public Nodo(T obj, IGrafica<T> graf)
		{
			_obj = obj;
			_succ = graf.Vecinos(obj);
		}

		T _obj;
		IEnumerable<T> _succ;

		#region INodo implementation

		T INodo<T>.getObjeto
		{
			get
			{
				return _obj;
			}
		}

		System.Collections.Generic.IEnumerable<T> INodo<T>.getSucc
		{
			get
			{
				return (IEnumerable<T>)_succ;
			}
		}

		#endregion
	}
}

