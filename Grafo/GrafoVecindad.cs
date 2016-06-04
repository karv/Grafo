using System.Collections.Generic;
using Graficas.Aristas;

namespace Graficas.Grafo
{
	public class GrafoVecindad<T> : IGrafo<T>
	{
		#region Ctor

		public GrafoVecindad (IEqualityComparer<T> comparador = null)
		{
			Comparador = comparador ?? EqualityComparer<T>.Default;
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
			throw new System.NotImplementedException ();
		}

		public Graficas.Rutas.IRuta<T> ToRuta (IEnumerable<T> seq)
		{
			throw new System.NotImplementedException ();
		}

		public IGrafo<T> Subgrafo (IEnumerable<T> conjunto)
		{
			throw new System.NotImplementedException ();
		}

		public IArista<T> this [T desde, T hasta]
		{
			get
			{
				throw new System.NotImplementedException ();
			}
		}

		ICollection<T> IGrafo<T>.Nodos
		{
			get
			{
				throw new System.NotImplementedException ();
			}
		}

		#endregion
	}
}