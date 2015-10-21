namespace Graficas
{
	/// <summary>
	/// Promete la habilidad de decidir el peso de cada arista en una gráfica.
	/// </summary>
	/// <typeparam name="T">Tipo de nodos.</typeparam>
	public interface ILecturaGrafoPeso<T> : ILecturaGrafo<T>
	{
		/// <summary>
		/// El peso de una arista.
		/// </summary>
		/// <param name="desde">Origen de la arista</param>
		/// <param name="hasta">Destino de la arista</param>
		/// <returns>Devuelve el peso (float) de la arista.</returns>
		new float this [T desde, T hasta] { get; }
	}
}