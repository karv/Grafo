using System;
using System.Collections.Generic;
using System.Linq;
using Graficas.Rutas;
using Graficas.Edges;

namespace Graficas.Grafo
{
	/// <summary>
	/// Modela una IMulticolGrafica que es la superposición de varias IGráficas
	/// </summary>
	public class MulticolGrafica<TNodo, TColor> : IMulticolGrafo<TNodo, TColor>
		where TNodo : IEquatable<TNodo>
	{
		/// <summary>
		/// La asignación de color -> Gráfica
		/// </summary>
		readonly Dictionary<TColor, IGrafo<TNodo>> _asignación = new Dictionary<TColor, IGrafo<TNodo>> ();

		#region IGrafica

		/// <summary>
		/// Calcula el subgrafo generado por un subconjutno de Nodos
		/// </summary>
		/// <param name="conjunto">Conjunto de nodos para calcular el subgrafo</param>
		public IGrafo<TNodo> Subgrafo (IEnumerable<TNodo> conjunto)
		{
			throw new NotImplementedException ();
		}

		ICollection<IEdge<TNodo>> IGrafo<TNodo>.Aristas ()
		{
			throw new NotImplementedException ();
		}

		/// <summary>
		/// Devuelve los vecinos de cualquier color de un nodo dado
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		public ICollection<TNodo> Vecinos (TNodo nodo)
		{
			ISet<TNodo> ret = new HashSet<TNodo> ();

			foreach (var color in _asignación.Keys)
			{
				ret.UnionWith (_asignación[color].Vecinos (nodo));
			}
			return ret;
		}

		/// <summary>
		/// Convierte una sucesión consistente de nodos a una ruta
		/// </summary>
		/// <returns>The ruta.</returns>
		/// <param name="seq">Sucesión consistente.</param>
		public IRuta<TNodo> ToRuta (IEnumerable<TNodo> seq)
		{
			throw new NotImplementedException ();
			/*
			IRuta<TNodo> ret = new Ruta<TNodo> ();
			TNodo [] arr = seq.ToArray ();
			IArista<TNodo> mejorPaso;
			for (int i = 0; i < arr.Count () - 1; i++)
			{
				// Encontrar mejor paso
				mejorPaso = new Paso<TNodo> (arr [i], arr [i + 1], float.PositiveInfinity);

				foreach (var x in _asignación)
				{
					if (mejorPaso.Peso > 1 &&
					    x.Value.Vecinos (arr [i]).Contains (arr [i + 1]))
						mejorPaso = new Paso<TNodo> (mejorPaso.Origen, mejorPaso.Destino, 1);
				}
				ret.Concat (mejorPaso);
			}
			return ret;
			*/
		}

		IEdge<TNodo> IGrafo<TNodo>.this[TNodo desde, TNodo hasta]
		{
			get
			{
				throw new NotImplementedException ();
			}
		}

		/// <summary>
		/// Elimina nodos y aristas.
		/// </summary>
		public void Clear ()
		{
			_asignación.Clear ();
		}

		#endregion

		#region IMulticolGrafica implementation


		bool ExisteArista (TNodo desde, TNodo hasta)
		{
			return _asignación.Any (z => z.Value[desde, hasta].Exists);
		}

		/// <summary>
		/// Devuelve los colores existentes
		/// </summary>
		/// <returns>The arista.</returns>
		/// <param name="aris">Aris.</param>
		public IEnumerable<TColor> ColoresArista (IEdge<TNodo> aris)
		{
			throw new NotImplementedException ();
		}

		/// <summary>
		/// Agrega un color
		/// </summary>
		/// <param name="color">Nombre del color</param>
		/// <param name="grafo">Grafo que modela este color</param>
		public void AgregaColor (TColor color, IGrafo<TNodo> grafo)
		{
			if (_asignación.ContainsKey (color))
				throw new ColorDuplicadoException ("Ya existe el color " + color);
			_asignación.Add (color, grafo);
		}

		/// <summary>
		/// Devuelve el grafo de un color dado.
		/// </summary>
		/// <returns>The color.</returns>
		/// <param name="color">Color.</param>
		public IGrafo<TNodo> GrafoColor (TColor color)
		{
			IGrafo<TNodo> ret;
			if (_asignación.TryGetValue (color, out ret))
				return ret;
			throw new Exception (string.Format ("Color {0} no existe.", color));
		}

		/// <summary>
		/// Vecinos de un nodo y color específicos
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		/// <param name="color">Color.</param>
		public ICollection<TNodo> Vecinos (TNodo nodo, TColor color)
		{
			IGrafo<TNodo> graf;
			return _asignación.TryGetValue (color, out graf) ? graf.Vecinos (nodo) : new TNodo[0];
		}

		/// <summary>
		/// Devuelve los nodos de la gráfica
		/// </summary>
		/// <value>The nodos.</value>
		public ICollection<TNodo> Nodos
		{
			get
			{
				var ret = new List<TNodo> ();
				foreach (var x in _asignación)
				{
					foreach (var nod in x.Value.Nodos)
					{
						if (!ret.Contains (nod))
							ret.Add (nod);
					}
				}
				return ret;
			}
		}

		#endregion
	}
}