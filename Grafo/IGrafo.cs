namespace Graficas.Grafo
{
	public interface IGrafo<T> : ILecturaGrafo<T>
	{
		void Clear ();

		new bool this [T desde, T hasta]{ get; set; }
	}
}