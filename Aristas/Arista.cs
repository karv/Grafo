namespace Graficas
{
	public class Arista<T> : IArista<T>
	{
		public T Origen { get; }

		public T Destino { get; }

		public float Peso { get; }

		public Arista(T origen, T destino, float peso)
		{
			Origen = origen;
			Destino = destino;
			Peso = peso;
		}
	}
}