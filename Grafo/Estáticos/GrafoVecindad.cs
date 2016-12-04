using System;
using System.Collections.Generic;
using System.Linq;
using Graficas.Aristas;
using Graficas.Rutas;

namespace Graficas.Grafo.Estáticos
{
	/// <summary>
	/// Representa un grafo visto como una función que a cada nodo le asigna su vecindad.
	/// </summary>
	public class GrafoVecindad<T> : IGrafo<T>
	{
		#region Ctor

		/// <param name="nodos">Colección de nodos del grafo</param>
		/// <param name="simétrico">Si el grafo es simétrico</param>
		/// <param name="comparador">Comparador</param>
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

		/// <summary>
		/// Devuelve el comparador que se usa para los nodos
		/// </summary>
		/// <value>The comparador.</value>
		public IEqualityComparer<T> Comparador { get; }

		/// <summary>
		/// El diccionario que asigna a cada nodo su vecindad
		/// </summary>
		/// <value>The vecindad.</value>
		protected Dictionary<T, HashSet<T>> Vecindad { get; }

		/// <summary>
		/// Devuelve un <c>bool</c> que indica si este grafo es tratato como simétrico
		/// </summary>
		/// <value><c>true</c> if simétrico; otherwise, <c>false</c>.</value>
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

		/// <summary>
		/// Devuelve una copia de la vecindad de un nodo dado.
		/// </summary>
		/// <param name="nodo">Nodoa a considedad su vecindad</param>
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

		/// <summary>
		/// Convierte una sucesión consistente de nodos a una ruta
		/// </summary>
		/// <returns>The ruta.</returns>
		/// <param name="seq">Sucesión consistente.</param>
		public IRuta<T> ToRuta (IEnumerable<T> seq)
		{
			var ret = new Ruta<T> ();
			bool iniciando = true; // Flag que indica que está construyendo el primer nodo (no paso)
			T last = default(T);
			foreach (var x in seq)
			{
				if (iniciando)
				{
					iniciando = false;
					ret = new Ruta<T> ();
				}
				else
				{
					if (!Vecinos (last).Contains (x))
						throw new RutaInconsistenteException ("La sucesión dada no representa una ruta.");
					ret.Concat (new Paso<T> (last, x));
				}
				last = x;
			}
			return ret;
		}

		/// <summary>
		/// Calcula el subgrafo generado por un subconjutno de Nodos
		/// </summary>
		/// <param name="conjunto">Conjunto de nodos para calcular el subgrafo</param>
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
				throw new ArgumentException (m, "conjunto", ex);
			}
		}

		IGrafo<T> IGrafo<T>.Subgrafo (IEnumerable<T> conjunto)
		{
			return Subgrafo (conjunto);
		}

		/// <summary>
		/// El valor de existencia de la arista correspondiente a un par de puntos
		/// </summary>
		/// <param name="desde">Origen</param>
		/// <param name="hasta">Destino</param>
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

		/// <summary>
		/// Elimina una arista
		/// </summary>
		/// <param name="desde">Desde.</param>
		/// <param name="hasta">Hasta.</param>
		protected void EliminaArista (T desde, T hasta)
		{
			var vec = Vecindad [desde];
			vec.Remove (hasta);
		}

		/// <summary>
		/// Agrega una arista
		/// </summary>
		/// <param name="desde">Desde.</param>
		/// <param name="hasta">Hasta.</param>
		protected void AgregaArista (T desde, T hasta)
		{
			var vec = Vecindad [desde];
			vec.Add (hasta);
		}

		/// <summary>
		/// Devuelve una arista que representa al estado de la arista en el grafo
		/// </summary>
		/// <param name="desde">Desde.</param>
		/// <param name="hasta">Hasta.</param>
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

		/// <summary>
		/// Devuelve una colección sólo lectura de sus nodos
		/// </summary>
		/// <value>The nodos.</value>
		public ICollection<T> Nodos
		{
			get
			{
				return new List<T> (nodos).AsReadOnly ();
			}
		}

		#endregion
	}
}