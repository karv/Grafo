﻿using System.Collections.Generic;
using Graficas.Nodos;
using Graficas.Edges;
using System;

namespace Graficas.Rutas
{
	/// <summary>
	/// Representa una ruta dinámica según un grafo asociado.
	/// </summary>
	public class HardRuta<T> : IPath<T>
		where T : IEquatable<T>
	{
		List<Node<T>> _pasos { get; set; }

		/// <param name="inicial">Nodo inicial</param>
		public HardRuta (Node<T> inicial)
		{
			_pasos = new List<Node<T>> ();
			_pasos.Add (inicial);
		}

		/// <param name="pasos">Conjunto de pasos</param>
		public HardRuta (IEnumerable<Node<T>> pasos)
		{
			_pasos = new List<Node<T>> (pasos);
		}

		/// <summary>
		/// Construye uan ruta como ésta, en sentido inverso.
		/// </summary>
		public HardRuta<T> Reversa ()
		{
			var ret = new HardRuta<T> (NodoFinal);
			ret._pasos = new List<Node<T>> (_pasos);
			ret._pasos.Reverse ();
			return ret;
		}

		/// <summary>
		/// Devuelve el nodo inicial
		/// </summary>
		/// <value>The nodo inicial.</value>
		public Node<T> NodoInicial
		{
			get
			{
				return _pasos[0];
			}
		}

		/// <summary>
		/// Devuelve el nodo final
		/// </summary>
		/// <value>The nodo final.</value>
		public Node<T> NodoFinal
		{
			get
			{
				return _pasos[_pasos.Count - 1];
			}
		}

		T IPath<T>.NodoInicial
		{
			get
			{
				return NodoInicial.Item;
			}
		}

		T IPath<T>.NodoFinal
		{
			get
			{
				return NodoFinal.Item;
			}
		}

		/// <summary>
		/// Concatena esta ruta con un paso
		/// </summary>
		/// <param name="paso">Paso con qué concatenar</param>
		public void Concat (IStep<T> paso)
		{
			if (NodoFinal.Item.Equals (paso.Origin))
			{
				var agrega = NodoFinal.Neighborhood.Find (x => x.Item.Equals (paso.Destination));
				if (agrega == null)
					throw new Exception ("Paso inexsistente en grafo.");
				_pasos.Add (agrega);
				return;
			}
			throw new Exception ("Nodo final de la ruta no concide con origen del paso.");
		}

		/// <summary>
		/// Concatena esta ruta con otra
		/// </summary>
		/// <param name="ruta">Ruta.</param>
		public void Concat (IPath<T> ruta)
		{
			if (!NodoFinal.Item.Equals (ruta.NodoInicial))
				throw new Exception ("Nodo final de la ruta no concide con origen del paso.");
			foreach (var x in ruta.Pasos)
			{
				Concat (x);
			}
		}

		/// <summary>
		/// Concatena esta ruta con un nodo
		/// </summary>
		/// <param name="nodo">Nodo al final</param>
		public void Concat (T nodo)
		{
			var agrega = NodoFinal.Neighborhood.Find (x => x.Item.Equals (nodo));
			if (agrega == null)
				throw new Exception ("Paso inexsistente en grafo.");
			_pasos.Add (agrega);
		}

		/// <summary>
		/// Devuelve la longitud.
		/// </summary>
		/// <value>The longitud.</value>
		public float Longitud
		{
			get
			{
				return NumPasos;
			}
		}

		/// <summary>
		/// Devuelve el número de pasos
		/// </summary>
		/// <value>The number pasos.</value>
		public int NumPasos
		{
			get
			{
				return _pasos.Count - 1;
			}
		}


		/// <summary>
		/// Enumera los pasos
		/// </summary>
		/// <value>The pasos.</value>
		public IEnumerable<IStep<T>> Pasos
		{
			get
			{
				for (int i = 0; i < NumPasos; i++)
				{
					yield return new Step<T> (_pasos[i].Item, _pasos[i + 1].Item);
				}
			}
		}
	}
}