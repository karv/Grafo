using System;
using System.Collections.Generic;
using System.Linq;
using Graficas.Rutas;

namespace Graficas
{
	/// <summary>
	/// Modela una IMulticolGrafica que es la superposición de varias IGráficas
	/// </summary>
	public class MulticolGrafica<T, V> : IMulticolGrafica<T, V>
		where T :IEquatable<T>
	{
		/// <summary>
		/// La asignación de color -> Gráfica
		/// </summary>
		readonly Dictionary <V, IGrafica<T>> _asignación = new Dictionary<V, IGrafica<T>>();

		/// <summary>
		/// Color default
		/// </summary>
		V defColor;

		public MulticolGrafica()
		{
			
		}

		/// <param name="defColor">Color default</param>
		public MulticolGrafica(V defColor)
		{
			this.defColor = defColor;
			AgregaColor(defColor);
		}

		#region IGrafica

		/// <summary>
		/// Devuelve los vecinos de cualquier color de un nodo dado
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		public ICollection<T> Vecinos(T nodo)
		{
			ISet<T> ret = new HashSet<T>();

			foreach (var color in _asignación.Keys)
			{
				ret.UnionWith(_asignación[color].Vecinos(nodo));
			}
			return ret;
		}

		public IRuta<T> ToRuta(IEnumerable<T> seq)
		{
			IRuta<T> ret = new Ruta<T>();
			T[] arr = seq.ToArray();
			Paso<T> mejorPaso;
			for (int i = 0; i < arr.Count() - 1; i++)
			{
				// Encontrar mejor paso
				mejorPaso = new Paso<T>(arr[i], arr[i + 1], float.PositiveInfinity);

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

		void IGrafica<T>.AgregaArista(T desde, T hasta)
		{
			_asignación[defColor].AgregaArista(desde, hasta);
		}


		bool IGrafica<T>.EsSimétrico
		{
			get
			{
				return false;
			}
		}

		#endregion


		#region IMulticolGrafica implementation

		bool IGrafica<T>.ExisteArista(T desde, T hasta)
		{
			return _asignación.Any(z => z.Value.ExisteArista(desde, hasta));
		}

		public IEnumerable<V> ColoresArista(IArista<T> aris)
		{
			var ret = new List<V>();
			foreach (var gr in _asignación)
			{
				if (gr.Value.ExisteArista(aris.Origen, aris.Destino))
					ret.Add(gr.Key);
			}
			return ret;
		}

		public void AgregaColor(V color)
		{
			AgregaColor(color, new Grafica<T>());
		}

		/// <summary>
		/// Agrega un color
		/// </summary>
		/// <param name="color">Nombre del color</param>
		/// <param name="modelo">Gráfica que modela este color</param>
		public void AgregaColor(V color, IGrafica<T> modelo)
		{
			if (_asignación.ContainsKey(color))
				throw new ColorDuplicadoExpection("Ya existe el color " + color);
			_asignación.Add(color, modelo);
		}

		public IGrafica<T> GraficaColor(V color)
		{
			return _asignación[color];
		}

		public ICollection<T> Vecinos(T nodo, V color)
		{
			IGrafica<T> graf;
			return _asignación.TryGetValue(color, out graf) ? graf.Vecinos(nodo) : new T[0];
		}

		/// <summary>
		/// Devuelve los nodos de la gráfica
		/// </summary>
		/// <value>The nodos.</value>
		public ICollection<T> Nodos
		{			
			get
			{
				var ret = new List<T>();
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

		#region Exceptions

		/// <summary>
		/// Color duplicado expection.
		/// </summary>
		[Serializable]
		public class ColorDuplicadoExpection : Exception
		{
			public ColorDuplicadoExpection()
			{
			}

			public ColorDuplicadoExpection(string message) : base(message)
			{
			}

			public ColorDuplicadoExpection(string message, Exception inner) : base(message, inner)
			{
			}

			protected ColorDuplicadoExpection(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
			{
			}
		}

		#endregion
	}
}

