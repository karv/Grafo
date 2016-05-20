using System;

namespace Graficas.Aristas
{
	/// <summary>
	/// Representa una arista de valor booleano
	/// </summary>
	public class AristaBool<T>  : IAristaDirigida<T> //Fact todo
		where T : IEquatable<T>
	{
		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		/// <param name="existe">If set to <c>true</c> existe.</param>
		/// <param name="sóloLectura">Si la arista debe ser de sólo lectura</param>
		public AristaBool (T origen,
		                   T destino,
		                   bool existe = true,
		                   bool sóloLectura = false)
		{
			Origen = origen;
			Destino = destino;
			Existe = existe;
			SóloLectura = sóloLectura;
		}

		T _origen;
		T _destino;
		bool _existe;

		/// <summary>
		/// Devuelve el origen de la arista
		/// </summary>
		/// <value>The origen.</value>
		public T Origen // Fact
		{
			get
			{
				return _origen;
			}
			set
			{
				if (SóloLectura)
					throw new OperaciónAristaInválidaException ("Arista está en modo lectura.");
				_origen = value;
			}
		}

		/// <summary>
		/// Devuelve el destino de la arista
		/// </summary>
		/// <value>The destino.</value>
		public T Destino // Fact
		{
			get
			{
				return _destino;
			}
			set
			{
				if (SóloLectura)
					throw new OperaciónAristaInválidaException ("Arista está en modo lectura.");
				_destino = value;
			}
		}

		/// <summary>
		/// Existe la arista
		/// </summary>
		/// <value>true</value>
		/// <c>false</c>
		public bool Existe // Fact
		{
			get
			{
				return _existe;
			}
			set
			{
				if (SóloLectura)
					throw new OperaciónAristaInválidaException ("Arista está en modo lectura.");
				_existe = value;
			}
		}

		// Fact
		/// <summary>
		/// Indica si la arista es sólo lectura.
		/// </summary>
		public bool SóloLectura { get; }

		public bool Coincide (T origen, T destino)
		{
			return Origen.Equals (origen) && Destino.Equals (destino);
		}

		public ListasExtra.ParNoOrdenado<T> ComoPar ()
		{
			return new ListasExtra.ParNoOrdenado<T> (Origen, Destino);
		}

		public T Antipodo (T nodo)
		{
			return nodo.Equals (Origen) ? Destino : Origen;
		}

		public bool Corta (T nodo)
		{
			return nodo.Equals (Origen) || nodo.Equals (Destino);
		}
	}
}