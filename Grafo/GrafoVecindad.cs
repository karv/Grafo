using System.Collections.Generic;
using Graficas.Aristas;
using System;
using System.Linq;

namespace Graficas.Grafo
{
	public class GrafoVecindad<T> : IGrafo<T> // TEST all
	{
		#region Ctor

		public GrafoVecindad (IEnumerable<T> nodos, bool simétrico = false,
		                      IEqualityComparer<T> comparador = null)
		{
			Comparador = comparador ?? EqualityComparer<T>.Default;
			Simétrico = simétrico;
			this.nodos = new HashSet<T> (nodos, Comparador);
			Vecindad = new Dictionary<T, HashSet<T>> (Comparador);
			inicializaDiccionario ();
		}

		void inicializaDiccionario ()
		{
			foreach (var x in nodos)
				Vecindad.Add (x, new HashSet<T> (Comparador));
		}

		#endregion

		#region Interno

		HashSet<T> nodos { get; }

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
			catch (KeyNotFoundException ex)
			{
				var m = string.Format (
					        "No se puede calcular vecindad de {0}. ¿Es un nodo de esta clase?",
					        nodo);
				throw new NodoInexistenteException (m, ex);
			}
		}

		public Graficas.Rutas.IRuta<T> ToRuta (IEnumerable<T> seq)
		{
			throw new NotImplementedException ();
		}

		public GrafoVecindad<T> Subgrafo (IEnumerable<T> conjunto)
		{
			try
			{
				var ret = new GrafoVecindad<T> (conjunto, Simétrico, Comparador);
				foreach (var c in conjunto)
					ret.Vecindad [c].UnionWith (Vecindad [c].Intersect (conjunto));
				return ret;
			}
			catch (Exception ex)
			{
				var m = string.Format ("No se puede calcular el subgrafo de esta clase respecto a {0}.\n" +
				        "¿Es el argumento un subconjunto de Nodos?", conjunto);
				throw new OperaciónInválidaGrafosException (m, ex);
			}
		}

		IGrafo<T> IGrafo<T>.Subgrafo (IEnumerable<T> conjunto)
		{
			return Subgrafo (conjunto);
		}

		public bool this [T desde, T hasta]
		{
			get
			{
				bool ret = Vecindad [desde].Contains (hasta);
				return ret;
			}
			set
			{
				establecerArista (desde, hasta, value);
				if (Simétrico)
					establecerArista (hasta, desde, value);
			}
		}

		void establecerArista (T desde, T hasta, bool valor)
		{
			if (valor)
				AgregaArista (desde, hasta);
			else
				EliminaArista (desde, hasta);
		}

		protected void EliminaArista (T desde, T hasta)
		{
			var vec = Vecindad [desde];
			vec.Remove (hasta);
		}

		protected void AgregaArista (T desde, T hasta)
		{
			var vec = Vecindad [desde];
			vec.Add (hasta);
		}

		public AristaBool<T> Arista (T desde, T hasta)
		{
			bool ret = Vecindad [desde].Contains (hasta);
			return new AristaBool<T> (desde, hasta, ret, true, Simétrico);
		}

		IArista<T> IGrafo<T>.this [T desde, T hasta]
		{
			get
			{
				return Arista (desde, hasta);
			}
		}

		public ICollection<T> Nodos
		{
			get
			{
				return new HashSet<T> (nodos, Comparador);
			}
		}

		#endregion
	}
}