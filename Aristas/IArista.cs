namespace Graficas
{

	/// <summary>
	/// Promete habilidad para pedir origen y destino (de esta arista).
	/// </summary>
	/// <typeparam name="T">Tipo de nodos</typeparam>
	public interface IArista<T>
	{
		T Origen { get; }

		T Destino { get; }

		float Peso { get; }
	}

}
