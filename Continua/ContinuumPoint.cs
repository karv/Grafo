using System;
using System.Collections.Generic;
using System.Linq;
using CE.Graph.Edges;
using ListasExtra;

namespace CE.Graph.Continua
{
	// TODO: Many methods in this must be placed on the continuum class.
	/// <summary>
	/// Representa un punto en un continuo.
	/// </summary>
	[Serializable]
	public class ContinuumPoint<T> : IDisposable, ICloneable
	{
		/// <summary>
		/// A position.
		/// </summary>
		public T A
		{
			get => _a;
			set { _a = value; }
		}

		/// <summary>
		/// B position.
		/// </summary>
		public T B { get; set; }

		/// <summary>
		/// Distance between this point and <see cref="A"/>.
		/// </summary>
		public float Loc
		{
			get { return _loc; }
			set
			{
				_loc = value;
				if (_loc < 0)
					throw new ArgumentException ("Value cannot be negative.", nameof (value));
				if (B != null && Aloc < 0)
					throw new ArgumentException ("Value cannot exceed the length of the current edge.", nameof (value));
			}
		}

		/// <summary>
		/// Gets the distance between this and the second node.
		/// </summary>
		public float Aloc
		{
			get
			{
				if (AtNode)
					throw new InvalidOperationException ("Cannot get property ALoc when this point is a node.");
				return Universe.GrafoBase[A, B] - Loc;
			}
		}

		/// <summary>
		/// Gets the universe where this point exists.
		/// </summary>
		public ContinuumGraph<T> Universe { get; }

		/// <summary>
		/// Gets a value indicating whether this point is on a graph node.
		/// </summary>
		public bool AtNode => Loc <= 0;

		/// <summary>
		/// Gets the endpoint adjacent to this point.
		/// </summary>
		public ParNoOrdenado<T> EndPoints => new ParNoOrdenado<T> (A, B);


		/// <param name="universe">The continuum where this point will be created.</param>
		/// <param name="node">graph node.</param>
		public ContinuumPoint (ContinuumGraph<T> universe, T node)
			: this (universe, node, default (T), 0) { }

		/// <param name="universe">The continuum where this point will be created.</param>
		/// <param name="p0">An adjacent node for this point.</param>
		/// <param name="p1">Another adjacent node.</param>
		/// <param name="dist">Distance between <paramref name="p0"/> and this.</param>
		public ContinuumPoint (ContinuumGraph<T> universe, T p0, T p1, float dist)
		{
			Universe = universe;
			A = p0;
			B = p1;
			Loc = dist;
			Universe.Points.Add (this);
		}

		/// <param name="universe">The continuum where this point will be created.</param>
		/// <remarks>When invoking this, please make sure the field <see cref="A"/> is assigned.</remarks>
		protected ContinuumPoint (ContinuumGraph<T> universe)
		{
			Universe = universe;
		}

		internal ContinuumPoint (T node)
		{
			A = node;
		}

		/// <summary/>
		public override string ToString ()
		{
			return AtNode ? A.ToString () : string.Format (
				"[{0}, {1}]@{2}",
				A,
				B,
				Loc);
		}

		/// <summary>
		/// Gets a copy of this point.
		/// </summary>
		public ContinuumPoint<T> Clone ()
		{
			return new ContinuumPoint<T> (Universe, A, B, Loc);
		}

		/// <summary>
		/// Removes this point from its <see cref="ContinuumGraph{T}"/>.
		/// </summary>
		public void Remove ()
		{
			Universe.Points.Remove (this);
		}

