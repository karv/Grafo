using System;
using System.Collections.Generic;
using System.Linq;
using CE.Collections;

namespace CE.Graph.Continua
{
	/// <summary>
	/// A path for <see cref="ContinuumPoint{T}"/>.
	/// </summary>
	[Serializable]
	public class Path<T> : Rutas.Path<T>, IClass<ContinuumPoint<T>>
	{
		/// <summary>
		/// Gets the origin node.
		/// </summary>
		public new ContinuumPoint<T> StartNode { get; }

		/// <summary>
		/// Gets the destination.
		/// </summary>
		public new ContinuumPoint<T> EndNode { get; private set; }

		/// <summary>
		/// Gets the total weight of this route
		/// </summary>
		public new float Length
		{
			get
			{
				var lbase = Step.Sum (z => z.Weight);
				return lbase + StartNode.DistanciaAExtremo (base.StartNode) + EndNode.DistanciaAExtremo (base.EndNode);
			}
		}

		/// <summary>
		/// Gets the step count.
		/// </summary>
		public new int StepCount => base.StepCount + 2;

		/// <summary>
		/// Initializes a new instance of the class.
		/// </summary>
		/// <param name="origin">Origin</param>
		public Path (ContinuumPoint<T> origin)
		{
			StartNode = origin;
			EndNode = origin;
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
			EndNode = final;
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
			if (StartNode.OnSameInterval (punto))
			{
				T MyA = punto.A;
				if (StartNode.DistanciaAExtremo (MyA) <= punto.Loc)
					return true;
			}

			// 2)
			foreach (var x in Pasos)
			{
				if (punto.OnEdge (x.Origin, x.Destination))
					return true;
			}

			// 1)
			if (EndNode.OnSameInterval (punto))
			{
				T MyB = punto.B;
				if (EndNode.DistanciaAExtremo (MyB) < punto.Aloc)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Gets a new list of points in this path.
		/// </summary>
		public List<ContinuumPoint<T>> PointsInGraph (ContinuumGraph<T> gr)
		{
			return gr.Points.FindAll (Contains);
		}
	}
}