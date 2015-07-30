using System;
using Graficas;
using System.Collections.Generic;

namespace Graficas
{
	/// <summary>
	/// Representa una gráfica modelada como conjunto de sus subgráficas completas maximales
	/// </summary>
	public class GraficaClan<T>: IGrafica<T>
	{
		class Clan : HashSet<T>
		{
			
		}

		ISet<Clan> clanes = new HashSet<Clan>();

		public GraficaClan()
		{
		}

		#region IGrafica implementation

		/// <summary>
		/// Agrega una arista
		/// </summary>
		/// <param name="desde">Desde.</param>
		/// <param name="hasta">Hasta.</param>
		public void AgregaArista(T desde, T hasta)
		{
			((IGrafica<T>)this).AgregaArista(new Arista<T>(desde, hasta));
		}

		void IGrafica<T>.AgregaArista(IArista<T> aris)
		{
			throw new NotImplementedException();
		}

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

