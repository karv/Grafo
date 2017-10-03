using System;
using System.Collections.Generic;
using Graficas.Edges;
using Graficas.Nodos;

namespace Graficas.Rutas
{
	/// <summary>
	/// A path that preserves references.
	/// </summary>
	public class ReferencedPath<T> : IPath<T>
	{
		/// <summary>
		/// Gets the length.
		/// </summary>
		public float Length => StepCount;

		/// <summary>
		/// Gets the step count.
		/// </summary>
		public int StepCount => _pasos.Count - 1;


		/// <summary>
		/// Enumerates the steps.
		/// </summary>
		public IEnumerable<IStep<T>> Steps
		{
			get
			{
				for (int i = 0; i < StepCount; i++)
					yield return new Step<T> (_pasos[i].Item, _pasos[i + 1].Item);
			}
		}

		/// <summary>
		/// Gets the initial node.
		/// </summary>
		public Node<T> StartNode => _pasos[0];

		/// <summary>
		/// Gets the final node.
		/// </summary>
		public Node<T> EndNode => _pasos[_pasos.Count - 1];

		T IPath<T>.StartNode => StartNode.Item;

		T IPath<T>.EndNode => EndNode.Item;

		/// <param name="inicial">Initial node.</param>
		public ReferencedPath (Node<T> inicial)
		{
			_pasos = new List<Node<T>> { inicial };
		}

		/// <param name="steps">Steps.</param>
		public ReferencedPath (IEnumerable<Node<T>> steps)
		{
			_pasos = new List<Node<T>> (steps);
		}

		/// <summary>
		/// Gets the reversed path.
		/// </summary>
		public ReferencedPath<T> Reverse ()
		{
			var ret = new ReferencedPath<T> (EndNode) { _pasos = new List<Node<T>> (_pasos) };
			ret._pasos.Reverse ();
			return ret;
		}

		/// <summary>
		/// Concatenates this with a step.
		/// </summary>
		/// <param name="step">Concatenating step.</param>
		public void Concat (IStep<T> step)
		{
			if (EndNode.Item.Equals (step.Origin))
			{
				var agrega = EndNode.Neighborhood.Find (x => x.Item.Equals (step.Destination));
				if (agrega == null)
					throw new InvalidOperationException ();
				_pasos.Add (agrega);
				return;
			}
			throw new InvalidOperationException ("End points do not match.");
		}

		/// <summary>
		/// Concatenates two paths.
		/// </summary>
		public void Concat (IPath<T> path)
		{
			if (!EndNode.Item.Equals (path.StartNode))
				throw new InvalidOperationException ("End points do not match.");
			foreach (var x in path.Steps)
				Concat (x);
		}

		/// <summary>
		/// Concatenates with a node.
		/// </summary>
		/// <param name="nodo">Nodo al final</param>
		public void Concat (T nodo)
		{
			var agrega = EndNode.Neighborhood.Find (x => x.Item.Equals (nodo));
			if (agrega == null)
				throw new InvalidOperationException ();
			_pasos.Add (agrega);
		}

		List<Node<T>> _pasos;

	}
}