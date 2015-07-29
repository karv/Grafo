using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading;

namespace Graficas.Rutas
{
	public class Ruta<T>: IRuta<T>
	{
		IList<IPaso<T>> _paso = new List<IPaso<T>>();


		public Ruta()
		{
		}

		#region IEnumerable implementation

		public System.Collections.Generic.IEnumerator<T> GetEnumerator()
		{
			T[] ret = new T[NumPasos + 1];
			ret[0] = _paso[0].Origen;

			for (int i = 0; i < NumPasos; i++)
			{
				ret[i + 1] = _paso[i].Destino;
			}

			return ((IEnumerable<T>)ret).GetEnumerator();
		}

		#endregion

		#region IEnumerable implementation

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region IMultiRuta implementation

		/// <summary>
		/// Devuelve el origen de la ruta
		/// </summary>
		/// <value>The nodo inicial.</value>
		public T NodoInicial
		{
			get
			{
				return _paso[0].Origen;
			}
		}

		/// <summary>
		/// Devuelve el destino de la ruta
		/// </summary>
		/// <value>The nodo final.</value>
		public T NodoFinal
		{
			get
			{
				return _paso[NumPasos - 1].Destino;
			}
		}

		/// <summary>
		/// Devuelve el peso total de la ruta
		/// </summary>
		/// <value>The longitud.</value>
		public float Longitud
		{
			get
			{
				float ret = 0;
				foreach (var x in _paso)
				{
					ret += x.Peso;
				}
				return ret;
			}
		}

		/// <summary>
		/// Devuelve el número de pasos en la ruta
		/// </summary>
		/// <value>The number pasos.</value>
		public int NumPasos
		{
			get
			{
				return _paso.Count;
			}
		}

		#endregion
	}
}

