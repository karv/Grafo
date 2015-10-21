using System;
using System.Collections.Generic;
using Graficas;

namespace Graficas
{
	/// <summary>
	/// Representa una gráfica modelada como conjunto de sus subgráficas completas maximales
	/// </summary>
	public class GrafoClan<T>: IGrafoRutas<T>
		where T:IEquatable<T>
	{
		class Clan : HashSet<T>
		{
			public Clan ()
			{
			}

			public Clan (ISet<T> elementos)
				: base (elementos)
			{
			}
		}

		/// <summary>
		/// Calcula el subgrafo generado por un subconjutno de Nodos
		/// </summary>
		/// <param name="conjunto">Conjunto de nodos para calcular el subgrafo</param>
		public ILecturaGrafo<T> Subgrafo (IEnumerable<T> conjunto)
		{
			throw new NotImplementedException ();
		}

		ICollection<T> _nodos = new HashSet<T> ();

		readonly ISet<Clan> clanes = new HashSet<Clan> ();

		#region Interno Técnico

		/// <summary>
		/// Revisa si un conjunto de nodos es completo.
		/// </summary>
		/// <returns><c>true</c>, if completo was esed, <c>false</c> otherwise.</returns>
		/// <param name="conj">Conj.</param>
		bool EsCompleto (ISet<T> conj)
		{
			var H = GetPares (conj);

			foreach (var clan in clanes)
			{
				/*
				foreach (var r in getPares(clan))
				{
					if (H.Contains(r))
						H.Remove(r);
				}
*/
				H.ExceptWith (GetPares (clan));
			}
			return H.Count == 0;
		}

		/// <summary>
		/// Devuelve la lista de parejas [conj]^2
		/// </summary>
		/// <returns>The pares.</returns>
		/// <param name="conj">Conj.</param>
		static ISet<ListasExtra.ParNoOrdenado<T>> GetPares (ICollection<T> conj)
		{
			var arr = new T[conj.Count];
			ISet<ListasExtra.ParNoOrdenado<T>> ret = new HashSet<ListasExtra.ParNoOrdenado<T>> ();
			conj.CopyTo (arr, 0);
			for (int i = 0; i < conj.Count; i++)
			{
				for (int j = i + 1; j < conj.Count; j++)
				{
					ret.Add (new ListasExtra.ParNoOrdenado<T> (arr [i], arr [j]));
				}
			}
			return ret;
		}

		#endregion

		#region IGrafica implementation

		bool ILecturaGrafo<T>.this [T desde, T hasta]
		{
			get
			{
				return ExisteArista (desde, hasta);
			}
		}

		ICollection<IArista<T>> ILecturaGrafo<T>.Aristas ()
		{
			throw new NotImplementedException ();
		}

		public bool ExisteArista (T desde, T hasta)
		{
			return ExisteArista (new Arista<T> (desde, hasta, 1));
		}

		/// <summary>
		/// Agrega una arista
		/// </summary>
		/// <param name="desde">Desde.</param>
		/// <param name="hasta">Hasta.</param>
		public void AgregaArista (T desde, T hasta)
		{
			var NuevoClan = new Clan ();
			clanes.Add (NuevoClan);
			Clan tempo;
			_nodos.Add (desde);
			_nodos.Add (hasta);
			NuevoClan.Add (desde);
			NuevoClan.Add (hasta);
			foreach (var z in Nodos)
			{
				if (!NuevoClan.Contains (z))
				{
					tempo = new Clan (NuevoClan);
					tempo.Add (z);
					if (EsCompleto (tempo))
						NuevoClan.Add (z);
				}
			}
			// Eliminar todos los clanes ya no maximales
			var clone = new HashSet<Clan> (clanes);
			clone.Remove (NuevoClan);
			foreach (var c in clone)
			{
				if (NuevoClan.IsSupersetOf (c))
					clanes.Remove (c);
			}
		}

		public ICollection<T> Vecinos (T nodo)
		{
			var ret = new HashSet<T> ();
			foreach (var c in clanes)
			{
				if (c.Contains (nodo))
					ret.UnionWith (c);
			}
			return ret;
		}

		public bool ExisteArista (IArista<T> aris)
		{
			ISet<T> ar = new HashSet<T> ();
			ar.Add (aris.Origen);
			ar.Add (aris.Destino);
			foreach (var c in clanes)
			{
				if (c.IsSupersetOf (ar))
					return true;
			}
			return false;
		}

		public Graficas.Rutas.IRuta<T> ToRuta (IEnumerable<T> seq)
		{
			Rutas.Ruta<T> ret = new Graficas.Rutas.Ruta<T> ();
			var lst = new List<T> (seq);
			for (int i = 0; i < lst.Count - 1; i++)
			{
				Rutas.Paso<T> nuevoPaso = new Graficas.Rutas.Paso<T> (lst [i], lst [i + 1], 1);
				ret.Concat (nuevoPaso);
			}
			return ret;
		}

		public Graficas.Rutas.IRuta<T> RutaÓptima (T x, T y)
		{
			throw new NotImplementedException ();
		}

		public ICollection<T> Nodos
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
		public bool EsCompleto (IEnumerable<T> nods)
		{
			foreach (var x in clanes)
			{
				if (x.IsSupersetOf (nods))
					return true;
			}
			return false;
		}
	}
}

