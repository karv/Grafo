using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Graficas.Aristas;
using Graficas.Grafo;
using Graficas.Grafo.Estáticos;
using Graficas.Rutas;
using ListasExtra;

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
			// FIXME: ¿Por qué no estoy usandoque arista sea una pareja?
			return Puntos.Where (x => x.Extremos.Equals (arista));
		}

		/// <summary>
		/// Devuelve el comparador de nodos que se utiliza.
		/// Es el mismo que usa el grafo base.
		/// </summary>
		public IEqualityComparer<T> ComparaNodos
		{
			get
			{
				return GrafoBase.Comparador;
			}
		}

		/// <summary>
		/// Devuelve el comparador que se usa para los puntos (flotantes)
		/// </summary>
		public IEqualityComparer<Punto<T>> ComparaPuntos { get; }

		#endregion

		#region Acceso a puntos

		/// <summary>
		/// Devuelve el punto en el continuo equivalente a un nodo del grafo.
		/// </summary>
		/// <param name="punto">Nodo en el grafo</param>
		public Punto<T> PuntoFijo (T punto)
		{
			try
			{
				return puntosFijos [punto];
			}
			catch (KeyNotFoundException ex)
			{
				throw new NodoInexistenteException (
					string.Format (
						"El nodo {0} no se encuentra en el grafo base {1}.",
						punto,
						GrafoBase),
					ex);
			}
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
	}
}