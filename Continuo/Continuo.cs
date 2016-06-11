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

		/// <summary>
		/// Una ruta de ContinuoPuntos
		/// </summary>
		[Serializable]
		public class Ruta : Ruta<T>
		{
			/// <summary>
			/// Devuelve el origen de la ruta
			/// </summary>
			/// <value>The nodo inicial.</value>
			public new Punto<T> NodoInicial { get; }

			/*{
				get
				{
					return (Pasos.Count > 0) ? Pasos [0].Origen : intInicial;

				}
			}*/

			/// <summary>
			/// Devuelve el destino de la ruta
			/// </summary>
			/// <value>The nodo final.</value>
			public new Punto<T> NodoFinal { get; private set; }

			/// <summary>
			/// Initializes a new instance of the class
			/// </summary>
			/// <param name="inicial">Punto de origen</param>
			public Ruta (Punto<T> inicial)
			{
				NodoInicial = inicial;
				NodoFinal = inicial;
			}

			/// <summary>
			/// Elimina el primer paso.
			/// </summary>
			public void EliminarPrimero ()
			{
				Paso.RemoveAt (0);
				//NodoInicial = Paso [0].Origen;
			}

			/// <summary>
			/// Concatena finalmente con un punto
			/// </summary>
			/// <param name="final">Final.</param>
			public void ConcatFinal (Punto<T> final)
			{
				NodoFinal = final;
			}

			/// <summary>
			/// Devuelve el peso total de la ruta
			/// </summary>
			/// <value>The longitud.</value>
			public new float Longitud
			{
				get
				{
					var lbase = 0f;
					foreach (var x in Paso)
						lbase += x.Peso;
			
					return lbase + NodoInicial.DistanciaAExtremo (base.NodoInicial) + NodoFinal.DistanciaAExtremo (base.NodoFinal);
				}
			}

			/// <summary>
			/// Devuelve el número de pasos en la ruta
			/// </summary>
			/// <value>The number pasos.</value>
			public new int NumPasos
			{
				get
				{
					return base.NumPasos + 2;
				}
			}

			/// <summary>
			/// Revisa si un punto dado pertenece a esta ruta.
			/// </summary>
			/// <param name="punto">Punto.</param>
			public bool Contiene (Punto<T> punto)
			{
				// Hay de tres:
				// 0) Está en el semiintervalo inicial
				// 1) Está en el semiintervalo final
				// 2) Está en un intervalo intermedio
			
				// 0)
				if (NodoInicial.EnMismoIntervalo (punto))
				{
					T MyA = punto.A;
					if (NodoInicial.DistanciaAExtremo (MyA) <= punto.Loc)
						return true;
				}

				// 2)
				foreach (var x in Pasos)
				{
					if (punto.EnIntervaloInmediato (x.Origen, x.Destino))
						return true;
				}

				// 1)
				if (NodoFinal.EnMismoIntervalo (punto))
				{
					T MyB = punto.B;
					if (NodoFinal.DistanciaAExtremo (MyB) < punto.Aloc)
						return true;
				}

				return false;
			}

			/// <summary>
			/// Devuelve una enumeración de los puntos contenidos en esta ruta
			/// </summary>
			public List<Punto<T>> PuntosEnRuta (Continuo<T> gr)
			{
				return gr.Puntos.FindAll (Contiene);
			}
		}

		/// <summary>
		/// Gráfica donde vive este contnuo
		/// </summary>
		public readonly Grafo<T, float> GráficaBase;

		/// <summary>
		/// Conjuntos de puntos asociados a este continuo
		/// </summary>
		public readonly List<Punto<T>> Puntos = new List<Punto<T>> ();

		readonly Dictionary<T, Punto<T>> puntosFijos = new Dictionary<T, Punto<T>> ();

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
		/// Construye una instancia de esta clase con una grafica dada como base.
		/// </summary>
		/// <param name="gráfica">Grafica base</param>
		public Continuo (Grafo<T, float> gráfica)
		{
			Debug.Assert (
				gráfica.SóloLectura,
				"Este grafo debe ser sólo lectura para evitar comportamiento inesperado."); 
			GráficaBase = gráfica;
			foreach (var x in gráfica.Nodos)
				puntosFijos.Add (x, AgregaPunto (x));
		}

		/// <summary>
		/// Devuelve la ruta óptima entre dos puntos
		/// </summary>
		/// <param name="inicial">Punto inicial.</param>
		/// <param name="final">Punto final.</param>
		/// <param name="rutas">Conjunto de rutas óptimas previamente calculadas.</param>
		public static Ruta RutaÓptima (Punto<T> inicial,
		                               Punto<T> final,
		                               ConjuntoRutasÓptimas<T> rutas)
		{
			var ruta = rutas.CaminoÓptimo (inicial.A, final.A);
			var ret = new Ruta (inicial);
			ret.Concat (ruta);
			ret.ConcatFinal (final);
			return ret;
		}


		/// <summary>
		/// Devuelve una nueva lista de los puntos que hay en dos nodos consecutivos.
		/// </summary>
		public ICollection<Punto<T>> PuntosEnIntervalo (T p1, T p2)
		{
			return Puntos.FindAll (x => x.EnIntervaloInmediato (p1, p2));
		}

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

		[Conditional ("DEBUG")]
		void ProbarIntegridad ()
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
	}
}