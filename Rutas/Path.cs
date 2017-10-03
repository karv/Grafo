using System;
using System.Collections.Generic;
using System.Linq;
using Graficas.Edges;

namespace Graficas.Rutas
{
	/// <summary>
	/// A path in a graph.
	/// </summary>
	[Serializable]
	public class Path<T> : IPath<T>
	{
		/// <summary>
		/// Enumerates the steps.
		/// </summary>
		public IStep<T>[] Pasos => Step.ToArray ();

		/// <summary>
		/// Gets the path length.
		/// </summary>
		public float Length
		{
			get
			{
				float ret = 0;
				foreach (var x in Step)
					ret += x.Weight;
				return ret;
			}
		}

		/// <summary>
		/// Gets the starting node.
		/// </summary>
		public T StartNode => Step.Count == 0 ? _virtualInicial : Step[0].Origin;

		/// <summary>
		/// Gets the finish node.
		/// </summary>
		/// <value>The nodo final.</value>
		public T EndNode => StepCount < 1 ? StartNode : Step[StepCount - 1].Destination;

		/// <summary>
		/// Gets the step count.
		/// </summary>
		public int StepCount => Step.Count;

		/// <summary>
		/// Gets a value that determines whether this path is empty.
		/// </summary>
		public bool IsEmpty => StepCount == 0;

		/// <summary>
		/// Step list.
		/// </summary>
		protected IList<IStep<T>> Step { get; } = new List<IStep<T>> ();

		IEnumerable<IStep<T>> IPath<T>.Steps => Pasos;

		/// <summary>
		/// </summary>
		public Path ()
		{
		}

		/// <param name="origin">Initial node.</param>
		/// <param name="destination">Final node.</param>
		/// <param name="weigth">Step weigth</param>
		public Path (T origin, T destination, float weigth = 1)
		{
			Step.Add (new Step<T> (origin, destination, weigth));
		}

		/// <summary>
		/// Initializes a clone of the specified path.
		/// </summary>
		public Path (IPath<T> ruta)
		{
			if (ruta == null)
				throw new ArgumentNullException ();
			_virtualInicial = ruta.StartNode;
			foreach (var x in ruta.Steps)
				Step.Add (new Step<T> (x.Origin, x.Destination, x.Weight));
		}

		/// <summary>
		/// </summary>
		public override string ToString ()
		{
			return StepCount + " - " + string.Join (" ", Step.Select (z => z.ToString ()));
		}

		/// <summary>
		/// Concatenates a step.
		/// </summary>
		public void Concat (IStep<T> paso)
		{
			if (paso == null)
				throw new NullReferenceException ();
			if (StepCount == 0)
				Step.Add (new Step<T> (paso));
			else
			{
				if (paso.Contains (EndNode))
					Step.Add (new Step<T> (paso));
				else
					throw new InvalidPathOperationException ();
			}
		}

		/// <summary>
		/// Cocatenates an edge.
		/// </summary>
		public void Concat (IEdge<T> step, T orig)
		{
			var p = new Step<T> (orig, step.Antipode ((orig)));
			Concat (p);
		}

		/// <summary>
		/// Concatenates a path.
		/// </summary>
		/// <param name="path">Path to concatenate.</param>
		public void Concat (IPath<T> path)
		{
			if (path.StepCount == 0)
				return;
			if (StepCount > 0 && !EndNode.Equals (path.StartNode))
				throw new InvalidPathOperationException ();

			foreach (var paso in path.Steps)
			{
				Step.Add (new Step<T> (paso));
			}
		}

		T _virtualInicial;

		/// <summary>
		/// Determines whether the specified path is nul or empty.
		/// </summary>
		public static bool NullOrEmpty (Path<T> r)
		{
			// Analysis disable ConstantNullCoalescingCondition
			return r?.IsEmpty ?? true;
			// Analysis restore ConstantNullCoalescingCondition
		}

		/// <summary>
		/// Gets the empty path.
		/// </summary>
		public static Path<T> Empty => new Path<T> ();
	}
}