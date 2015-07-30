﻿using System;
using System.Collections.Generic;
using Graficas.Rutas;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

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
		Dictionary <V, IGrafica<T>> _asignación = new Dictionary<V, IGrafica<T>>();


		public MulticolGrafica()
		{
			
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

		public Graficas.Rutas.IRuta<T> toRuta(IEnumerable<T> seq)
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

		public Graficas.Rutas.IRuta<T> RutaOptima(T x, T y)
		{
			throw new NotImplementedException();
		}

		#endregion


		#region IMulticolGrafica implementation

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
				throw new ColorDuplicadoExpection("Ya existe el color " + color.ToString());
			_asignación.Add(color, modelo);
		}

		public IGrafica<T> GraficaColor(V color)
		{
			return _asignación[color];
		}

		public ICollection<T> Vecinos(T nodo, V color)
		{
			IGrafica<T> graf;
			if (_asignación.TryGetValue(color, out graf))
			{
				return graf.Vecinos(nodo);
			}
			else
				return new T[0];
		}

		/// <summary>
		/// Devuelve los nodos de la gráfica
		/// </summary>
		/// <value>The nodos.</value>
		public ICollection<T> Nodos
		{			
			get
			{
				List<T> ret = new List<T>();
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
			/// <summary>
			/// Initializes a new instance of the <see cref="T:MyException"/> class
			/// </summary>
			public ColorDuplicadoExpection()
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="T:MyException"/> class
			/// </summary>
			/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
			public ColorDuplicadoExpection(string message) : base(message)
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="T:MyException"/> class
			/// </summary>
			/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
			/// <param name="inner">The exception that is the cause of the current exception. </param>
			public ColorDuplicadoExpection(string message, Exception inner) : base(message, inner)
			{
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="T:MyException"/> class
			/// </summary>
			/// <param name="context">The contextual information about the source or destination.</param>
			/// <param name="info">The object that holds the serialized object data.</param>
			protected ColorDuplicadoExpection(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
			{
			}
		}

		#endregion
	}
}

