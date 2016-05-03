using System;
using System.Collections.Generic;
using Graficas.Aristas;

namespace Graficas.Rutas
{
	[Serializable]
	/// <summary>
	/// Representa una ruta
	/// </summary>
	public class Ruta<T> : IRuta<T>
	{
		[Serializable]
		protected struct NodoPeso
		{
			public T Nodo;
			public float Peso;

			public NodoPeso (T nodo, float peso)
			{
				Nodo = nodo;
				Peso = peso;
			}
		}

		public Ruta ()
		{
		}

		/// <param name="origen">Origen</param>
		public Ruta (T origen)
		{
			Paso.Add (new NodoPeso (origen, 0));
		}

		/// <summary>
		/// Construye una implementación de esta ruta, dada una ruta abstracta
		/// </summary>
		/// <param name="ruta">Ruta a imitar</param>
		public Ruta (IRuta<T> ruta)
			: this (ruta.NodoInicial)
		{
			foreach (var x in ruta.Pasos)
			{
				Paso.Add (new NodoPeso (x.Destino, x.Peso));
			}
		}

		/// <param name="aris">Arista inicial</param>
		public Ruta (IArista<T> aris)
		{
			Paso.Add (new NodoPeso (aris.Origen, 0));
			Paso.Add (new NodoPeso (aris.Destino, aris.Peso));
		}

		/// <summary>
		/// Lista de pasos de esta ruta.
		/// </summary>
		readonly protected IList<NodoPeso> Paso = new List<NodoPeso> ();

		public override string ToString ()
		{
			string ret = string.Format ("[{0}]: ", NumPasos);
			foreach (var x in Paso)
			{
				ret += string.Format (" {0} ", x.Nodo);

			}
			return ret;
		}

		/// <summary>
		/// Enumera los pasos de la ruta
		/// </summary>
		public IEnumerable<IPaso<T>> Pasos
		{ 
			get
			{ 
				var ret = new List<IPaso<T>> ();
				for (int i = 0; i < Paso.Count - 1; i++)
				{
					ret.Add (new Paso<T> (Paso [i].Nodo, Paso [i + 1].Nodo, Paso [i + 1].Peso));
				}
				return ret;
			} 
		}

		/// <summary>
		/// Concatena con un paso una ruta
		/// </summary>
		/// <param name="paso">Paso con qué concatenar</param>
		public void Concat (IPaso<T> paso)
		{
			if (paso == null)
				throw new NullReferenceException ();
			if (NumPasos == 0)
			{
				Paso.Add (new NodoPeso (paso.Origen, 0));
				Paso.Add (new NodoPeso (paso.Destino, paso.Peso));
			}
			else
			{
				if (NodoFinal.Equals (paso.Origen))
				{
					Paso.Add (new NodoPeso (paso.Destino, paso.Peso));
				}
				else
				{
					throw new System.Exception ("El nodo final debe coincidir con el origen de el paso para poder concatenar.");
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
				throw new System.Exception ("No se puede concatenar si no coinciden los extremos finales e iniciales de los nodos.");

			foreach (var paso in ruta.Pasos)
			{
				Paso.Add (new NodoPeso (paso.Destino, paso.Peso));
			}
		}

		/// <summary>
		/// Concatena esta ruta
		/// </summary>
		/// <param name="nodo">Nodo con qué concatenar</param>
		/// <param name="peso">distancia de el Nodo final a este nuevo nodo</param>
		public void Concat (T nodo, float peso)
		{
			Paso.Add (new NodoPeso (nodo, peso));
		}

		/// <summary>
		/// Construye uan ruta como ésta, en sentido inverso.
		/// </summary>
		public Ruta<T> Reversa ()
		{
			var ret = new Ruta<T> ();
			for (int i = Paso.Count - 1; i >= 0; i--)
			{
				ret.Paso.Add (Paso [i]);
			}
			return ret;
		}

		IRuta<T> IRuta<T>.Reversa ()
		{
			return Reversa ();
		}

		/// <summary>
		/// Devuelve el origen de la ruta
		/// </summary>
		/// <value>The nodo inicial.</value>
		public T NodoInicial
		{
			get
			{
				return Paso [0].Nodo;
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
				return Paso [NumPasos].Nodo;
			}
		}

		/// <summary>
		/// Devuelve el peso total de la ruta
		/// </summary>
		/// <value>The longitud.</value>
		public float Longitud
		{
			get
			{
				float ret = 0;
				foreach (var x in Paso)
				{
					ret += x.Peso;
				}
				return ret;
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
				return Paso.Count - 1;
			}
		}
	}
}