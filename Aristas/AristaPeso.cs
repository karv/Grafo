using System;

namespace Graficas.Aristas
{
	/// <summary>
	/// Representa una arista de un grafo.
	/// Almacena el peso fuertemente, por lo que no hay vínculo directo con un grafo
	/// </summary>
	[Serializable]
	public class AristaPeso<TNodo, TValor> : IArista<TNodo>
	{
		TValor _dalor;
		TNodo _origen;
		TNodo _destino;
		bool _existe;

		/// <summary>
		/// Devuelve el origen de la arista
		/// </summary>
		/// <value>The origen.</value>
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
					return _dalor;
				else
					throw new OperaciónAristaInválidaException ("No se puede acceder al peso de una arista no existente.");
			}
			set
			{
				if (SóloLectura)
					throw new OperaciónAristaInválidaException ("Arista está en modo lectura.");
				if (!Existe)
					throw new OperaciónAristaInválidaException ("No se le puede asignar peso a una arista no existente.");
				_dalor = value;
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
			_dalor = valor;
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