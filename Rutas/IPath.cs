using System.Collections.Generic;
using CE.Graph.Edges;
using CE.Collections;

namespace CE.Graph.Rutas
{
	/// <summary>
	/// A graph path.
	/// </summary>
	public interface IPath<T> : IClass<T>
	{
		/// <summary>
		/// Gets the starting node.
		/// </summary>
		T StartNode { get; }

		/// <summary>
		/// Gets the finish node.
		/// </summary>
		/// <value>The nodo final.</value>
		T EndNode { get; }

		/// <summary>
		/// Gets the step count.
		/// </summary>
		int StepCount { get; }

		/// <summary>
		/// Gets the length.
		/// </summary>
		float Length { get; }

		/// <summary>
		/// Concatenates a step.
		/// </summary>
		/// <param name="step">Step to concatenate.</param>
		void Concat (IStep<T> step);

		/// <summary>
		/// Concatenates a path.
		/// </summary>
		/// <param name="path">Path to concatenate.</param>
		void Concat (IPath<T> path);

		/// <summary>
		/// Enumerates the steps.
		/// </summary>
		IEnumerable<IStep<T>> Steps { get; }
	}
}