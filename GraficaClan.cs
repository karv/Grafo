using System;
using System.Collections.Generic;
using Graficas;
using Graficas.Misc;

namespace Graficas
{
	/// <summary>
	/// Representa una gráfica modelada como conjunto de sus subgráficas completas maximales
	/// </summary>
	public class GraficaClan<T>: IGrafica<T>
	{
		class Clan : HashSet<T>
		{
			public Clan() : base()
			{
			}

			public Clan(ISet<T> elementos) : base(elementos)
			{
			}
		}

		ICollection<T> _nodos = new HashSet<T>();

		ISet<Clan> clanes = new HashSet<Clan>();

		public GraficaClan()
		{
		}

		#region Interno Técnico

		/// <summary>
		/// Revisa si un conjunto de nodos es completo.
		/// </summary>
		/// <returns><c>true</c>, if completo was esed, <c>false</c> otherwise.</returns>
		/// <param name="conj">Conj.</param>
		bool EsCompleto(ISet<T> conj)
		{
			ISet<Graficas.Misc.ParNoOrdenado<T>> H = getPares(conj);

			foreach (var clan in clanes)
			{
				/*
				foreach (var r in getPares(clan))
				{
					if (H.Contains(r))
						H.Remove(r);
				}
*/
				H.ExceptWith(getPares(clan));
			}
			return H.Count == 0;
		}

		/// <summary>
		/// Devuelve la lista de parejas [conj]^2
		/// </summary>
		/// <returns>The pares.</returns>
		/// <param name="conj">Conj.</param>
		ISet<Graficas.Misc.ParNoOrdenado<T>> getPares(ICollection<T> conj)
		{
			T[] arr = new T[conj.Count];
			ISet<Graficas.Misc.ParNoOrdenado<T>> ret = new HashSet<Graficas.Misc.ParNoOrdenado<T>>(new ParNoOrdenado<T>.comparer());
			conj.CopyTo(arr, 0);
			for (int i = 0; i < conj.Count; i++)
			{
				for (int j = i + 1; j < conj.Count; j++)
				{
					ret.Add(new Graficas.Misc.ParNoOrdenado<T>(arr[i], arr[j]));
				}
			}
			return ret;
		}

		#endregion




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
			Clan NuevoClan = new Clan();
			clanes.Add(NuevoClan);
			Clan tempo;
			_nodos.Add(aris.desde);
			_nodos.Add(aris.hasta);
			NuevoClan.Add(aris.desde);
			NuevoClan.Add(aris.hasta);
			foreach (var z in Nodos)
			{
				if (!NuevoClan.Contains(z))
				{
					tempo = new Clan(NuevoClan);
					tempo.Add(z);
					if (EsCompleto(tempo))
						NuevoClan.Add(z);
				}
			}
			// Eliminar todos los clanes ya no maximales
			HashSet<Clan> clone = new HashSet<Clan>(clanes);
			clone.Remove(NuevoClan);
			foreach (var c in clone)
			{
				if (NuevoClan.IsSupersetOf(c))
					clanes.Remove(c);
			}
		}

		public System.Collections.Generic.ICollection<T> Vecinos(T nodo)
		{
			HashSet<T> ret = new HashSet<T>();
			foreach (var c in clanes)
			{
				if (c.Contains(nodo))
					ret.UnionWith(c);
			}
			return ret;
		}

		public bool ExisteArista(IArista<T> aris)
		{
			ISet<T> ar = new HashSet<T>();
			ar.Add(aris.desde);
			ar.Add(aris.hasta);
			foreach (var c in clanes)
			{
				if (c.IsSupersetOf(ar))
					return true;
			}
			return false;
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
				return _nodos;
			}
		}

		#endregion

		/// <summary>
		/// Revisa si un conjunto de nodos es completo en la gráfica
		/// </summary>
		/// <returns><c>true</c>, if completo was esed, <c>false</c> otherwise.</returns>
		/// <param name="nods">Nods.</param>
		public bool esCompleto(IEnumerable<T> nods)
		{
			foreach (var x in clanes)
			{
				if (x.IsSupersetOf(nods))
					return true;
			}
			return false;
		}
	}
}

