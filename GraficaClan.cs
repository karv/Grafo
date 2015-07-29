using System;
using Graficas;
using System.Collections.Generic;

namespace Graficas
{
	public class GraficaClan<T>: IGrafica<T>
	{
		/// <summary>
		/// Representa una gráfica modelada como conjunto de sus subgráficas completas maximales
		/// </summary>
		public GraficaClan()
		{
		}

		#region IGrafica implementation

		public System.Collections.Generic.ICollection<T> Vecinos(T nodo)
		{
			throw new NotImplementedException();
		}

		public bool ExisteArista(IArista<T> aris)
		{
			throw new NotImplementedException();
		}

		public Graficas.Rutas.IRuta<T> toRuta(IEnumerable<T> seq)
		{
			throw new NotImplementedException();
		}

		public Graficas.Rutas.IRuta<T> RutaOptima(T x, T y)
		{
			throw new NotImplementedException();
		}

		public System.Collections.Generic.ICollection<T> Nodos
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}

