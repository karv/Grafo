using System;
using CE.Graph.Grafo;
using CE.Graph.Nodos;
using CE.Graph.Rutas;

namespace Graficas.Rutas
{
	/// <summary>
	/// Collection of optimal path getter.
	/// </summary>
	public class OnDemandOptimalPathCollection<TNode>
	{
		public IGraph<TNode> PathFindingGraph { get; }

		public OnDemandOptimalPathCollection (IGraph<TNode> pathFindingGraph)
		{
			PathFindingGraph = pathFindingGraph;
		}

		public IPath<TNode> GetBestPath (INode<TNode> startNode, INode<TNode> endNode)
		{
			if (_optCollection.TryGetPath (startNode.Item, endNode.Item, out var optPath))
				return optPath;

			// Compute the route
			throw new NotImplementedException ();
		}

		readonly PathCollection<TNode> _optCollection = new PathCollection<TNode> ();
	}
}