using Graficas.Nodos;
using System;

namespace Graficas.Aristas
{
	/// <summary>
	/// Arista que conserva los nodos por referencia
	/// </summary>
	public class HardArista<T> : IAristaDirigida<T>
		where T : IEquatable<T>
	{
		/// <summary>
		/// Devuelve el nodo correspondiente al origen
		/// </summary>
		public Nodo<T> Origen { get; }

		/// <summary>
		/// Devuelve el nodo correspondiente al destino
		/// </summary>
		public Nodo<T> Destino { get; }

		/// <summary>
		/// Devuelve 1 si existe esta arista; 0 en caso contratio.
		/// </summary>
		public float Peso
		{
			get
			{
				return Origen.Vecindad.Contains (Destino) ? 1 : 0;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public override string ToString ()
		{
			return string.Format (
				"[HardArista: Origen={0}, Destino={1}, Peso={2}]",
				Origen.Objeto,
				Destino.Objeto,
				Peso);
		}

		/// <summary>
		/// Initializes a new instance
		/// </summary>
		/// <param name="desde">Desde</param>
		/// <param name="hasta">Hasta</param>
		public HardArista (Nodo<T> desde, Nodo<T> hasta)
		{
			Origen = desde;
			Destino = hasta;
		}

		/// <summary>
		/// Existe la arista
		/// </summary>
		/// <value><c>true</c> Si esta arista existe; si no <c>false</c>.</value>
		public bool Existe
		{
			get
			{
				throw new NotImplementedException ();
			}
		}

		public bool Coincide (T origen, T destino)
		{
			return Origen.Objeto.Equals ((origen)) && Destino.Objeto.Equals ((destino));
		}

		public ListasExtra.ParNoOrdenado<T> ComoPar ()
		{
			return new ListasExtra.ParNoOrdenado<T> (Origen.Objeto, Destino.Objeto);
		}

		public T Antipodo (T nodo)
		{
			return nodo.Equals (Destino.Objeto) ? Origen.Objeto : Destino.Objeto;
		}

		public bool Corta (T nodo)
		{
			return nodo.Equals ((Origen.Objeto)) || nodo.Equals ((Destino.Objeto));
		}

		#region IArista

		T IAristaDirigida<T>.Origen { get { return Origen.Objeto; } }

		T IAristaDirigida<T>.Destino { get { return Destino.Objeto; } }

		#endregion
	}
}