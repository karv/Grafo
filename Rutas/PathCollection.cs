using System.Collections.ObjectModel;
using CE.Graph.Rutas;
using CE.Graph.Nodos;

namespace Graficas.Rutas
{
	public class PathCollection<T> : Collection<IPath<T>>
	{
		public bool TryGetPath (T startNode, T endNode, out IPath<T> optPath)
		{
			optPath = null;
			// TODO: Make it binary searchable
			foreach (var path in this)
			{
				// TODO: custom equal
				if (Equals (path.StartNode, startNode) && Equals (path.EndNode, endNode))
				{
					optPath = path;
					return true;
				}
			}
			return false;
		}

		public bool TryGetPath (INode<T> startNode, INode<T> endNode, out IPath<T> optPath)
		{
			optPath = null;
			// TODO: Make it binary searchable
			foreach (var path in this)
			{
				// TODO: custom equal
				if (Equals (path.StartNode, startNode.Item) && Equals (path.EndNode, endNode.Item))
				{
					optPath = path;
					return true;
				}
			}
			return false;
		}
	}
}