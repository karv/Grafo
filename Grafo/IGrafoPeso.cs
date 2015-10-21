namespace Graficas
{
	public interface IGrafoPeso<T> : ILecturaGrafoPeso<T>
	{
		/// <summary>
		/// El peso de una arista.
		/// </summary>
		/// <param name="desde">Origen de la arista</param>
		/// <param name="hasta">Destino de la arista</param>
		/// <returns>Devuelve el peso (float) de la arista.</returns>
		new float this [T desde, T hasta]
		{
			get;
			set
		}
	}
}

