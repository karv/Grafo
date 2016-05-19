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
		class TPaso : IAristaDirigida<T>
		{
			public TPaso (T origen, T destino)
			{
				Origen = origen;
				Destino = destino;
			}

			public T Origen { get; set; }

			public T Destino { get; set; }

			public bool Coincide (T origen, T destino)
			{
				return Origen.Equals (origen) && Destino.Equals (destino);
			}

			public ListasExtra.ParNoOrdenado<T> ComoPar ()
			{
				return new ListasExtra.ParNoOrdenado<T> (Origen, Destino);
			}

			public T Antipodo (T nodo)
			{
				return nodo.Equals (Origen) ? Destino : Origen;
			}

			public bool Corta (T nodo)
			{
				return nodo.Equals (Origen) || nodo.Equals (Destino);
			}

			public bool Existe { get; set; }
		}

		/// <summary>
		/// 
		/// </summary>
		public Ruta ()
		{
		}

		/// <summary>
		/// Construye una implementación de esta ruta, dada una ruta abstracta
		/// </summary>
		/// <param name="ruta">Ruta a imitar</param>
		public Ruta (IRuta<T> ruta)
		{
			foreach (var x in ruta.Pasos)
				Paso.Add (x);
		}

		/// <param name="aris">Arista inicial</param>
		public Ruta (IAristaDirigida<T> aris)
		{
			Paso.Add (aris);
		}

		/// <summary>
		/// Lista de pasos de esta ruta.
		/// </summary>
		readonly protected IList<IAristaDirigida<T>> Paso = new List<IAristaDirigida<T>> ();

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
		public IEnumerable<IAristaDirigida<T>> Pasos
		{ 
			get
			{ 
				return new List<IAristaDirigida<T>> (Paso);
			} 
		}

		/// <summary>
		/// Concatena con un paso una ruta
		/// </summary>
		/// <param name="paso">Paso con qué concatenar</param>
		public void Concat (IAristaDirigida<T> paso)
		{
			if (paso == null)
				throw new NullReferenceException ("No se puede concatenar con una arista nula.");
			if (NumPasos == 0)
			{
				Paso.Add (paso);
			}
			else
			{
				if (paso.Corta (NodoFinal))
				{
					Paso.Add (paso);
				}
				else
				{
					throw new RutaInconsistenteException ("El nodo final debe cortar a la arista para poder concatenar.");
				}
			}
		}

		public void Concat (IArista<T> paso, T origen)
		{
			TPaso p = new Ruta<T>.TPaso (origen, paso.Antipodo ((origen)));
			Concat (p);
		}

		/// <summary>
		/// Concatena esta ruta
		/// </summary>
		/// <param name="ruta">Ruta.</param>
		public void Concat (IRuta<T> ruta)
		{
			if (NumPasos > 0 && !NodoFinal.Equals (ruta.NodoInicial))
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
				if (Paso.Count == 0)
					throw new InvalidOperationException ("Una ruta vacia no tiene definida un NodoInicial.");
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
				if (NumPasos < 1)
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