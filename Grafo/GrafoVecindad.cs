using System.Collections.Generic;

namespace Graficas.Grafo
{
	public class GrafoVecindad<T>
	{
		#region Ctor

		public GrafoVecindad (IEqualityComparer<T> comparador = null)
		{
			Comparador = comparador ?? EqualityComparer<T>.Default;
			Nodos = new HashSet<T> (Comparador);
			Vecindad = new Dictionary<T, HashSet<T>> (Comparador);
			inicializaDiccionario ();
		}

		void inicializaDiccionario ()
		{
			foreach (var x in Nodos)
				Vecindad.Add (x, new HashSet<T> (Comparador));
		}

		#endregion

		#region Interno

		HashSet<T> Nodos { get; }

		public IEqualityComparer<T> Comparador { get; }

		protected Dictionary<T, HashSet<T>> Vecindad { get; }

		#endregion
	}
}