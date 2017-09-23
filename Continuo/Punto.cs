using System;
using System.Linq;
using ListasExtra;
using System.Collections.Generic;
using Graficas.Aristas;

namespace Graficas.Continuo
{
	/// <summary>
	/// Representa un punto en un continuo.
	/// </summary>
	[Serializable]
	public class Punto<T> : IDisposable
	{
		#region General

		/// <summary>
		/// 
		/// </summary>
		public override string ToString ()
		{
			return EnOrigen ? A.ToString () : string.Format (
				"[{0}, {1}]@{2}",
				A,
				B,
				Loc);
		}

		/// <summary>
		/// Devuelve un clón (aún como elemento de la gráfica base) de este punto.
		/// </summary>
		public Punto<T> Clonar ()
		{
			return new Punto<T> (Universo, A, B, Loc);
		}

		/// <summary>
		/// Elimina este punto de la gráfica y se libera.
		/// </summary>
		public void Remove ()
		{
			Universo.Puntos.Remove (this);
		}

		#endregion

		#region Ctor

		/// <param name="universo">Continuo donde vive este punto</param>
		/// <param name="nodo">Nodo donde 'poner' el punto</param>
		public Punto (Continuo<T> universo, T nodo)
			: this (universo, nodo, default(T), 0)
		{
		}

		internal Punto (T nodo)
		{
			A = nodo;
		}

		/// <param name="universo">Universo.</param>
		/// <remarks>Asegúrese de asignar un valor a A al heredar este constructor.</remarks>
		protected Punto (Continuo<T> universo)
		{
			Universo = universo;
		}

		/// <param name="universo">Continuo donde vive este punto</param>
		/// <param name="p0">Un punto fijo adyacence a este nuevo punto</param>
		/// <param name="p1">El otro punto fijo adyacence a este nuevo punto</param>
		/// <param name="dist">Distancia de este punto al primer punto fijo dado</param>
		/// <remarks>Hacer p1 == null hace que este punto nuevo coincida con el primer punto. 
		/// Hacer a p0 == null tira exception.</remarks>
		public Punto (Continuo<T> universo, T p0, T p1, float dist)
		{
			Universo = universo;
			A = p0;
			B = p1;
			Loc = dist;
			Universo.Puntos.Add (this);
		}


		#endregion

		#region Posición

		float _loc;

		T a;

