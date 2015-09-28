namespace Graficas.Rutas
{
	public interface IPaso<T>
	{
		T Origen { get; }

		T Destino{ get; }

		float Peso { get; }
	}
}

