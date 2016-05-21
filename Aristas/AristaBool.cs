using System;

namespace Graficas.Aristas
{
	/// <summary>
	/// Representa una arista de valor booleano
	/// </summary>
	public class AristaBool<TNodo>  : IAristaDirigida<TNodo> //Test todo
		where TNodo : IEquatable<TNodo>
	{
		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		/// <param name="existe">If set to <c>true</c> existe.</param>
		/// <param name="sóloLectura">Si la arista debe ser de sólo lectura</param>
		/// <param name="simetría">Determina si la arista es bidireccional</param>
		public AristaBool (TNodo origen,
		                   TNodo destino,
		                   bool existe = true,
		                   bool sóloLectura = false,
		                   bool simetría = false)
		{
			Origen = origen;
			Destino = destino;
			Existe = existe;
			SóloLectura = sóloLectura;
			EsSimétrico = simetría;
		}

		TNodo _origen;
		TNodo _destino;
		bool _existe;

		/// <summary>
		/// Devuelve el origen de la arista
		/// </summary>
		/// <value>The origen.</value>
		public TNodo Origen // Fact
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
		public TNodo Destino // Fact
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

		public bool EsSimétrico { get; }
		// Fact
		/// <summary>
		/// Indica si la arista es sólo lectura.
		/// </summary>
		public bool SóloLectura { get; }

		public bool Coincide (TNodo origen, TNodo destino)
		{
			return (_origen.Equals (origen) && _destino.Equals (destino)) ||
			((EsSimétrico) && (_origen.Equals (destino) && _destino.Equals (origen)));
		}

		public ListasExtra.ParNoOrdenado<TNodo> ComoPar ()
		{
			return new ListasExtra.ParNoOrdenado<TNodo> (Origen, Destino);
		}

		public TNodo Antipodo (TNodo nodo)
		{
			return nodo.Equals (Origen) ? Destino : Origen;
		}

		public bool Corta (TNodo nodo)
		{
			return nodo.Equals (Origen) || nodo.Equals (Destino);
		}

		public override string ToString ()
		{
			return ComoPar ().ToString ();
		}
	}
}