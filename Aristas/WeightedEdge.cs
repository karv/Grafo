using System;

namespace Graficas.Aristas
{
	/// <summary>
	/// A weighted edge.
	/// </summary>
	[Serializable]
	public class WeightedEdge<TNode, TWg> : ExistentialEdge<TNode>
	{
		TWg _valor;

		/// <summary>
		/// Gets the weight of this edge.
		/// </summary>
		/// <value>The peso.</value>
		public TWg Data
		{
			get
			{
				if (Exists)
					return _valor;
				throw new OperaciónAristaInválidaException ("No se puede acceder al peso de una arista no existente.");
			}
			set
			{
				if (Readonly)
					throw new OperaciónAristaInválidaException ("Arista está en modo lectura.");
				if (!Exists)
					throw new OperaciónAristaInválidaException ("No se le puede asignar peso a una arista no existente.");
				_valor = value;
			}
		}

		/// <summary> Initializes a new (existing)edge. </summary>
		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		/// <param name="valor">Peso.</param>
		/// <param name="sóloLectura">El objeto se creará como sólo lectura</param>
		/// <param name="simétrico">La arista tiene dirección</param>
		public WeightedEdge (TNode origen,
											 TNode destino,
											 TWg valor,
											 bool sóloLectura = false,
											 bool simétrico = false)
			: base (origen, destino, true, sóloLectura, simétrico)
		{
			_valor = valor;
		}

		/// <summary>
		/// Initializes a new non-existent edge.
		/// </summary>
		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		/// <param name="sóloLectura">El objeto se creará como sólo lectura</param>
		/// <param name="simétrico">La arista tiene dirección</param>
		public WeightedEdge (TNode origen,
											 TNode destino,
											 bool sóloLectura = false,
											 bool simétrico = false)
			: base (origen, destino, false, sóloLectura, simétrico)
		{
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:Graficas.Aristas.WeightedEdge`2"/>.
		/// </summary>
		public override string ToString ()
		{
			var baseStr = base.ToString ();
			return string.Format ("{0}: {1}", baseStr, _valor);
		}
	}
}