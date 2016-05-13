namespace Graficas.Rutas
{
	// THINK: ¿Hacer obsoleto y usar aristas?
	/// <summary>
	/// Representa un paso; como sinónimo de arista
	/// </summary>
	public interface IPaso<T>
	{
		/// <summary>
		/// Devuelve el origen
		/// </summary>
		/// <value>The origen.</value>
		T Origen { get; }

		/// <summary>
		/// Devuelve el destino
		/// </summary>
		/// <value>The destino.</value>
		T Destino { get; }

		/// <summary>
		/// Devuelve el peso o longitud del paso
		/// </summary>
		float Peso { get; }
	}
}