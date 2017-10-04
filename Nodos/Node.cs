using System;
using System.Collections.Generic;

namespace CE.Graph.Nodos
{
	/// <summary>
	/// Representa un nodo que vincula a su vecindad
	/// </summary>
	[Serializable]
	public class Node<T> : INode<T>
	{
		/// <summary>
		/// Gets the name of the node.
		/// </summary>
		public T Item { get; }

		/// <summary>
		/// Vecindad del nodo
		/// </summary>
		public List<Node<T>> Neighborhood { get; }

		IEnumerable<INode<T>> INode<T>.Neighborhood => Neighborhood;

		/// <param name="obj">Name</param>
		public Node (T obj)
		{
			Item = obj;
			Neighborhood = new List<Node<T>> ();
		}

		/// <summary>
		/// </summary>
		public override string ToString ()
		{
			return Item.ToString ();
		}
	}
}