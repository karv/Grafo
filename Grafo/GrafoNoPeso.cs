using System;
using System.Collections.Generic;
using Graficas.Rutas;

namespace Graficas
{

	public class GrafoNoPeso<T> : IGrafo<T> where T : IEquatable<T>
	{
		class Nodo
		{
			public T Obj;
			public ISet<T> Vecinos = new HashSet<T>();

			public Nodo(T nod)
			{
				Obj = nod;
			}
		}

		#region IGrafica

		bool ILecturaGrafo<T>.this [T desde, T hasta]{ get { return ExisteArista(desde, hasta); } }

		bool IGrafo<T>.this [T desde, T hasta]
		{ 
			get
			{ 
				return ExisteArista(desde, hasta); 
			} 
			set
			{
				if (value)
				{
					AgregaArista(desde, hasta);
				}
				else
				{
					getNodo(desde).Vecinos.Remove(hasta);
				}
			}
		}

		ICollection<IArista<T>> ILecturaGrafo<T>.Aristas()
		{
			throw new NotImplementedException();
		}

		public IRuta<T> ToRuta(IEnumerable<T> seq)
		{
			var ret = new Ruta<T>();
			var lst = new List<T>(seq);
			for (int i = 0; i < lst.Count - 1; i++)
			{
				var nuevoPaso = new Paso<T>(lst[i], lst[i + 1], 1);
				ret.Concat(nuevoPaso);
			}
			return ret;
		}

		/// <summary>
		/// Agrega un vértice dado su origen y final.
		/// </summary>
		/// <param name="desde">Origen.</param>
		/// <param name="hasta">Destino.</param>
		public void AgregaVertice(T desde, T hasta)
		{
			getNodo(desde).Vecinos.Add(hasta);
		}

		/// <summary>
		/// Revisa si existe una arista dada
		/// </summary>
		/// <returns><c>true</c>, if arista was existed, <c>false</c> otherwise.</returns>
		/// <param name="desde">Desde.</param>
		/// <param name="hasta">Hasta.</param>
		public bool ExisteArista(T desde, T hasta)
		{
			return this[desde].Contains(hasta);
		}

		/// <summary>
		/// Agrega una arista a la gráfica
		/// </summary>
		/// <param name="desde">Desde.</param>
		/// <param name="hasta">Hasta.</param>
		public void AgregaArista(T desde, T hasta)
		{
			this[desde].Add(hasta);
		}

		/// <summary>
		/// Devuelve la lista de nodos.
		/// </summary>
		/// <returns>Devuelve una copia, no la colección modificable interna</returns>
		public ICollection<T> Nodos
		{
			get
			{
				return nodos.ConvertAll(x => x.Obj);
			}
		}

		/// <summary>
		/// Devuelve la lista de vecinos de un nodo.
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		public ICollection<T> Vecinos(T nodo)
		{
			return nodos.Find(x => x.Equals(nodo)).Vecinos;
		}

		/// <summary>
		/// Devuelve un arreglo con los vecinos de un nodo específico.
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		public ISet<T> this [T nodo]
		{
			get
			{
				return nodos.Find(x => x.Obj.Equals(nodo)).Vecinos;
			}
		}

		public bool ExisteArista(IArista<T> aris)
		{
			return getNodo(aris.Origen).Vecinos.Contains(aris.Destino);
		}

		public void AgregaArista(IArista<T> aris)
		{
			getNodo(aris.Origen).Vecinos.Add(aris.Destino);
		}

		public void AgregaNodo(T nodo)
		{
			// Resiva si existe
			if (nodos.Exists(x => x.Obj.Equals(nodo)))
				throw new Exception("Nodo ya existente.");
			nodos.Add(new Nodo(nodo));
		}


		#endregion

		#region Internos

		readonly List<Nodo> nodos = new List<Nodo>();

		/// <summary>
		/// Devuelve el nodo que le corresponde a un objeto tipo T.
		/// </summary>
		/// <returns>The nodo.</returns>
		/// <param name="nod">Nod.</param>
		Nodo getNodo(T nod)
		{
			return nodos.Find(x => x.Obj.Equals(nod));
		}

		#endregion

		#region ctor

		/// <summary>
		/// Ctor
		/// </summary>
		/// <param name="nods">Nodos de la gráfica.</param>
		public GrafoNoPeso(T[] nods)
			: this()
		{
			foreach (var x in nods)
			{
				AgregaNodo(x);
			}
		}

		public GrafoNoPeso()
		{
		}

		#endregion

	}
	
}
