using System;
using System.Collections.Generic;

namespace Graficas.Aristas
{
	/// <summary>
	/// Un paso de una ruta.
	/// </summary>
	/// <remarks>Este objeto es de sólo lectura</remarks>
	[Serializable]
	public struct Step<T> : IStep<T>
	{
		/// <summary>
		/// Devuelve el origen del paso
		/// </summary>
		/// <value>The origen.</value>
		public T Origen { get; }

		/// <summary>
		/// Devuelve el destino del paso
		/// </summary>
		/// <value>The destino.</value>
		public T Destino { get; }

		/// <summary>
		/// Peso del paso
		/// </summary>
		/// <value>The peso.</value>
		public float Peso { get; }

		/// <summary>
		/// Initializes a new instance of this struct.
		/// </summary>
		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		/// <param name="peso">Peso.</param>
		public Step (T origen, T destino, float peso = 1)
		{
			Origen = origen;
			Destino = destino;
			Peso = peso;
		}

		/// <summary>
		/// Initializes a new instance of this struct.
		/// </summary>
		/// <param name="aris">Arista a copiar</param>
		public Step (IDirectedEdge<T> aris)
			: this (aris.Origin, aris.Destination)
		{
			var ar = aris as AristaPeso<T, float>;
			if (ar != null) {
				if (!ar.Exists)
					throw new Exception ("No se puede construir un paso desde una arista inexistente.");
				Peso = ar.Data;
			}
		}

		/// <summary>
		/// Initializes a new instance of this struct.
		/// </summary>
		/// <param name="aris">Arista a copiar</param>
		/// <param name="peso">Peso del paso</param>
		public Step (IDirectedEdge<T> aris, float peso)
			: this (aris.Origin, aris.Destination)
		{
			Peso = peso;
		}

		/// <summary>
		/// Clona un paso
		/// </summary>
		/// <param name="paso">Paso a clonar</param>
		public Step (IStep<T> paso)
		{
			Origen = paso.Origin;
			Destino = paso.Destination;
			Peso = paso.Peso;
		}

		/// <summary>
		/// Si esta arista coincide con extremos
		/// </summary>
		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		public bool Coincide (T origen, T destino)
		{
			var cmp = EqualityComparer<T>.Default;
			return cmp.Equals (origen, Origen) && cmp.Equals (destino, Destino);
		}

		/// <summary>
		/// Devuelve un par que representa a la arista.
		/// </summary>
		/// <returns>The par.</returns>
		public ListasExtra.ParNoOrdenado<T> ComoPar ()
		{
			return new ListasExtra.ParNoOrdenado<T> (Origen, Destino);
		}

		/// <summary>
		/// Devuelve el nodo de la arista que no es el dado
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		public T Antipodo (T nodo)
		{
			return ComoPar ().Excepto (nodo);
		}

		/// <summary>
		/// Devuelve si esta arista toca a un nodo dado
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		public bool Corta (T nodo)
		{
			var cmp = EqualityComparer<T>.Default;
			return cmp.Equals (nodo, Origen) || cmp.Equals (nodo, Destino);
		}

		/// <summary>
		/// Existe este paso.
		/// </summary>
		/// <value><c>true</c> si existe. <c>false</c> en caso contrario</value>
		public bool Existe {
			get {
				return true;
			}
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current paso/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current struct.</returns>
		public override string ToString ()
		{
			return string.Format ("{0} -- [{2}] -> {1}", Origen, Destino, Peso);
		}
	}
}