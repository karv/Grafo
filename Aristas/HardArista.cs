using Graficas.Nodos;
using System;

namespace Graficas.Aristas
{
	/// <summary>
	/// Arista que conserva los nodos por referencia
	/// </summary>
	public class HardArista<T> : IArista<T>
	{
		/// <summary>
		/// Devuelve el nodo correspondiente al origen
		/// </summary>
		public Nodo<T> Origen { get; }

		/// <summary>
		/// Devuelve el nodo correspondiente al destino
		/// </summary>
		public Nodo<T> Destino { get; }

		T IArista<T>.Origen
		{
			get
			{
				return Origen.Objeto;
			}
		}

		T IArista<T>.Destino
		{
			get
			{
				return Destino.Objeto;
			}
		}

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

		public bool Existe
		{
			get
			{
				throw new NotImplementedException ();
			}
		}
	}
}