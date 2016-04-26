using System;
using System.Collections.Generic;
using Graficas.Rutas;
using Graficas.Aristas;

namespace Graficas.Grafo
{
	/// <summary>
	/// Representa un grafo como una colección de aristas
	/// </summary>
	public class GrafoAristas<T> : IGrafoPeso<T>
		where T:IEquatable<T>
	{
		public GrafoAristas ()
		{
			AristasColección = new List<IArista<T>> ();
		}

		/// <summary>
		/// Devuelve los nodos que son apuntados por un nodo dado.
		/// </summary>
		/// <returns>The exterior.</returns>
		public ICollection<T> VecindadExterior (T nodo)
		{
			if (!Nodos.Contains (nodo))
				throw new InvalidOperationException (string.Format (
					"Nodo {0} no pertenece a gráfica",
					nodo));

			var ret = new HashSet<T> ();
			foreach (var a in AristasColección)
				if (a.Origen.Equals (nodo))
					ret.Add (a.Destino);

			return ret;
		}

		/// <summary>
		/// Devuelve los nodos que apuntan a un nodo dado.
		/// </summary>
		/// <returns>The interior.</returns>
		public ICollection<T> VecindadInterior (T nodo)
		{
			if (!Nodos.Contains (nodo))
				throw new InvalidOperationException (string.Format (
					"Nodo {0} no pertenece a gráfica",
					nodo));

			var ret = new HashSet<T> ();
			foreach (var a in AristasColección)
				if (a.Destino.Equals (nodo))
					ret.Add (a.Origen);

			return ret;
		}

		/// <summary>
		/// La colección de aristas
		/// </summary>
		protected ICollection<IArista<T>> AristasColección;

		#region Grafo

		/// <summary>
		/// Clear this instance.
		/// </summary>
		public void Clear ()
		{
			AristasColección.Clear ();
		}

		/// <summary>
		/// Colección de aristas
		/// </summary>
		public ICollection<IArista<T>> Aristas ()
		{
			return AristasColección;
		}

		/// <summary>
		/// Colección de vecinos de un nodo
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		public ICollection<T> Vecinos (T nodo)
		{
			if (!Nodos.Contains (nodo))
				throw new InvalidOperationException (string.Format (
					"Nodo {0} no pertenece a gráfica",
					nodo));
			
			var ret = new HashSet<T> ();
			foreach (var x in AristasColección)
			{
				if (x.Origen.Equals (nodo))
					ret.Add (x.Destino);
				if (x.Destino.Equals (nodo))
					ret.Add (x.Origen);
			}
			return ret;
		}

		/// <summary>
		/// Convierte una sucesión consistente de nodos a una ruta
		/// </summary>
		/// <returns>The ruta.</returns>
		/// <param name="seq">Sucesión consistente.</param>
		public IRuta<T> ToRuta (IEnumerable<T> seq)
		{
			throw new NotImplementedException ();
		}

		/// <summary>
		/// Calcula el subgrafo generado por un subconjutno de Nodos
		/// </summary>
		/// <param name="conjunto">Conjunto de nodos para calcular el subgrafo</param>
		public ILecturaGrafo<T> Subgrafo (IEnumerable<T> conjunto)
		{
			var col = new HashSet<T> (conjunto);
			var ret = new Grafo<T> ();

			foreach (var u in AristasColección)
			{
				if (col.Contains (u.Origen) || col.Contains (u.Destino))
					ret.AgregaArista (u);
			}
			return ret;
		}

		bool ILecturaGrafo<T>.this [T desde, T hasta]
		{
			get
			{
				return ExisteArista (desde, hasta);
			}
		}

		float IGrafoPeso<T>.this [T desde, T hasta]
		{
			get
			{ 
				IArista<T> art;
				return TryGetArista (desde, hasta, out art) ? art.Peso : 0;
			}
			set
			{
				IArista<T> art;
				if (TryGetArista (desde, hasta, out art))
					AristasColección.Remove (art);
				AgregaArista (new Arista<T> (desde, hasta, value));
			}
		}

		float ILecturaGrafoPeso<T>.this [T desde, T hasta]
		{
			get
			{ 
				IArista<T> art;
				return TryGetArista (desde, hasta, out art) ? art.Peso : 0;
			}
		}

		/// <summary>
		/// Nodos
		/// </summary>
		public ICollection<T> Nodos
		{
			get
			{
				var ret = new HashSet<T> ();
				foreach (var a in AristasColección)
				{
					ret.Add (a.Origen);
					ret.Add (a.Destino);
				}
				return ret;
			}
		}

		#endregion

		/// <summary>
		/// Agrega una arista
		/// </summary>
		/// <param name="art">Art.</param>
		public void AgregaArista (IArista<T> art)
		{
			AristasColección.Add (art);
		}

		/// <summary>
		/// Devuelve la arista entre dos puntos
		/// </summary>
		/// <returns>The arista.</returns>
		/// <param name="desde">Desde.</param>
		/// <param name="hasta">Hasta.</param>
		public IArista<T> GetArista (T desde, T hasta)
		{
			foreach (var x in AristasColección)
			{
				if (x.Origen.Equals (desde) && x.Destino.Equals (hasta))
					return x;
			}
			throw new AristaInexistenteException<T> (desde, hasta, this);
		}

		/// <summary>
		/// Existe una arista con extremos dados
		/// </summary>
		/// <returns><c>true</c>, if arista was existed, <c>false</c> otherwise.</returns>
		/// <param name="desde">Desde.</param>
		/// <param name="hasta">Hasta.</param>
		public bool ExisteArista (T desde, T hasta)
		{
			foreach (var x in AristasColección)
			{
				if (x.Origen.Equals (desde) && x.Destino.Equals (hasta))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Intenta devolver una arista con extremos dados
		/// </summary>
		/// <returns><c>true</c>, si existe una arista con tales extremos, <c>false</c> otherwise.</returns>
		/// <param name="desde">Origen</param>
		/// <param name="hasta">Destino</param>
		/// <param name="art">Arista encontrada</param>
		public bool TryGetArista (T desde, T hasta, out IArista<T> art)
		{
			foreach (var x in AristasColección)
			{
				if (x.Origen.Equals (desde) && x.Destino.Equals (hasta))
				{
					art = x;
					return true;
				}
			}
			art = null;
			return false;
		}
	}


	[Serializable]
	public class AristaInexistenteException<T> : System.Exception
	{
		public T Origen;
		public T Destino;
		public ILecturaGrafo<T> Grafo;

		public AristaInexistenteException ()
		{
		}

		public AristaInexistenteException (T origen,
		                                   T destino,
		                                   ILecturaGrafo<T> grafo)
		{
			Origen = origen;
			Destino = destino;
			Grafo = grafo;
		}

		public AristaInexistenteException (string message)
			: base (message)
		{
		}

		public AristaInexistenteException (string message, System.Exception inner)
			: base (message,
			        inner)
		{
		}

		protected AristaInexistenteException (System.Runtime.Serialization.SerializationInfo info,
		                                      System.Runtime.Serialization.StreamingContext context)
			: base (info,
			        context)
		{
		}
	}
}