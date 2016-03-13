using System;
using Graficas;
using System.Collections.Generic;
using Graficas.Rutas;

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

		protected ICollection<IArista<T>> AristasColección;

		#region Grafo

		public void Clear ()
		{
			AristasColección.Clear ();
		}

		public ICollection<IArista<T>> Aristas ()
		{
			return AristasColección;
		}

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

		public IRuta<T> ToRuta (IEnumerable<T> seq)
		{
			throw new NotImplementedException ();
		}

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

		public void AgregaArista (IArista<T> art)
		{
			AristasColección.Add (art);
		}

		public IArista<T> GetArista (T desde, T hasta)
		{
			foreach (var x in AristasColección)
			{
				if (x.Origen.Equals (desde) && x.Destino.Equals (hasta))
					return x;
			}
			throw new AristaInexistenteException<T> (desde, hasta, this);
		}

		public bool ExisteArista (T desde, T hasta)
		{
			foreach (var x in AristasColección)
			{
				if (x.Origen.Equals (desde) && x.Destino.Equals (hasta))
					return true;
			}
			return false;
		}

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
	public class AristaInexistenteException<T> : Exception
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

		public AristaInexistenteException (string message, Exception inner)
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