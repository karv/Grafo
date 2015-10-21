using System;
using System.Collections.Generic;

namespace Graficas
{
	public class Nodo<T>: INodo<T>
	{
		public Nodo(T obj)
		{
			Obj = obj;
			Vecindad = new List<INodo<T>>();
		}

		public T Obj { get; }

		public List<INodo<T>> Vecindad;

		#region INodo implementation

		T INodo<T>.Objeto
		{
			get
			{
				return Obj;
			}
		}

		IEnumerable<INodo<T>> INodo<T>.Vecindad
		{
			get
			{
				return Vecindad;
			}
		}



		#endregion
	}
}

