using System.Collections.Generic;
using Graficas.Aristas;
using System;
using System.Linq;

namespace Graficas.Grafo
{
	public class GrafoVecindad<T> : IGrafo<T> // TEST all
	{
		#region Ctor

		public GrafoVecindad (bool simétrico = false,
		                      IEqualityComparer<T> comparador = null)
		{
			Comparador = comparador ?? EqualityComparer<T>.Default;
			Simétrico = simétrico;
			Nodos = new HashSet<T> (Comparador);
			Vecindad = new Dictionary<T, HashSet<T>> (Comparador);
			inicializaDiccionario ();
		}

		void inicializaDiccionario ()
		{
			foreach (var x in Nodos)
				Vecindad.Add (x, new HashSet<T> (Comparador));
		}

		#endregion

		#region Interno

		HashSet<T> Nodos { get; }

		public IEqualityComparer<T> Comparador { get; }

		protected Dictionary<T, HashSet<T>> Vecindad { get; }

		public bool Simétrico { get; }

		#endregion

		#region Grafo

		/// <summary>
		/// Elimina aristas.
		/// </summary>
		public void Clear ()
		{
			foreach (var x in Vecindad)
				x.Value.Clear ();
		}

		/// <summary>
		/// Colección de aristas existentes.
		/// </summary>
		/// <remarks>No preserva referencia </remarks>
		public ICollection<IArista<T>> Aristas ()
		{
			var ret = new HashSet<IArista<T>> ();
			foreach (var x in Vecindad)
				foreach (var y in x.Value)
					ret.Add (new AristaBool<T> (x.Key, y, true, true, Simétrico));
			return ret;
		}

		public ICollection<T> Vecinos (T nodo)
		{
			try
			{
				return new HashSet<T> (Vecindad [nodo], Comparador);
			}
			catch (Exception ex)
			{
				var m = string.Format (
					        "No se puede calcular vecindad de {0}. ¿Es un nodo de esta clase?",
					        nodo);
				throw new Exception (m, ex);
			}
		}

		public Graficas.Rutas.IRuta<T> ToRuta (IEnumerable<T> seq)
		{
			throw new NotImplementedException ();
		}

		public IGrafo<T> Subgrafo (IEnumerable<T> conjunto)
		{
			try
			{
				var ret = new GrafoVecindad<T> (Comparador);
				foreach (var c in conjunto)
					ret.Vecindad [c].UnionWith (Vecindad [c].Intersect (conjunto));
				return ret;
			}
			catch (Exception ex)
			{
				var m = string.Format ("No se puede calcular el subgrafo de esta clase respecto a {0}.\n" +
				        "¿Es el argumento un subconjunto de Nodos?", conjunto);
				throw new Exception (m, ex);
			}
		}

		public IArista<T> this [T desde, T hasta]
		{
			get
			{
				throw new NotImplementedException ();
			}
		}

		ICollection<T> IGrafo<T>.Nodos
		{
			get
			{
				throw new NotImplementedException ();
			}
		}

		#endregion
	}
}