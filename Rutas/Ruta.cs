using System;
using System.Collections.Generic;
using Graficas.Aristas;

namespace Graficas.Rutas
{
	/// <summary>
	/// Representa una ruta
	/// </summary>
	[Serializable]
	public class Ruta<T> : IRuta<T>
		where T : IEquatable<T>
	{
		/// <summary>
		/// 
		/// </summary>
		public Ruta ()
		{
		}

		/// <param name="origen">Origen</param>
		public Ruta (T origen)
		{
			throw new NotImplementedException ();
		}

		/// <summary>
		/// Construye una implementación de esta ruta, dada una ruta abstracta
		/// </summary>
		/// <param name="ruta">Ruta a imitar</param>
		public Ruta (IRuta<T> ruta)
			: this (ruta.NodoInicial)
		{
			foreach (var x in ruta.Pasos)
				Paso.Add (x);
		}

		/// <param name="aris">Arista inicial</param>
		public Ruta (IArista<T> aris)
		{
			Paso.Add (aris);
		}

		/// <summary>
		/// Lista de pasos de esta ruta.
		/// </summary>
		readonly protected IList<IArista<T>> Paso = new List<IArista<T>> ();

		/// <summary>
		/// 
		/// </summary>
		public override string ToString ()
		{
			string ret = string.Format ("[{0}]: ", NumPasos);
			foreach (var x in Paso)
			{
				ret += string.Format (" {0} ", x);

			}
			return ret;
		}

		/// <summary>
		/// Enumera los pasos de la ruta
		/// </summary>
		public IEnumerable<IArista<T>> Pasos
		{ 
			get
			{ 
				return new List<IArista<T>> (Paso);
			} 
		}

		/// <summary>
		/// Concatena con un paso una ruta
		/// </summary>
		/// <param name="paso">Paso con qué concatenar</param>
		public void Concat (IArista<T> paso)
		{
			if (paso == null)
				throw new NullReferenceException ("No se puede concatenar con una arista nula.");
			if (NumPasos == 0)
			{
				Paso.Add (paso);
			}
			else
			{
				if (NodoFinal.Equals (paso.Origen))
				{
					Paso.Add (paso);
				}
				else
				{
					throw new RutaInconsistenteException ("El nodo final debe coincidir con el origen de el paso para poder concatenar.");
				}
			}
		}

		/// <summary>
		/// Concatena esta ruta
		/// </summary>
		/// <param name="ruta">Ruta.</param>
		public void Concat (IRuta<T> ruta)
		{
			if (!NodoFinal.Equals (ruta.NodoInicial))
				throw new RutaInconsistenteException ("No se puede concatenar si no coinciden los extremos finales e iniciales de los nodos.");

			foreach (var paso in ruta.Pasos)
			{
				Paso.Add (paso);
			}
		}

		/// <summary>
		/// Devuelve el origen de la ruta
		/// </summary>
		/// <value>The nodo inicial.</value>
		public T NodoInicial
		{
			get
			{
				if (NumPasos < 0)
					throw new System.Exception ("No existe el nodo final en un path vacío.");
				return Paso [0].Origen;
			}
		}

		/// <summary>
		/// Devuelve el destino de la ruta
		/// </summary>
		/// <value>The nodo final.</value>
		public T NodoFinal
		{
			get
			{
				if (NumPasos < 0)
					throw new System.Exception ("No existe el nodo final en un path vacío.");
				return Paso [NumPasos - 1].Destino;
			}
		}

		/// <summary>
		/// Devuelve el número de pasos en la ruta
		/// </summary>
		/// <value>The number pasos.</value>
		public int NumPasos
		{
			get
			{
				return Paso.Count;
			}
		}
	}
}