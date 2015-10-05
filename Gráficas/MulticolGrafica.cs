using System;
using System.Collections.Generic;
using System.Linq;
using Graficas.Rutas;

namespace Graficas
{
	/// <summary>
	/// Modela una IMulticolGrafica que es la superposición de varias IGráficas
	/// </summary>
	public class MulticolGrafica<TNodo, TColor> : IMulticolGrafica<TNodo, TColor>
		where TNodo :IEquatable<TNodo>
	{
		/// <summary>
		/// La asignación de color -> Gráfica
		/// </summary>
		readonly Dictionary <TColor, IGrafica<TNodo>> _asignación = new Dictionary<TColor, IGrafica<TNodo>>();

		/// <summary>
		/// Color default
		/// </summary>
		TColor defColor;

		public MulticolGrafica()
		{
			
		}

		/// <param name="defColor">Color default</param>
		public MulticolGrafica(TColor defColor)
		{
			this.defColor = defColor;
			AgregaColor(defColor);
		}

		#region IGrafica

		ICollection<IArista<TNodo>> IGrafica<TNodo>.Aristas()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Devuelve los vecinos de cualquier color de un nodo dado
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		public ICollection<TNodo> Vecinos(TNodo nodo)
		{
			ISet<TNodo> ret = new HashSet<TNodo>();

			foreach (var color in _asignación.Keys)
			{
				ret.UnionWith(_asignación[color].Vecinos(nodo));
			}
			return ret;
		}

		public IRuta<TNodo> ToRuta(IEnumerable<TNodo> seq)
		{
			IRuta<TNodo> ret = new Ruta<TNodo>();
			TNodo[] arr = seq.ToArray();
			Paso<TNodo> mejorPaso;
			for (int i = 0; i < arr.Count() - 1; i++)
			{
				// Encontrar mejor paso
				mejorPaso = new Paso<TNodo>(arr[i], arr[i + 1], float.PositiveInfinity);

				foreach (var x in _asignación)
				{
					if (mejorPaso.peso > 1 &&
					    x.Value.Vecinos(arr[i]).Contains(arr[i + 1]))
						mejorPaso.peso = 1;
				}
				ret.Concat(mejorPaso);
			}
			return ret;
		}

		bool IGrafica<TNodo>.this [TNodo desde, TNodo hasta]
		{ get { return ExisteArista(desde, hasta); } }

		void IGrafica<TNodo>.AgregaArista(TNodo desde, TNodo hasta)
		{
			_asignación[defColor].AgregaArista(desde, hasta);
		}

		#endregion


		#region IMulticolGrafica implementation


		bool ExisteArista(TNodo desde, TNodo hasta)
		{
			return _asignación.Any(z => z.Value[desde, hasta]);
		}

		public IEnumerable<TColor> ColoresArista(IArista<TNodo> aris)
		{
			var ret = new List<TColor>();
			foreach (var gr in _asignación)
			{
				if (gr.Value[aris.Origen, aris.Destino])
					ret.Add(gr.Key);
			}
			return ret;
		}

		public void AgregaColor(TColor color)
		{
			AgregaColor(color, new Grafica<TNodo>());
		}

		/// <summary>
		/// Agrega un color
		/// </summary>
		/// <param name="color">Nombre del color</param>
		/// <param name="modelo">Gráfica que modela este color</param>
		public void AgregaColor(TColor color, IGrafica<TNodo> modelo)
		{
			if (_asignación.ContainsKey(color))
				throw new ColorDuplicadoException("Ya existe el color " + color);
			_asignación.Add(color, modelo);
		}

		public IGrafica<TNodo> GraficaColor(TColor color)
		{
			return _asignación[color];
		}

		public ICollection<TNodo> Vecinos(TNodo nodo, TColor color)
		{
			IGrafica<TNodo> graf;
			return _asignación.TryGetValue(color, out graf) ? graf.Vecinos(nodo) : new TNodo[0];
		}

		/// <summary>
		/// Devuelve los nodos de la gráfica
		/// </summary>
		/// <value>The nodos.</value>
		public ICollection<TNodo> Nodos
		{			
			get
			{
				var ret = new List<TNodo>();
				foreach (var x in _asignación)
				{
					foreach (var nod in x.Value.Nodos)
					{
						if (!ret.Contains(nod))
							ret.Add(nod);
					}
				}
				return ret;
			}
		}

		#endregion

	}
}

