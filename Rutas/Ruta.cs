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
	{
		/// <summary>
		/// 
		/// </summary>
		public Ruta ()
		{
		}

		/// <param name="inicial">Nodo inicial</param>
		[Obsolete]
		public Ruta (T inicial)
		{
			_virtualInicial = inicial;
		}

		/// <param name="origen">Nodo inicial</param>
		/// <param name="destino">Nodo final</param>
		/// <param name="peso">Peso del paso</param>
		/// <remarks>origen-destino debe ser una arista</remarks>
		public Ruta (T origen, T destino, float peso = 1)
		{
			Paso.Add (new Step<T> (origen, destino, peso)); 
		}

		/// <summary>
		/// Construye una implementación de esta ruta, dada una ruta abstracta
		/// </summary>
		/// <param name="ruta">Ruta a imitar</param>
		public Ruta (IRuta<T> ruta)
		{
			if (ruta == null)
				throw new ArgumentNullException ();
			_virtualInicial = ruta.NodoInicial;
			foreach (var x in ruta.Pasos)
				Paso.Add (new Step<T> (x.Origin, x.Destination, x.Peso));
		}

		/// <summary>
		/// Lista de pasos de esta ruta.
		/// </summary>
		readonly protected IList<IStep<T>> Paso = new List<IStep<T>> ();

		/// <summary>
		/// 
		/// </summary>
		public override string ToString ()
		{
			string ret = string.Format ("[{0}]: ", NumPasos);
			foreach (var x in Paso)
				ret += string.Format (" {0} ", x);
			return ret;
		}

		/// <summary>
		/// Enumera los pasos de la ruta
		/// </summary>
		IEnumerable<IStep<T>> IRuta<T>.Pasos
		{ 
			get
			{ 
				return  Pasos;
			} 
		}

		/// <summary>
		/// Enumera los pasos de la ruta
		/// </summary>
		/// <value>Un clón de los pasos</value>
		public List<IStep<T>> Pasos
		{
			get
			{
				return new List<IStep<T>> (Paso);
			}
		}

		/// <summary>
		/// Devuelve la longitud de la ruta
		/// </summary>
		/// <value>The longitud.</value>
		public float Longitud
		{
			get
			{
				float ret = 0;
				foreach (var x in Paso)
					ret += x.Peso;
				return ret;
			}
		}

		/// <summary>
		/// Concatena con un paso una ruta
		/// </summary>
		/// <param name="paso">Paso con qué concatenar</param>
		public void Concat (IStep<T> paso)
		{
			if (paso == null)
				throw new NullReferenceException ("No se puede concatenar con una arista nula.");
			if (NumPasos == 0)
			{
				Paso.Add (new Step<T> (paso));
			}
			else
			{
				if (paso.Intersects (NodoFinal))
				{
					Paso.Add (new Step<T> (paso));
				}
				else
				{
					throw new RutaInconsistenteException ("El nodo final debe cortar a la arista para poder concatenar.");
				}
			}
		}

		/// <summary>
		/// Se concatena con una arista
		/// </summary>
		/// <param name="paso">Arista que compone</param>
		/// <param name="origen">Nodo intersección entre esta ruta y la arista</param>
		public void Concat (IEdge<T> paso, T origen)
		{
			var p = new Step<T> (origen, paso.Antipode ((origen)));
			Concat (p);
		}

		/// <summary>
		/// Concatena esta ruta
		/// </summary>
		/// <param name="ruta">Ruta.</param>
		public void Concat (IRuta<T> ruta)
		{
			if (ruta.NumPasos == 0)
				return;
			if (NumPasos > 0 && !NodoFinal.Equals (ruta.NodoInicial))
				throw new RutaInconsistenteException ("No se puede concatenar si no coinciden los extremos finales e iniciales de los nodos.");

			foreach (var paso in ruta.Pasos)
			{
				Paso.Add (new Step<T> (paso));
			}
		}

		T _virtualInicial;

		/// <summary>
		/// Devuelve el origen de la ruta
		/// </summary>
		/// <value>The nodo inicial.</value>
		public T NodoInicial
		{
			get
			{
				return Paso.Count == 0 ? _virtualInicial : Paso [0].Origin;
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
				return NumPasos < 1 ? NodoInicial : Paso [NumPasos - 1].Destination;
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

		/// <summary>
		/// Devuelve un valor que indica si esta ruta es nula, ie. No tiene pasos
		/// </summary>
		/// <remarks>Que la ruta sea nula no es lo mismo que su valor sea <c>null</c>.</remarks>
		public bool EsNulo
		{
			get
			{
				return NumPasos == 0;
			}
		}

		/// <summary>
		/// Revisa si una ruta es nula o es referencia nula.
		/// </summary>
		/// <returns><c>true</c>, si es nula, <c>false</c> otherwise.</returns>
		/// <param name="r">Ruta</param>
		public static bool RutaNula (Ruta<T> r)
		{
			// Analysis disable ConstantNullCoalescingCondition
			return r?.EsNulo ?? true;
			// Analysis restore ConstantNullCoalescingCondition
		}

		/// <summary>
		/// Devuelve la ruta nula
		/// </summary>
		/// <value>The paso nulo.</value>
		public static Ruta<T> Nulo
		{
			get
			{
				var ret = new Ruta<T> ();
				return ret;
			}
		}
	}
}