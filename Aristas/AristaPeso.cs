using System;

namespace Graficas.Aristas
{
	/// <summary>
	/// Representa una arista de un grafo.
	/// Almacena el peso fuertemente, por lo que no hay vínculo directo con un grafo
	/// </summary>
	[Serializable]
	public class AristaPeso<TNodo, TValor> : AristaBool<TNodo>
		where TNodo : IEquatable<TNodo>
	{
		TValor _valor;

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

		/// <summary> Construye una arista existente </summary>
		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		/// <param name="valor">Peso.</param>
		/// <param name="sóloLectura">El objeto se creará como sólo lectura</param>
		/// <param name="simétrico">La arista tiene dirección</param>
		public AristaPeso (TNodo origen,
		                   TNodo destino,
		                   TValor valor,
		                   bool sóloLectura = false,
		                   bool simétrico = false)
			: base (origen, destino, true, sóloLectura, simétrico)
		{
			_valor = valor;
		}

		/// <summary>
		/// Construye una arista inexistente
		/// </summary>
		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		/// <param name="sóloLectura">El objeto se creará como sólo lectura</param>
		/// <param name="simétrico">La arista tiene dirección</param>
		public AristaPeso (TNodo origen,
		                   TNodo destino,
		                   bool sóloLectura = false,
		                   bool simétrico = false)
			: base (origen, destino, false, sóloLectura, simétrico)
		{
		}

		public override string ToString ()
		{
			return string.Format ("{0}: {1}", base.ToString (), _valor);
		}
	}
}