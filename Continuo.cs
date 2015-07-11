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
using System.Linq;

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
			#region Ctor

			public ContinuoPunto(Continuo<T> universo, T A) : this(universo)
			{
				_origen = A;
			}

			public ContinuoPunto(Continuo<T> universo)
			{
				_universo = universo;
				_universo.Puntos.Add(this);
			}

			#endregion

			public override string ToString()
			{
				if (this.enOrigen())
					return A.ToString();
				else
					return string.Format("[{0}, {1}]@{2}", A, B, loc);
				return string.Format("[ContinuoPunto: A={0}, B={1}, loc={2}, aloc={3}]", A, B, loc, aloc);
			}

			#region Posición

			T _origen;
			T _destino;
			float _loc;

			/// <summary>
			/// Posición A
			/// </summary>
			public T A
			{
				get { return _origen; }
				set { _origen = value; }
			}

			/// <summary>
			/// Posición B
			/// </summary>
			public T B
			{
				get { return _destino; }
				set { _destino = value; }
			}

			/// <summary>
			/// Distancia hasta A   [A --[loc]-- aquí --- B]
			/// </summary>
			public float loc
			{
				get { return _loc; }
				set
				{ 
					_loc = value; 
					if (_loc < 0)
						FromGrafica(_origen);
					if (aloc < 0)
						FromGrafica(_destino);
				}
			}

			/// <summary>
			/// Distancia hasta B   [A --- aquí --[aloc]-- B]
			/// </summary>
			public float aloc
			{
				get
				{
					return _universo._grafica.Peso(A, B) - loc;
				}
			}

			#endregion

			#region Topología

			readonly Continuo<T> _universo;

			public bool enOrigen()
			{
				return loc == 0;
			}

			bool enDestino()
			{
				return aloc == 0;
			}

			/// <summary>
			/// Revisa si este punto está en dos vértices contiguos de una gráfica.
			/// </summary>
			/// <returns><c>true</c>, si el punto está en el intervalo, <c>false</c> otherwise.</returns>
			/// <param name="p1">Un extremo del intervalo.</param>
			/// <param name="p2">El otro extramo del intervalo.</param>
			public bool enIntervaloInmediato(T p1, T p2)
			{
				if (B == null)
				{
					return A.Equals(p1) || A.Equals(p2);
				}
				return (A.Equals(p1) && B.Equals(p2)) || (A.Equals(p2) && B.Equals(p1));
			}

			/// <summary>
			/// Devuelve la lista de terrenos contiguos a esta pseudoposición.
			/// </summary>
			/// <returns>Una nueva lista.</returns>
			public List<ContinuoPunto> getVecindad()
			{
				if (enOrigen())
				{
					T orig = A; // Posición de este punto.
					List<ContinuoPunto> ret = new List<ContinuoPunto>();
					// Si estoy en terreno
					foreach (var x in _universo._grafica.Vecinos(orig))
					{
						foreach (var y in this._universo [orig, x])
						{
							if (!ret.Contains(y))
								ret.Add(y);
						}
					}
					return ret;
				}
				else
				{
					return _universo[A, B];
				}
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
				loc = 0;
			}

			#endregion

			#region IEquatable implementation

			public bool Equals(ContinuoPunto other)
			{
				if (B == null)
					return (A.Equals(other.A) && other.loc == 0);
				return (A.Equals(other.A) && B.Equals(other.B) && loc == other.loc) ||
				(A.Equals(other.B) && B.Equals(other.A) && loc == other.aloc);
			}

			#endregion

			#region IDisposable implementation

			void IDisposable.Dispose()
			{
				_universo.Puntos.Remove(this);
			}

			#endregion
		}

		public readonly IGraficaPeso<T> _grafica;
		public readonly List<ContinuoPunto> Puntos = new List<ContinuoPunto>();

		public Continuo(IGraficaPeso<T> grafica)
		{
			_grafica = grafica;
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
				return Puntos.FindAll(x => x.enIntervaloInmediato(p1, p2));
			}
		}
	}

}