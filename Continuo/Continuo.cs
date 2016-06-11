using System;
using System.Collections.Generic;
using Graficas.Rutas;
using ListasExtra;
using System.Diagnostics;
using Graficas.Aristas;
using Graficas.Grafo;
using System.Linq;

namespace Graficas.Continuo
{

	/// <summary>
	/// Representa un continuo producido por una IGrafica
	/// </summary>
	[Serializable]
	public class Continuo<T>
	{
		#region General

		/// <summary>
		/// Gráfica donde vive este contnuo
		/// </summary>
		public readonly Grafo<T, float> GrafoBase;

		/// <summary>
		/// Conjuntos de puntos asociados a este continuo
		/// </summary>
		public readonly List<Punto<T>> Puntos = new List<Punto<T>> ();

		/// <summary>
		/// Agrega un punto al grafo y lo devuelve
		/// </summary>
		/// <param name="a">Un extremo del intervalo</param>
		/// <param name="b">Otro extremo del intervalo</param>
		/// <param name="loc">Distancia de este punto a el primer punto dado, a</param>
		public Punto<T> AgregaPunto (T a, T b, float loc)
		{
			if (ReferenceEquals (a, null))
				throw new ArgumentNullException (
					"a",
					"El valor de un extremo no puede ser nulo.");
			var ret = new Punto<T> (this, a, b, loc);
			ret.A = a;
			ret.B = b;
			ret.Loc = loc;
			return ret;
		}

		/// <summary>
		/// Agrega un punto al grafo y lo devuelve
		/// </summary>
		/// <param name="a">Punto en el grafo</param>
		public Punto<T> AgregaPunto (T a)
		{
			return new Punto<T> (this, a);
		}

		/// <summary>
		/// Enumera los puntos existentes en una arista
		/// </summary>
		/// <param name="origen">Un extemo de la arista</param>
		/// <param name="destino">Segundo extremo de la arista</param>
		public IEnumerable<Punto<T>> PuntosArista (T origen, T destino)
		{
			var aris = new ParNoOrdenado<T> (origen, destino);
			return PuntosArista (aris);
		}

		/// <summary>
		/// Enumera los puntos existentes en una arista
		/// </summary>
		/// <param name="arista">Arista.</param>
		public IEnumerable<Punto<T>> PuntosArista (IArista<T> arista)
		{
			var aris = arista.ComoPar ();
			return PuntosArista (aris);
		}

		#endregion

		#region Interno

		readonly Dictionary<T, Punto<T>> puntosFijos;

		IEnumerable<Punto<T>> PuntosArista (ParNoOrdenado<T> arista)
		{
			foreach (var x in Puntos)
			{
				if (x.Extremos.Equals (arista))
				{
					yield return x;
				}
			}

		}

		IEqualityComparer<T> ComparaNodos
		{
			get
			{
				return GrafoBase.Comparador;
			}
		}

		/// <summary>
		/// Devuelve el comparador que se usa para los puntos (flotantes)
		/// </summary>
		protected IEqualityComparer<Punto<T>> ComparaPuntos { get; }

		#endregion

		#region Acceso a puntos

		/// <summary>
		/// Devuelve el punto en el continuo equivalente a un nodo del grafo.
		/// </summary>
		/// <param name="punto">Nodo en el grafo</param>
		public Punto<T> PuntoFijo (T punto)
		{
			Punto<T> ret;
			if (puntosFijos.Any (z => z.Key.Equals (punto)))
				return puntosFijos.First (z => z.Key.Equals (punto)).Value;
			ret = new Punto<T> (punto);
			puntosFijos.Add (punto, ret);
			return ret;
		}

		/// <summary>
		/// Devuelve una nueva lista de los puntos que hay en dos nodos consecutivos.
		/// </summary>
		public ICollection<Punto<T>> PuntosEnIntervalo (T p1, T p2)
		{
			return Puntos.FindAll (x => x.EnIntervaloInmediato (p1, p2));
		}

		#endregion

		#region Ctor

		/// <summary>
		/// Construye una instancia de esta clase con una grafica dada como base.
		/// </summary>
		/// <param name="gráfica">Grafica base</param>
		public Continuo (Grafo<T, float> gráfica)
		{
			Debug.Assert (
				gráfica.SóloLectura,
				"Este grafo debe ser sólo lectura para evitar comportamiento inesperado."); 
			GrafoBase = gráfica;
			ComparaPuntos = new ComparadorCoincidencia<T> (ComparaNodos);
			puntosFijos = new Dictionary<T, Punto<T>> (ComparaNodos);
			foreach (var x in gráfica.Nodos)
				puntosFijos.Add (x, AgregaPunto (x));
		}

		#endregion

		#region Rutas

		/// <summary>
		/// Devuelve la ruta óptima entre dos puntos
		/// </summary>
		/// <param name="inicial">Punto inicial.</param>
		/// <param name="final">Punto final.</param>
		/// <param name="rutas">Conjunto de rutas óptimas previamente calculadas.</param>
		public static Ruta<T> RutaÓptima (Punto<T> inicial,
		                                  Punto<T> final,
		                                  ConjuntoRutasÓptimas<T> rutas)
		{
			var ruta = rutas.CaminoÓptimo (inicial.A, final.A);
			var ret = new Ruta<T> (inicial);
			ret.Concat (ruta);
			ret.ConcatFinal (final);
			return ret;
		}

		#endregion

		#region Debug

		[Conditional ("DEBUG")]
		public void ProbarIntegridad ()
		{
			string generalExcMsg = string.Format ("Fallo de integridad en {0}\n", this);
			foreach (var p in Puntos)
			{
				if (ReferenceEquals (p, null))
					throw new ArgumentNullException (generalExcMsg + "Punto nulo en universo.");
				if (typeof (T).IsClass)
				{
					if (ReferenceEquals (p.A, null) && ReferenceEquals (p.B, null))
						throw new ArgumentNullException (generalExcMsg + "Punto no nulo con extremos nulos.");
				}
			}
		}

		#endregion
	}
}