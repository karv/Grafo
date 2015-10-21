using System;
using System.Collections.Generic;
using Graficas;
using Graficas.Rutas;
using ListasExtra;
using System.Diagnostics;

namespace Graficas.Continuo
{
	/// <summary>
	/// Representa un continuo producido por una IGrafica
	/// </summary>
	public class Continuo<T>
		where T : IEquatable<T>
	{
		/// <summary>
		/// Representa un punto en un continuo.
		/// </summary>
		public class ContinuoPunto : IEquatable<ContinuoPunto>, IDisposable
		{
			#region General

			public override string ToString ()
			{
				return EnOrigen ? A.ToString () : string.Format ("[{0}, {1}]@{2}", A, B, Loc);
			}

			/// <summary>
			/// Devuelve un clón (aún como elemento de la gráfica base) de este punto.
			/// </summary>
			public ContinuoPunto Clonar ()
			{
				var ret = new ContinuoPunto (Universo);
				ret.A = A;
				ret.B = B;
				ret.Loc = Loc;
				return ret;
			}

			/// <summary>
			/// Elimina este punto de la gráfica y se libera.
			/// </summary>
			public void Remove ()
			{
				Universo.Puntos.Remove (this);
				A = default(T);
				B = default(T);
			}

			#endregion

			#region Ctor

			public ContinuoPunto (Continuo<T> universo, T nodo)
				: this (universo)
			{
				A = nodo;
			}

			public ContinuoPunto (Continuo<T> universo)
			{
				Universo = universo;
				Universo.Puntos.Add (this);
			}

			#endregion

			#region Posición

			float _loc;

			//public ParNoOrdenado<T> Extremos { get; protected set; }

			/// <summary>
			/// Posición A
			/// </summary>
			public T A { get; set; }

			/// <summary>
			/// Posición B
			/// </summary>
			public T B { get; set; }

			/// <summary>
			/// Distancia hasta A   [A --[loc]-- aquí --- B]
			/// </summary>
			public float Loc
			{
				get { return _loc; }
				set
				{ 
					_loc = value; 
					if (_loc < 0)
						FromGrafica (A);
					if (Aloc < 0)
						FromGrafica (B);
				}
			}

			/// <summary>
			/// Distancia hasta B   [A --- aquí --[aloc]-- B]
			/// </summary>
			public float Aloc
			{
				get
				{
					return Universo.GráficaBase [A, B] - Loc;
				}
			}

			/// <summary>
			/// Revisa si dos puntos están en un mismo intervalo
			/// </summary>
			public static bool EnMismoIntervalo (ContinuoPunto punto1, ContinuoPunto punto2)
			{
				if (punto1.A.Equals (punto2.A) && punto1.B.Equals (punto2.B))
					return true;
				
				return false;
			}

			/// <summary>
			/// Invierte, si es posible, A con B
			/// </summary>
			protected void Invertir ()
			{
				if (!EnOrigen)
				{
					T nodoTmp = A;
					A = B;
					B = nodoTmp;
					Loc = Aloc;
				}
			}

			#endregion

			/// <summary>
			/// Revisa si este punto coincide (están en un mismo intervalo) con otro
			/// </summary>
			[Obsolete ("Usar EnMismoIntervalo()")]
			public bool CoincideCon (ContinuoPunto punto)
			{
				if (EnOrigen)
				{
					return punto.EnOrigen ? A.Equals (punto.A) : punto.CoincideCon (this);
				}
				if (punto.EnOrigen)
					return A.Equals (punto.A) || B.Equals (punto.B);
				return Extremos.Equals (punto.Extremos);
			}

			/// <summary>
			/// Devuelve la distancia a uno de sus dos extremos
			/// </summary>
			public float DistanciaAExtremo (T extremo)
			{
				if (extremo.Equals (A))
					return Loc;
				if (extremo.Equals (B))
					return Aloc;
				if (EnOrigen && !float.IsInfinity (Universo.GráficaBase [A, extremo]))
					return Universo.GráficaBase [A, extremo];

				throw new IndexOutOfRangeException (string.Format ("{0} no es un extremo de {1}", extremo, this));
				
			}

			#region Topología

			protected readonly Continuo<T> Universo;

			/// <summary>
			/// Revisa y devuelve si este punto está en un nodo.
			/// </summary>
			public bool EnOrigen
			{
				get
				{
					return Loc == 0;
				}
			}

			public ParNoOrdenado<T> Extremos
			{
				get
				{ return new ParNoOrdenado<T> (A, B); }
			}

			/// <summary>
			/// Revisa si éste y otro punto están en un mismo intervalo.
			/// </summary>
			/// <returns><c>true</c>, if mismo intervalo was ened, <c>false</c> otherwise.</returns>
			/// <param name="punto">Punto.</param>
			public bool EnMismoIntervalo (ContinuoPunto punto)
			{
				// Esto si ambos extremos esan definidos
				if (EnOrigen)
				{
					if (punto.EnOrigen)
					{
						// True si son vecinos según Universo
						return A.Equals (punto.A) || (!float.IsPositiveInfinity (Universo.GráficaBase [A, punto.A]));
					}
					else
					{
						if (!punto.Extremos.Contiene (A))
							return false;
						var nodo = punto.Extremos.Excepto (A);
						return !float.IsPositiveInfinity (Universo.GráficaBase [A, nodo]);
					}
				}
				else
				{
					return punto.EnOrigen ? punto.EnMismoIntervalo (this) : Extremos.Equals (punto.Extremos);
				}
			}

			/// <summary>
			/// Revisa si este punto está en dos vértices contiguos de una gráfica.
			/// </summary>
			/// <returns><c>true</c>, si el punto está en el intervalo, <c>false</c> otherwise.</returns>
			/// <param name="p1">Un extremo del intervalo.</param>
			/// <param name="p2">El otro extramo del intervalo.</param>
			public bool EnIntervaloInmediato (T p1, T p2)
			{
				if (EnOrigen)
				{
					return A.Equals (p1) || A.Equals (p2);
				}
				return new ParNoOrdenado<T> (p1, p2).Equals (Extremos);
			}

			/// <summary>
			/// Devuelve la lista de terrenos contiguos a esta pseudoposición.
			/// </summary>
			/// <returns>Una nueva lista.</returns>
			public ICollection<ContinuoPunto> Vecindad ()
			{
				if (EnOrigen)
				{
					T orig = A; // Posición de este punto.
					var ret = new List<ContinuoPunto> ();
					// Si estoy en terreno
					foreach (var x in Universo.GráficaBase.Vecinos(orig))
					{
						foreach (var y in Universo.PuntosEnIntervalo(orig, x))
						{
							if (!ret.Contains (y))
								ret.Add (y);
						}
					}
					return ret;
				}
				return Universo.PuntosEnIntervalo (A, B);
			}

			#endregion

			#region Dinámico

			/// <summary>
			/// Avanza esta pseudoposición hacia un vecino T una distancia específica
			/// </summary>
			/// <returns><c>true</c>, si llegó <c>false</c> otherwise.</returns>
			/// <param name="destino">Destino.</param>
			/// <param name="dist">Distancia.</param>
			bool AvanzarHacia (T destino, ref float dist)
			{
				var restante = DistanciaAExtremo (destino);
				var anterior = Clonar ();

				if (restante > dist) // No llega
				{
					if (destino.Equals (A))
					{
						Loc -= dist;
					}
					else
					{
						B = destino;
						Loc += dist;
					}
					dist = 0;
					AlDesplazarse?.Invoke ();

					VerificaColisión (anterior);
					return false;
				}

				dist = dist - restante;
				AlDesplazarse?.Invoke ();
				AlLlegarANodo?.Invoke ();
				FromGrafica (destino);
				VerificaColisión (anterior);
				return true;
			}

			void VerificaColisión (ContinuoPunto anterior)
			{
				var extremoBase = anterior.A;
				var minDist = anterior.DistanciaAExtremo (extremoBase);
				var maxDist = DistanciaAExtremo (extremoBase);
				Debug.WriteLineIf (maxDist < minDist, "¡Pasó algo raro!");
				foreach (var x in Universo.PuntosArista(A, B))
				{
					if (!ReferenceEquals (x, this) &&
					    !ReferenceEquals (x, anterior) &&
					    minDist <= x.DistanciaAExtremo (extremoBase) &&
					    maxDist >= x.DistanciaAExtremo (extremoBase))
					{
						AlColisionar?.Invoke (x);
						x.AlColisionar?.Invoke (this);
					}
				}
				anterior.Remove ();
			}

			/// <summary>
			/// Avanza esta pseudoposición hacia un vecino T una distancia específica
			/// </summary>
			/// <returns><c>true</c>, si llegó <c>false</c> otherwise.</returns>
			/// <param name="destino">Destino.</param>
			/// <param name="dist">Distancia.</param>
			public bool AvanzarHacia (T destino, float dist)
			{
				float Ref = dist;
				return AvanzarHacia (destino, ref Ref);
			}

			public bool AvanzarHacia (Ruta ruta, float dist)
			{
				foreach (var r in ruta.Pasos)
				{
					if (r.Destino.Equals (this))
						continue;
					if (!AvanzarHacia (r.Destino, ref dist))
						return false;
					ruta.EliminarPrimero ();
				}
				AlTerminarRuta?.Invoke ();
				return true;
			}

			/// <summary>
			/// Avanza hacia un punto
			/// </summary>
			/// <returns><c>true</c>, si llegó, <c>false</c> otherwise.</returns>
			/// <param name="destino">Destino.</param>
			/// <param name="dist">Distancia</param>
			public bool AvanzarHacia (ContinuoPunto destino, ref float dist)
			{
				if (!EnMismoIntervalo (destino))
					throw new Exception ("No se puede avanzar si no coinciden");

				var relRestante = DistanciaAExtremo (A) - destino.DistanciaAExtremo (A);
				var absRestante = Math.Abs (relRestante);
				var avance = Math.Min (dist, absRestante);
				dist -= avance;

				if (relRestante < 0)
				{
					AvanzarHacia (B, ref avance);
				}
				else
				{
					AvanzarHacia (A, ref avance);
				}
				return Equals (destino);
			}

			#endregion

			#region Eventos

			/// <summary>
			/// Ocurre cuando este punto se desplaza con respecto a la gráfica.
			/// </summary>
			public event Action AlDesplazarse;

			/// <summary>
			/// Ocurre cuando este punto coincide con un punto en la gráfica.
			/// </summary>
			public event Action AlLlegarANodo;

			/// <summary>
			/// Ocurre cuando, al llamar a AvanzarHacia(Ruta), ésta devuelve true.
			/// </summary>
			public event Action AlTerminarRuta;

			/// <summary>
			/// Ocurre cuando un punto colisiona con otro
			/// </summary>
			public event Action<ContinuoPunto> AlColisionar;

			#endregion

			#region Conversores

			/// <summary>
			/// Pone a este punto en un punto de la gráfica.
			/// </summary>
			public void FromGrafica (T punto)
			{
				A = punto;
				B = default(T);
				Loc = 0;
			}

			#endregion

			#region IEquatable implementation

			public bool Equals (ContinuoPunto other)
			{
				if (EnOrigen)
					return (A.Equals (other.A) && other.Loc == 0);
				return (A.Equals (other.A) && B.Equals (other.B) && Loc == other.Loc) ||
				(A.Equals (other.B) && B.Equals (other.A) && Loc == other.Aloc);
			}

			#endregion

			#region IDisposable implementation

			void IDisposable.Dispose ()
			{
				Universo.Puntos.Remove (this);
			}

			#endregion
		}

		/// <summary>
		/// Una ruta de ContinuoPuntos
		/// </summary>
		public class Ruta : Ruta<T>
		{
			/// <summary>
			/// Devuelve el origen de la ruta
			/// </summary>
			/// <value>The nodo inicial.</value>
			public new ContinuoPunto NodoInicial { get; }

			/// <summary>
			/// Devuelve el destino de la ruta
			/// </summary>
			/// <value>The nodo final.</value>
			public new ContinuoPunto NodoFinal { get; private set; }

			/// <summary>
			/// Initializes a new instance of the class
			/// </summary>
			/// <param name="inicial">Punto de origen</param>
			public Ruta (ContinuoPunto inicial)
			{
				NodoInicial = inicial;
			}

			public void EliminarPrimero ()
			{
				Paso.RemoveAt (0);
			}

			/// <summary>
			/// Concatena finalmente con un punto
			/// </summary>
			/// <param name="final">Final.</param>
			public void ConcatFinal (ContinuoPunto final)
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

					return base.Longitud + NodoInicial.Aloc + NodoFinal.Loc;
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
			public bool Contiene (ContinuoPunto punto)
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
			public List<ContinuoPunto> PuntosEnRuta (Continuo<T> gr)
			{
				return gr.Puntos.FindAll (Contiene);
			}
		}

		public readonly ILecturaGrafoPeso<T> GráficaBase;
		public readonly List<ContinuoPunto> Puntos = new List<ContinuoPunto> ();

		public Continuo (ILecturaGrafoPeso<T> grafica)
		{
			GráficaBase = grafica;
		}

		/// <summary>
		/// Devuelve la ruta óptima entre dos puntos
		/// </summary>
		/// <param name="inicial">Punto inicial.</param>
		/// <param name="final">Punto final.</param>
		/// <param name="rutas">Rutas</param>
		public Ruta RutaÓptima (ContinuoPunto inicial, ContinuoPunto final, ConjuntoRutasÓptimas<T> rutas)
		{
			var ruta = rutas.CaminoÓptimo (inicial.A, final.A);
			var ret = new Ruta (inicial);
			ret.Concat (inicial.A, 0);
			ret.Concat (ruta);
			ret.ConcatFinal (final);
			return ret;
		}


		/// <summary>
		/// Devuelve una nueva lista de los puntos que hay en dos nodos consecutivos.
		/// </summary>
		public ICollection<ContinuoPunto> PuntosEnIntervalo (T p1, T p2)
		{
			return Puntos.FindAll (x => x.EnIntervaloInmediato (p1, p2));
		}

		/// <summary>
		/// Agrega un punto al grafo y lo devuelve
		/// </summary>
		/// <param name="a">Un extremo del intervalo</param>
		/// <param name="b">Otro extremo del intervalo</param>
		/// <param name="loc">Distancia de este punto a el primer punto dado, a</param>
		public ContinuoPunto AgregaPunto (T a, T b, float loc)
		{
			var ret = new ContinuoPunto (this);
			ret.A = a;
			ret.B = b;
			ret.Loc = loc;
			return ret;
		}

		/// <summary>
		/// Agrega un punto al grafo y lo devuelve
		/// </summary>
		/// <param name="a">Punto en el grafo</param>
		public ContinuoPunto AgregaPunto (T a)
		{
			return new ContinuoPunto (this, a);
		}

		IEnumerable<ContinuoPunto> PuntosArista (ParNoOrdenado<T> arista)
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
		public IEnumerable<ContinuoPunto> PuntosArista (T origen, T destino)
		{
			var aris = new ParNoOrdenado<T> (origen, destino);
			return PuntosArista (aris);
		}

		/// <summary>
		/// Enumera los puntos existentes en una arista
		/// </summary>
		/// <param name="arista">Arista.</param>
		public IEnumerable<ContinuoPunto> PuntosArista (IArista<T> arista)
		{
			var aris = new ParNoOrdenado<T> (arista.Origen, arista.Destino);
			return PuntosArista (aris);
		}
	}
}