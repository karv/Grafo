using System;
using System.Collections.Generic;
using Graficas.Rutas;
using Graficas.Aristas;

namespace Graficas.Grafo
{
	/// <summary>
	/// Un grafo cuyas aristas no guardan peso
	/// </summary>
	public class GrafoNoPeso<T> : IGrafo<T>
		where T : IEquatable<T>
	{
		class Nodo
		{
			public T Obj;
			public ISet<T> Vecinos = new HashSet<T> ();

			public Nodo (T nod)
			{
				Obj = nod;
			}
		}

		/// <summary>
		/// Elimina cada nodo y aristas
		/// </summary>
		public void Clear ()
		{
			nodos.Clear ();
		}

		#region IGrafica

		/// <summary>
		/// Calcula el subgrafo generado por un subconjutno de Nodos
		/// </summary>
		/// <param name="conjunto">Conjunto de nodos para calcular el subgrafo</param>
		public GrafoNoPeso<T> Subgrafo (IEnumerable<T> conjunto)
		{
			throw new NotImplementedException ();
		}

		IGrafo<T> IGrafo<T>.Subgrafo (IEnumerable<T> conjunto)
		{
			return Subgrafo (conjunto);
		}

		public AristaBool<T> this [T desde, T hasta]
		{ 
			get
			{ 
				return new AristaBool<T> (desde, hasta, ExisteArista (desde, hasta), true);
			}
		}

		IArista<T> IGrafo<T>.this [T desde, T hasta]
		{ 
			get
			{ 
				return new AristaBool<T> (desde, hasta, ExisteArista (desde, hasta), true);
			} 
		}

		ICollection<IArista<T>> IGrafo<T>.Aristas ()
		{
			throw new NotImplementedException ();
		}

		/// <summary>
		/// Revisa si existe una arista dado los extremos
		/// </summary>
		/// <returns><c>true</c>, si existe una arista, <c>false</c> otherwise.</returns>
		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		public bool ContieneArista (T origen, T destino)
		{
			return getNodo (origen).Vecinos.Contains (destino);
		}

		/// <summary>
		/// Convierte una sucesión consistente de nodos a una ruta
		/// </summary>
		/// <returns>The ruta.</returns>
		/// <param name="seq">Sucesión consistente.</param>
		public IRuta<T> ToRuta (IEnumerable<T> seq)
		{
			var ret = new Ruta<T> ();
			var lst = new List<T> (seq);
			for (int i = 0; i < lst.Count - 1; i++)
				ret.Concat (this [lst [i], lst [i + 1]]);
			return ret;
		}

		/// <summary>
		/// Agrega un vértice dado su origen y final.
		/// </summary>
		/// <param name="desde">Origen.</param>
		/// <param name="hasta">Destino.</param>
		public void AgregaVertice (T desde, T hasta)
		{
			getNodo (desde).Vecinos.Add (hasta);
		}

		/// <summary>
		/// Revisa si existe una arista dada
		/// </summary>
		/// <returns><c>true</c>, if arista was existed, <c>false</c> otherwise.</returns>
		/// <param name="desde">Desde.</param>
		/// <param name="hasta">Hasta.</param>
		public bool ExisteArista (T desde, T hasta)
		{
			return this [desde].Contains (hasta);
		}

		/// <summary>
		/// Agrega una arista a la gráfica
		/// </summary>
		/// <param name="desde">Desde.</param>
		/// <param name="hasta">Hasta.</param>
		public void AgregaArista (T desde, T hasta)
		{
			this [desde].Add (hasta);
		}

		/// <summary>
		/// Devuelve la lista de nodos.
		/// </summary>
		/// <returns>Devuelve una copia, no la colección modificable interna</returns>
		public ICollection<T> Nodos
		{
			get
			{
				return nodos.ConvertAll (x => x.Obj);
			}
		}

		/// <summary>
		/// Devuelve la lista de vecinos de un nodo.
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		public ICollection<T> Vecinos (T nodo)
		{
			return nodos.Find (x => x.Equals (nodo)).Vecinos;
		}

		/// <summary>
		/// Devuelve un arreglo con los vecinos de un nodo específico.
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		public ISet<T> this [T nodo]
		{
			get
			{
				return nodos.Find (x => x.Obj.Equals (nodo)).Vecinos;
			}
		}

		/// <summary>
		/// Revisa si existe una arista compatible a otra dada
		/// </summary>
		/// <param name="aris">Arista.</param>
		public bool ExisteArista (IArista<T> aris)
		{
			return getNodo (aris.Origen).Vecinos.Contains (aris.Destino);
		}

		/// <summary>
		/// Agrega una arista
		/// </summary>
		public void AgregaArista (IArista<T> aris)
		{
			getNodo (aris.Origen).Vecinos.Add (aris.Destino);
		}

		/// <summary>
		/// Agrega un nodo
		/// </summary>
		public void AgregaNodo (T nodo)
		{
			// Resiva si existe
			if (nodos.Exists (x => x.Obj.Equals (nodo)))
				throw new System.Exception ("Nodo ya existente.");
			nodos.Add (new Nodo (nodo));
		}

		#endregion

		#region Internos

		readonly List<Nodo> nodos = new List<Nodo> ();

		/// <summary>
		/// Devuelve el nodo que le corresponde a un objeto tipo T.
		/// </summary>
		/// <returns>The nodo.</returns>
		/// <param name="nod">Nod.</param>
		Nodo getNodo (T nod)
		{
			return nodos.Find (x => x.Obj.Equals (nod));
		}

		#endregion

		#region ctor

		/// <param name="nods">Nodos de la gráfica.</param>
		public GrafoNoPeso (T [] nods)
			: this ()
		{
			foreach (var x in nods)
			{
				AgregaNodo (x);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public GrafoNoPeso ()
		{
		}

		#endregion

	}
	
}