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
			public List<T> Vecinos = new List<T>();

			public Nodo(T nod)
			{
				obj = nod;
			}
		}

		/// <summary>
		/// Devuelve la lista de nodos.
		/// </summary>
		/// <returns>The nodos.</returns>
		public T[] getNodos()
		{
			int num = nodos.Count;
			T[] ret = new T[num];
			for (int i = 0; i < num; i++)
			{
				ret[i] = nodos[i].obj;
			}
			return ret;
		}

		List<Nodo> nodos = new List<Nodo>();

		ICollection<T> IGrafica<T>.Nodos
		{
			get
			{
				return getNodos();
			}
		}

		/// <summary>
		/// Devuelve la lista de vecinos de un nodo.
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		public ICollection<T> Vecinos(T nodo)
		{
			return nodos.Find(x => x.Equals(nodo)).Vecinos.ToArray();
		}

		/// <summary>
		/// Devuelve un arreglo con los vecinos de un nodo específico.
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		public T[] this [T nodo]
		{
			get
			{
				return nodos.Find(x => x.obj.Equals(nodo)).Vecinos.ToArray();
			}
		}

		public IRuta<T> RutaOptima(T x, T y)
		{
			throw new NotImplementedException();
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
				throw new NotImplementedException();
			}
		}

		public void AgregaArista(IArista<T> aris)
		{
			throw new NotImplementedException();
		}

		public void AgregaNodo(T nodo)
		{
			// Resiva si existe
			if (nodos.Exists(x => x.obj.Equals(nodo)))
				throw new Exception("Nodo ya existente.");
			nodos.Add(new Nodo(nodo));
		}

		/// <summary>
		/// Devuelve el nodo que le corresponde a un objeto tipo T.
		/// </summary>
		/// <returns>The nodo.</returns>
		/// <param name="nod">Nod.</param>
		Nodo getNodo(T nod)
		{
			return nodos.Find(x => x.obj.Equals(nod));
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

		public IRuta<T> toRuta(IEnumerable<T> seq)
		{
			throw new NotImplementedException();
		}

	}
	
}