		/// <summary>
		/// Posición A
		/// </summary>
		public T A
		{
			get
			{
				return a;
			}
			set
			{
				// Analysis disable CompareNonConstrainedGenericWithNull
				if (value == null)
					// Analysis restore CompareNonConstrainedGenericWithNull
						throw new ArgumentNullException ("value", "A no puede ser nulo.");
				a = value;
			}
		}

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
					throw new ArgumentException ("Loc no puede ser negativo", "value");
				// Analysis disable CompareNonConstrainedGenericWithNull
				if (B != null && Aloc < 0)
					throw new ArgumentException (
						"Loc no puede ser mayor que si distancia al otro extremo",
						"value");
			}
		}

		/// <summary>
		/// Distancia hasta B   [A --- aquí --[aloc]-- B]
		/// </summary>
		public float Aloc
		{
			get
			{
				if (EnOrigen)
					throw new OperaciónAristaInválidaException ("No se puede acceder a ALoc estando en un punto fijo");
				return Universo.GrafoBase [A, B] - Loc;
			}
		}

		/// <summary>
		/// Revisa si dos puntos están en un mismo intervalo
		/// </summary>
		public static bool EnMismoIntervalo (Punto<T> punto1,
		                                     Punto<T> punto2)
		{
			if (punto1 == null || punto2 == null)
				return false;
			return punto1.EnMismoIntervalo (punto2);
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

		/// <summary>
		/// Devuelve la distancia a uno de sus dos extremos
		/// </summary>
		public float DistanciaAExtremo (T extremo)
		{
			if (EnOrigen)
			{
				if (Universo.ComparaNodos.Equals (A, extremo))
					return 0;
				var ar = Universo.GrafoBase.EncuentraArista (A, extremo);
				if (ar.Exists)
					return ar.Data;
				throw new IndexOutOfRangeException (string.Format (
					"{0} no es un extremo de {1}",
					extremo,
					this));
			}
			if (Universo.ComparaNodos.Equals (extremo, A))
				return Loc;
			if (Universo.ComparaNodos.Equals (extremo, B))
				return Aloc;

			throw new IndexOutOfRangeException (string.Format (
				"{0} no es un extremo de {1}",
				extremo,
				this));
		}


		#endregion

		#region Topología

		/// <summary>
		/// El universo donde habita este continuo
		/// </summary>
		protected readonly Continuo<T> Universo;

		/// <summary>
		/// Revisa y devuelve si este punto está en un nodo.
		/// </summary>
		public bool EnOrigen
		{
			get
			{
				return Loc <= 0;
			}
		}

		/// <summary>
		/// Extremos más próximos a este punto en la gráfica Universo.
		/// </summary>
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
		public bool EnMismoIntervalo (Punto<T> punto)
		{
			// Esto si ambos extremos esan definidos
			if (EnOrigen)
			{
				if (punto.EnOrigen)
				{
					// True si son vecinos según Universo
					return Universo.ComparaNodos.Equals (A, punto.A) ||
					Universo.GrafoBase.ExisteArista (A, punto.A);
				}
				else
				{
					if (!punto.Extremos.Contiene (A))
						return false;
					var nodo = punto.Extremos.Excepto (A);
					return !float.IsPositiveInfinity (Universo.GrafoBase [A, nodo]);
				}
			}
			return punto.EnOrigen ? punto.EnMismoIntervalo (this) : Extremos.Equals (punto.Extremos);
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
				return Universo.ComparaNodos.Equals (A, p1) ||
				Universo.ComparaNodos.Equals (A, p2);
			}
			return new ParNoOrdenado<T> (p1, p2).Equals (Extremos);
		}

		/// <summary>
		/// Devuelve la lista de nodos (estrictamente) contiguos a este punto
		/// </summary>
		/// <returns>Una nueva lista.</returns>
		public ICollection<Punto<T>> Vecindad ()
		{
			if (EnOrigen)
			{
				T orig = A; // Posición de este punto.
				// Si estoy en vértice
				var ret = new HashSet<Punto<T>> (Universo.Puntos.Where (x => x.Extremos.Contiene (orig)));
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

			if (restante > dist) // No llega
			{
				using (var anterior = Clonar ())
				{
					if (Universo.ComparaNodos.Equals (destino, A))
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
				}
				return false;
			}

			//using var otroAnt = 
			dist = dist - restante;
			AlDesplazarse?.Invoke ();
			AlLlegarANodo?.Invoke ();
			using (var anterior = Clonar ())
			{
				DesdeGrafo (destino);
				VerificaColisión (anterior);
			}
			return true;
		}

		void VerificaColisión (Punto<T> anterior)
		{
			var extremosBase = new List<T> (Extremos.AsSet ().Intersect (anterior.Extremos.AsSet ()));
			var extremoBase = extremosBase [0];


			var n0 = anterior.DistanciaAExtremo (extremoBase);
			var n1 = DistanciaAExtremo (extremoBase);
			var maxDist = Math.Max (n0, n1);
			var minDist = Math.Min (n0, n1);
			var puntosIntervalo = Vecindad ();
			foreach (var x in puntosIntervalo)
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

		/// <summary>
		/// Avanza por una ruta una distancia específica
		/// </summary>
		/// <returns><c>true</c>, si termina la ruta; <c>false</c> otherwise.</returns>
		/// <param name="ruta">Ruta.</param>
		/// <param name="dist">Dist.</param>
		public bool AvanzarHacia (Ruta<T> ruta, float dist)
		{
			foreach (var r in new List<IPaso<T>> (ruta.Pasos))
			{
				if (!AvanzarHacia (r.Destino, ref dist))
					return false;

				// Si llega aquí es que avanzó exactamente hasta un nodo hasta este momento.
				// Hay que eliminar el paso de la ruta y actualizar Inicial.
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
		/// <exception cref="System.Exception">Cuando no se intenta avanzar hacia un vecino inmediato</exception>
		public bool AvanzarHacia (Punto<T> destino, ref float dist)
		{
			if (!EnMismoIntervalo (destino))
				throw new Exception (string.Format ("No se puede avanzar si no coinciden\n{0} avanzando hacia {1}.\tDist:{2}",
					this,
					destino,
					dist));

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
			return Universo.ComparaPuntos.Equals (destino, this);
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
		public event Action<Punto<T>> AlColisionar;

		#endregion

		#region Conversores

		/// <summary>
		/// Pone a este punto en un punto de la gráfica.
		/// </summary>
		[Obsolete ("Usar this.DesdeGrafo")]
		public void FromGrafo (T punto)
		{
			A = punto;
			B = default(T);
			Loc = 0;
		}

		/// <summary>
		/// Pone a este punto en un punto de la gráfica.
		/// </summary>
		public void DesdeGrafo (T punto)
		{
			A = punto;
			B = default(T);
			Loc = 0;
		}

		#endregion

		#region IEquatable implementation

		/// <summary>
		/// Dos puntos se dicen iguales si representan el mismo punto encajado en el grafo.
		/// </summary>
		/// <param name="other">Comparando/>.</param>
		public bool Coincide (Punto<T> other)
		{
			try
			{
				if (other == null)
					return false;
				if (EnOrigen)
					return (other.EnOrigen && Universo.ComparaNodos.Equals (A, other.A));
				var ret =
					(Universo.ComparaNodos.Equals (A, other.A) &&
					Universo.ComparaNodos.Equals (B, other.B) &&
					Loc == other.Loc)

					||

					(Universo.ComparaNodos.Equals (A, other.B) &&
					Universo.ComparaNodos.Equals (B, other.A) &&
					Loc == other.Aloc);
				return ret;
			}
			catch (Exception ex)
			{
				var salida = string.Format (
					             "Se produce exception al comparar Puntos en Continuo\nthis:  {0}\nother: {1}",
					             Mostrar (),
					             other.Mostrar ());
				throw new Exception (salida, ex);
			}
		}

		string Mostrar ()
		{
			return string.Format (
				"[ContinuoPunto: _loc={0}, Universo={1}, A={2}, B={3}, Loc={4}, Aloc={5}, EnOrigen={6}, Extremos={7}]",
				_loc,
				Universo,
				A,
				B,
				Loc,
				Aloc,
				EnOrigen,
				Extremos);
		}


		#endregion

		#region IDisposable implementation

		void IDisposable.Dispose ()
		{
			Universo.Puntos.Remove (this);
		}

		#endregion
	}
}

