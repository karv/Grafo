namespace Graficas.Grafo
{
	/// <summary>
	/// Un grafo en la que sus aristas poseen un peso.
	/// </summary>
	public interface IGrafoPeso<T> : ILecturaGrafoPeso<T>, IGrafo<T>
	{
		/// <summary>
		/// El peso de una arista.
		/// </summary>
		/// <param name="desde">Origen de la arista</param>
		/// <param name="hasta">Destino de la arista</param>
		/// <returns>Devuelve el peso (float) de la arista.</returns>
		new float this [T desde, T hasta]{ get; set; }
	}
}