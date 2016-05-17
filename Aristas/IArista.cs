namespace Graficas.Aristas
{
	/// <summary>
	/// Promete habilidad para pedir origen y destino (de esta arista).
	/// </summary>
	/// <typeparam name="T">Tipo de nodos</typeparam>
	public interface IArista<T>
	{
		/// <summary>
		/// Devuelve el origen de la arista
		/// </summary>
		T Origen { get; }

		/// <summary>
		/// Devuelve el destino de la arista
		/// </summary>
		T Destino { get; }

		/// <summary>
		/// Existe la arista
		/// </summary>
		/// <value><c>true</c> Si esta arista existe; si no <c>false</c>.</value>
		bool Existe { get; }
	}
}