//
//  Continuo.cs
//
//  Author:
//       Edgar Carballo <karvayoEdgar@gmail.com>
//
//  Copyright (c) 2015 edgar
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using Graficas;

namespace Graficas.Continuo
{
	/// <summary>
	/// Representa un continuo producido por una IGrafica
	/// </summary>
	public class Continuo<T>
	{
		/// <summary>
		/// Representa un punto en un continuo.
		/// </summary>
		public class ContinuoPunto : IEquatable<ContinuoPunto>, IDisposable
		{
			#region General

			public override string ToString()
			{
				return EnOrigen() ? A.ToString() : string.Format("[{0}, {1}]@{2}", A, B, Loc);
			}

			#endregion

			#region Ctor

			public ContinuoPunto(Continuo<T> universo, T nodo) : this(universo)
			{
				A = nodo;
			}

			public ContinuoPunto(Continuo<T> universo)
			{
				Universo = universo;
				Universo.Puntos.Add(this);
			}

			#endregion

			#region Posición

			float _loc;

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
						FromGrafica(A);
					if (Aloc < 0)
						FromGrafica(B);
				}
			}

			/// <summary>
			/// Distancia hasta B   [A --- aquí --[aloc]-- B]
			/// </summary>
			public float Aloc
			{
				get
				{
					return Universo.GráficaBase[A, B] - Loc;
				}
			}

			#endregion

			#region Topología

			protected readonly Continuo<T> Universo;

			public bool EnOrigen()
			{
				return Loc == 0;
			}

			bool enDestino()
			{
				return Aloc == 0;
			}

			/// <summary>
			/// Revisa si este punto está en dos vértices contiguos de una gráfica.
			/// </summary>
			/// <returns><c>true</c>, si el punto está en el intervalo, <c>false</c> otherwise.</returns>
			/// <param name="p1">Un extremo del intervalo.</param>
			/// <param name="p2">El otro extramo del intervalo.</param>
			public bool EnIntervaloInmediato(T p1, T p2)
			{
				if (EnOrigen())
				{
					return A.Equals(p1) || A.Equals(p2);
				}
				return (A.Equals(p1) && B.Equals(p2)) || (A.Equals(p2) && B.Equals(p1));
			}

			/// <summary>
			/// Devuelve la lista de terrenos contiguos a esta pseudoposición.
			/// </summary>
			/// <returns>Una nueva lista.</returns>
			public List<ContinuoPunto> Vecindad()
			{
				if (EnOrigen())
				{
					T orig = A; // Posición de este punto.
					var ret = new List<ContinuoPunto>();
					// Si estoy en terreno
					foreach (var x in Universo.GráficaBase.Vecinos(orig))
					{
						foreach (var y in Universo [orig, x])
						{
							if (!ret.Contains(y))
								ret.Add(y);
						}
					}
					return ret;
				}
				return Universo[A, B];
			}

			#endregion

			#region Conversores

			/// <summary>
			/// Pune a este punto en un punto de la gráfica.
			/// </summary>
			/// <param name="punto">Punto.</param>
			public void FromGrafica(T punto)
			{
				A = punto;
				B = default(T);
				Loc = 0;
			}

			#endregion

			#region IEquatable implementation

			// Analysis disable MemberHidesStaticFromOuterClass
			public bool Equals(ContinuoPunto other)
			// Analysis restore MemberHidesStaticFromOuterClass
			{
				if (EnOrigen())
					return (A.Equals(other.A) && other.Loc == 0);
				return (A.Equals(other.A) && B.Equals(other.B) && Loc == other.Loc) ||
				(A.Equals(other.B) && B.Equals(other.A) && Loc == other.Aloc);
			}

			#endregion

			#region IDisposable implementation

			void IDisposable.Dispose()
			{
				Universo.Puntos.Remove(this);
			}

			#endregion
		}

		public readonly IGraficaPeso<T> GráficaBase;
		public readonly List<ContinuoPunto> Puntos = new List<ContinuoPunto>();

		public Continuo(IGraficaPeso<T> grafica)
		{
			GráficaBase = grafica;
		}

		/// <summary>
		/// Devuelve una nueva lista de los puntos que hay en dos nodos consecutivos.
		/// </summary>
		/// <param name="p1">P1.</param>
		/// <param name="p2">P2.</param>
		public List<ContinuoPunto> this [T p1, T p2]
		{
			get
			{
				return Puntos.FindAll(x => x.EnIntervaloInmediato(p1, p2));
			}
		}
	}

}