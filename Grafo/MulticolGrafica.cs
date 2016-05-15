using System;
using System.Collections.Generic;
using System.Linq;
using Graficas.Rutas;
using Graficas.Aristas;
using Graficas.Exception;

namespace Graficas.Grafo
{
	/// <summary>
	/// Modela una IMulticolGrafica que es la superposición de varias IGráficas
	/// </summary>
	public class MulticolGrafica<TNodo, TColor> : IMulticolGrafo<TNodo, TColor>
		where TNodo :IEquatable<TNodo>
	{
		/// <summary>
		/// La asignación de color -> Gráfica
		/// </summary>
		readonly Dictionary <TColor, ILecturaGrafo<TNodo>> _asignación = new Dictionary<TColor, ILecturaGrafo<TNodo>> ();

		#region IGrafica

		/// <summary>
		/// Calcula el subgrafo generado por un subconjutno de Nodos
		/// </summary>
		/// <param name="conjunto">Conjunto de nodos para calcular el subgrafo</param>
		public ILecturaGrafo<TNodo> Subgrafo (IEnumerable<TNodo> conjunto)
		{
			throw new NotImplementedException ();
		}

		ICollection<IArista<TNodo>> ILecturaGrafo<TNodo>.Aristas ()
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
				ret.UnionWith (_asignación [color].Vecinos (nodo));
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
			IRuta<TNodo> ret = new Ruta<TNodo> ();
			TNodo [] arr = seq.ToArray ();
			Paso<TNodo> mejorPaso;
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
		}

		bool ILecturaGrafo<TNodo>.this [TNodo desde, TNodo hasta]
		{ get { return ExisteArista (desde, hasta); } }

		bool ILecturaGrafo<TNodo>.ExisteArista (TNodo origen, TNodo destino)
		{
			foreach (var c in _asignación)
			{
				if (c.Value.ExisteArista (origen, destino))
					return true;
			}
			return false;
		}


		#endregion

		#region IMulticolGrafica implementation


		bool ExisteArista (TNodo desde, TNodo hasta)
		{
			return _asignación.Any (z => z.Value [desde, hasta]);
		}

		/// <summary>
		/// Devuelve los colores existentes
		/// </summary>
		/// <returns>The arista.</returns>
		/// <param name="aris">Aris.</param>
		public IEnumerable<TColor> ColoresArista (IArista<TNodo> aris)
		{
			var ret = new List<TColor> ();
			foreach (var gr in _asignación)
			{
				if (gr.Value [aris.Origen, aris.Destino])
					ret.Add (gr.Key);
			}
			return ret;
		}

		/// <summary>
		/// Agrega un color
		/// </summary>
		/// <param name="color">Nombre del color</param>
		/// <param name="grafo">Grafo que modela este color</param>
		public void AgregaColor (TColor color, ILecturaGrafo<TNodo> grafo)
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
		public ILecturaGrafo<TNodo> GrafoColor (TColor color)
		{
			ILecturaGrafo<TNodo> ret;
			if (_asignación.TryGetValue (color, out ret))
				return ret;
			throw new System.Exception (string.Format ("Color {0} no existe.", color));
		}

		/// <summary>
		/// Vecinos de un nodo y color específicos
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		/// <param name="color">Color.</param>
		public ICollection<TNodo> Vecinos (TNodo nodo, TColor color)
		{
			ILecturaGrafo<TNodo> graf;
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