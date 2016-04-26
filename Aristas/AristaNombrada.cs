namespace Graficas.Aristas
{
	public class AristaNombrada<TNodo, TNombre>  : IArista<TNodo>
	{
		protected AristaNombrada ()
		{
		}

		public AristaNombrada (TNodo origen, TNodo destino, TNombre nombre)
		{
			Origen = origen;
			Destino = destino;
			Nombre = nombre;
		}

		/// <summary>
		/// Origen de la arista
		/// </summary>
		public TNodo Origen { get; protected set; }

		/// <summary>
		/// Destino de la arista
		/// </summary>
		public TNodo Destino { get; protected set; }

		/// <summary>
		/// Nombre de la arista
		/// </summary>
		public TNombre Nombre { get; protected set; }

		float IArista<TNodo>.Peso
		{
			get
			{
				return 1;
			}
		}
	}
}