using System;
using System.Collections.Generic;

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

