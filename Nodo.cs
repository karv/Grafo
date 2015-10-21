using System.Collections.Generic;

namespace Graficas
{
	public class Nodo<T>: INodo<T>
	{
		public Nodo (T obj)
		{
			Objeto = obj;
			Vecindad = new List<Nodo<T>> ();
		}

		public T Objeto { get; }

		public List<Nodo<T>> Vecindad;

		#region INodo implementation

		T INodo<T>.Objeto
		{
			get
			{
				return Objeto;
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