		/// <summary>
		/// Gets the distance to the secified endpoint.
		/// </summary>
		public float DistanciaAExtremo (T extremo)
		{
			if (AtNode)
			{
				if (Universe.NodeComparer.Equals (A, extremo))
					return 0;
				var ar = Universe.GrafoBase.FindEdge (A, extremo);
				if (ar.Exists)
					return ar.Data;
				throw new IndexOutOfRangeException (string.Format (
					"{0} is not an endpoint of {1}",
					extremo,
					this));
			}
			if (Universe.NodeComparer.Equals (extremo, A))
				return Loc;
			if (Universe.NodeComparer.Equals (extremo, B))
				return Aloc;

			throw new IndexOutOfRangeException (string.Format (
				"{0} is not an endpoint of {1}",
				extremo,
				this));
		}

		/// <summary>
		/// Determines whether this and the specified point are on the same edge.
		/// </summary>
		/// <param name="point">Point to compare to.</param>
		public bool OnSameInterval (ContinuumPoint<T> point)
		{
			// TODO: Move to continuum graph.
			if (AtNode)
			{
				if (point.AtNode)
				{
					return Universe.NodeComparer.Equals (A, point.A) ||
					Universe.GrafoBase.EdgeExists (A, point.A);
				}
				else
				{
					if (!point.EndPoints.Contiene (A))
						return false;
					var nodo = point.EndPoints.Excepto (A);
					return !float.IsPositiveInfinity (Universe.GrafoBase[A, nodo]);
				}
			}
			return point.AtNode ? point.OnSameInterval (this) : EndPoints.Equals (point.EndPoints);
		}

		/// <summary>
		/// Determines whether this is on the specified edge.
		/// </summary>
		public bool OnEdge (T p1, T p2)
		{
			// TODO: Move to continuum graph.
			if (AtNode)
			{
				return Universe.NodeComparer.Equals (A, p1) ||
				Universe.NodeComparer.Equals (A, p2);
			}
			return new ParNoOrdenado<T> (p1, p2).Equals (EndPoints);
		}

		/// <summary>
		/// Gets a collection of observable points which does not have a graph node in bewteen.
		/// </summary>
		public ICollection<ContinuumPoint<T>> Neighborhood ()
		{
			if (AtNode)
			{
				T orig = A; // Posición de este punto.
										// Si estoy en vértice
				var ret = new HashSet<ContinuumPoint<T>> (Universe.Points.Where (x => x.EndPoints.Contiene (orig)));
				return ret;
			}
			return Universe.PointsInEdge (A, B);
		}

		/// <summary>
		/// Advances this towards a destination.
		/// </summary>
		/// <returns><c>true</c>, if reaches the destination <c>false</c> otherwise.</returns>
		/// <param name="destination">Destination</param>
		/// <param name="distance">Distance to advance.</param>
		public bool AdvanceTowards (T destination, float distance)
		{
			float Ref = distance;
			return AdvanceTowards (destination, ref Ref);
		}

		/// <summary>
		/// Advances this through a specific <see cref="Path{T}"/>.
		/// </summary>
		/// <returns><c>true</c> if the path is completed; <c>false</c> otherwise.</returns>
		/// <param name="path">path</param>
		/// <param name="dist">Distance</param>
		public bool AdvanceTowards (Path<T> path, float dist)
		{
			foreach (var r in new List<IStep<T>> (path.Pasos))
			{
				if (!AdvanceTowards (r.Destination, ref dist))
					return false;

				// Si llega aquí es que avanzó exactamente hasta un nodo hasta este momento.
				// Hay que eliminar el paso de la ruta y actualizar Inicial.
				path.RemoveFirstStep ();
			}
			return true;
		}

		/// <summary>
		/// Avanza towards a node.
		/// </summary>
		/// <returns><c>true</c>, si reached the node, <c>false</c> otherwise.</returns>
		/// <param name="destino">Destination</param>
		/// <param name="dist">Distancia</param>
		public bool AdvanceTowards (ContinuumPoint<T> destino, ref float dist)
		{
			if (!OnSameInterval (destino))
				throw new InvalidOperationException ();

			var relRestante = DistanciaAExtremo (A) - destino.DistanciaAExtremo (A);
			var absRestante = Math.Abs (relRestante);
			var avance = Math.Min (dist, absRestante);
			dist -= avance;

			if (relRestante < 0)
				AdvanceTowards (B, ref avance);
			else
				AdvanceTowards (A, ref avance);
			return Universe.PointComparer.Equals (destino, this);
		}

