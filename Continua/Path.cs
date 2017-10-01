using System;
using System.Collections.Generic;
using System.Linq;
using CE.Collections;

namespace Graficas.Continua
{
	/// <summary>
	/// A path for <see cref="ContinuumPoint{T}"/>.
	/// </summary>
	[Serializable]
	public class Path<T> : Rutas.Ruta<T>, IClass<ContinuumPoint<T>>
	{
		/// <summary>
		/// Gets the origin node.
		/// </summary>
		public new ContinuumPoint<T> NodoInicial { get; }

		/// <summary>
		/// Gets the destination.
		/// </summary>
		public new ContinuumPoint<T> NodoFinal { get; private set; }

		/// <summary>
		/// Gets the total weight of this route
		/// </summary>
		public new float Longitud
		{
			get
			{
				var lbase = Step.Sum (z => z.Weight);
				return lbase + NodoInicial.DistanciaAExtremo (base.NodoInicial) + NodoFinal.DistanciaAExtremo (base.NodoFinal);
			}
		}

		/// <summary>
		/// Gets the step count.
		/// </summary>
		public new int NumPasos => base.NumPasos + 2;

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="origin">Origin</param>
		public Path (ContinuumPoint<T> origin)
		{
			NodoInicial = origin;
			NodoFinal = origin;
		}

		/// <summary>
		/// Removes the first step
		/// </summary>
		public void RemoveFirstStep ()
		{
			Step.RemoveAt (0);
		}

		/// <summary>
		/// Concatenates a step on the end.
		/// </summary>
		public void ConcatFinal (ContinuumPoint<T> final)
		{
			NodoFinal = final;
		}


		/// <summary>
		/// Revisa si un punto dado pertenece a esta ruta.
		/// </summary>
		/// <param name="punto">Punto.</param>
		public bool Contains (ContinuumPoint<T> punto)
		{
			// Hay de tres:
			// 0) Está en el semiintervalo inicial
			// 1) Está en el semiintervalo final
			// 2) Está en un intervalo intermedio

			// 0)
			if (NodoInicial.OnSameInterval (punto))
			{
				T MyA = punto.A;
				if (NodoInicial.DistanciaAExtremo (MyA) <= punto.Loc)
					return true;
			}

			// 2)
			foreach (var x in Pasos)
			{
				if (punto.OnEdge (x.Origin, x.Destination))
					return true;
			}

			// 1)
			if (NodoFinal.OnSameInterval (punto))
			{
				T MyB = punto.B;
				if (NodoFinal.DistanciaAExtremo (MyB) < punto.Aloc)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Gets a new list of points in this path.
		/// </summary>
		public List<ContinuumPoint<T>> PointsInGraph (GraphContinuum<T> gr)
		{
			return gr.Points.FindAll (Contains);
		}
	}
}