using System;

namespace Graficas.Edges
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
				throw new InvalidOperationException ("Cannot get the weight of an non-existent edge.");
			}
			set
			{
				if (Readonly)
					throw new InvalidOperationException ("Edge is readonly.");
				if (!Exists)
					throw new InvalidOperationException ("Cannot set the weight of an non-existent edge.");
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