		/// <summary>
		/// Gets a value indicating whether this and specified point are geometrically the same point.
		/// </summary>
		/// <param name="other">Other point</param>
		public bool Match (ContinuumPoint<T> other)
		{
			try
			{
				if (other == null)
					return false;
				if (AtNode)
					return (other.AtNode && Universe.NodeComparer.Equals (A, other.A));
				var ret =
					(Universe.NodeComparer.Equals (A, other.A) &&
					Universe.NodeComparer.Equals (B, other.B) &&
					Loc == other.Loc)

					||

					(Universe.NodeComparer.Equals (A, other.B) &&
					Universe.NodeComparer.Equals (B, other.A) &&
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

		/// <summary>
		/// Inverts, if possible, the values of A and B without modifying the funcional position.
		/// </summary>
		protected void Invert ()
		{
			if (!AtNode)
			{
				T nodoTmp = A;
				A = B;
				B = nodoTmp;
				Loc = Aloc;
			}
		}


		string Mostrar ()
		{
			return string.Format (
				"[ContinuoPunto: _loc={0}, Universo={1}, A={2}, B={3}, Loc={4}, Aloc={5}, EnOrigen={6}, Extremos={7}]",
				_loc,
				Universe,
				A,
				B,
				Loc,
				Aloc,
				AtNode,
				EndPoints);
		}

		bool AdvanceTowards (T destino, ref float dist)
		{
			var restante = DistanciaAExtremo (destino);

			if (restante > dist) // No llega
			{
				using (var anterior = Clone ())
				{
					if (Universe.NodeComparer.Equals (destino, A))
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

					CheckCollisions (anterior);
				}
				return false;
			}

			//using var otroAnt = 
			dist = dist - restante;
			AlDesplazarse?.Invoke ();
			AlLlegarANodo?.Invoke ();
			using (var anterior = Clone ())
			{
				SetOnNode (destino);
				CheckCollisions (anterior);
			}
			return true;
		}

		void CheckCollisions (ContinuumPoint<T> anterior)
		{
			var extremosBase = new List<T> (EndPoints.AsSet ().Intersect (anterior.EndPoints.AsSet ()));
			var extremoBase = extremosBase[0];


			var n0 = anterior.DistanciaAExtremo (extremoBase);
			var n1 = DistanciaAExtremo (extremoBase);
			var maxDist = Math.Max (n0, n1);
			var minDist = Math.Min (n0, n1);
			var puntosIntervalo = Neighborhood ();
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

		void SetOnNode (T punto)
		{
			A = punto;
			B = default (T);
			Loc = 0;
		}

		void IDisposable.Dispose () => Universe.Points.Remove (this);

		object ICloneable.Clone () => Clone ();

		/// <summary>
		/// Occurs when this point's position changes.
		/// </summary>
		public event Action AlDesplazarse;

		/// <summary>
		/// Ocurrs when this point passes through a node.
		/// </summary>
		public event Action AlLlegarANodo;

		/// <summary>
		/// Ocurrs when this point collides another. 
		/// </summary>
		public event Action<ContinuumPoint<T>> AlColisionar;

		float _loc;
		T _a;

		/// <summary>
		/// Determines whether two observable points are in the same interval of a graph.
		/// </summary>
		public static bool OnSameInterval (ContinuumPoint<T> punto1, ContinuumPoint<T> punto2)
		{
			// TODO: Mode this to a continuum graph;
			// and set to obsolete
			if (punto1 == null || punto2 == null)
				return false;
			return punto1.OnSameInterval (punto2);
		}
	}
}

