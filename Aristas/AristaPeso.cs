using System;

namespace Graficas.Aristas
{
	/// <summary>
	/// Representa una arista de un grafo.
	/// Almacena el peso fuertemente, por lo que no hay vínculo directo con un grafo
	/// </summary>
	[Serializable]
	public class AristaPeso<TNodo, TValor> : IArista<TNodo> // TEST all
		where TNodo : IEquatable<TNodo>
	{
		TValor _valor;
		TNodo _origen;
		TNodo _destino;
		bool _existe;

		public bool Coincide (TNodo origen, TNodo destino)
		{
			return (_origen.Equals (origen) && _destino.Equals (destino)) ||
			((EsSimétrico) && (_origen.Equals (destino) && _destino.Equals (origen)));
		}

		public bool EsSimétrico { get; }

		/// <summary>
		/// Devuelve el origen de la arista
		/// </summary>
		/// <value>The origen.</value>
		[Obsolete]
		public TNodo Origen
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
		[Obsolete]
		public TNodo Destino
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
		/// Devuelve el peso de la arista
		/// </summary>
		/// <value>The peso.</value>
		public TValor Data
		{
			get
			{
				if (Existe)
					return _valor;
				else
					throw new OperaciónAristaInválidaException ("No se puede acceder al peso de una arista no existente.");
			}
			set
			{
				if (SóloLectura)
					throw new OperaciónAristaInválidaException ("Arista está en modo lectura.");
				if (!Existe)
					throw new OperaciónAristaInválidaException ("No se le puede asignar peso a una arista no existente.");
				_valor = value;
			}
		}

		/// <summary>
		/// Indica si este objeto es de sólo lectura
		/// </summary>
		/// <value><c>true</c> si es de sólo lectura; de otra forma, <c>false</c>.</value>
		public bool SóloLectura { get; }

		/// <summary>
		/// Existe la arista
		/// </summary>
		/// <value><c>true</c> Si esta arista existe; si no <c>false</c>.</value>
		public bool Existe
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

		public ListasExtra.ParNoOrdenado<TNodo> ComoPar ()
		{
			return new ListasExtra.ParNoOrdenado<TNodo> (_origen, _destino);
		}

		public TNodo Antipodo (TNodo nodo)
		{
			return nodo.Equals (_origen) ? _destino : _origen;
		}

		public bool Corta (TNodo nodo)
		{
			return nodo.Equals (_origen) || nodo.Equals (_destino);
		}

		/// <summary> Construye una arista existente </summary>
		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		/// <param name="valor">Peso.</param>
		/// <param name="sóloLectura">El objeto se creará como sólo lectura</param>
		public AristaPeso (TNodo origen,
		                   TNodo destino,
		                   TValor valor,
		                   bool sóloLectura = false)
		{
			_origen = origen;
			_destino = destino;
			_valor = valor;
			_existe = true;
			SóloLectura = sóloLectura;
		}

		/// <summary>
		/// Construye una arista inexistente
		/// </summary>
		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		/// <param name="sóloLectura">El objeto se creará como sólo lectura</param>
		public AristaPeso (TNodo origen, TNodo destino, bool sóloLectura = false)
		{
			_origen = origen;
			_destino = destino;
			_existe = false;
			SóloLectura = sóloLectura;
		}
	}
}