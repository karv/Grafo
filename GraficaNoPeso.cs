using System;
using System.Collections.Generic;
using Graficas.Rutas;
using ListasExtra;

namespace Graficas
{

	public class GraficaNoPeso<T> : IGrafica<T> where T : IEquatable<T>
	{
		class Nodo
		{
			public T obj;
			public ISet<T> Vecinos = new HashSet<T>();

			public Nodo(T nod)
			{
				obj = nod;
			}
		}

		#region IGrafica

		public IRuta<T> toRuta(IEnumerable<T> seq)
		{
			Rutas.Ruta<T> ret = new Graficas.Rutas.Ruta<T>();
			List<T> lst = new List<T>(seq);
			for (int i = 0; i < lst.Count - 1; i++)
			{
				Rutas.Paso<T> nuevoPaso = new Graficas.Rutas.Paso<T>(lst[i], lst[i + 1], 1);
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
				return nodos.ConvertAll(x => x.obj);
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
				return nodos.Find(x => x.obj.Equals(nodo)).Vecinos;
			}
		}

		public bool ExisteArista(IArista<T> aris)
		{
			if (this.getNodo(aris.desde).Vecinos.Contains(aris.hasta))
				return true;
			return false;
		}

		bool IGrafica<T>.esSimétrico
		{
			get
			{
				return false;
			}
		}

		public void AgregaArista(IArista<T> aris)
		{
			getNodo(aris.desde).Vecinos.Add(aris.hasta);
		}

		public void AgregaNodo(T nodo)
		{
			// Resiva si existe
			if (nodos.Exists(x => x.obj.Equals(nodo)))
				throw new Exception("Nodo ya existente.");
			nodos.Add(new Nodo(nodo));
		}


		#endregion

		#region Internos

		List<Nodo> nodos = new List<Nodo>();

		/// <summary>
		/// Devuelve el nodo que le corresponde a un objeto tipo T.
		/// </summary>
		/// <returns>The nodo.</returns>
		/// <param name="nod">Nod.</param>
		Nodo getNodo(T nod)
		{
			return nodos.Find(x => x.obj.Equals(nod));
		}

		#endregion

		#region ctor

		/// <summary>
		/// Initializes a new instance of the <see cref="Graficas.GraficaNoPeso`1"/> class.
		/// </summary>
		/// <param name="nods">Nodos de la gráfica.</param>
		public GraficaNoPeso(T[] nods)
			: this()
		{
			foreach (var x in nods)
			{
				AgregaNodo(x);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Graficas.GraficaNoPeso`1"/> class.
		/// </summary>
		public GraficaNoPeso()
		{
		}

		#endregion

	}
	
}
