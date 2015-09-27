namespace Graficas
{
	public class Arista<T> : IArista<T>
	{
		public T Origen { get; }

		public T Destino { get; }

		public Arista(T origen, T destino)
		{
			Origen = origen;
			Destino = destino;
		}
	}
}
