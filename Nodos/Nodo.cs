using System.Collections.Generic;

namespace Graficas.Nodos
{
	/// <summary>
	/// Representa un nodo que vincula a su vecindad
	/// </summary>
	public class Nodo<T> : INodo<T>
	{
		/// <param name="obj">Objecto del nodo</param>
		public Nodo (T obj)
		{
			Objeto = obj;
			Vecindad = new List<Nodo<T>> ();
		}

		/// <summary>
		/// Devuelve el objeto asociado al nodo
		/// </summary>
		/// <value>The objeto.</value>
		public T Objeto { get; }

		/// <summary>
		/// Vecindad del nodo
		/// </summary>
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