namespace Graficas
{
	public interface IGrafo<T> : ILecturaGrafo<T>
	{
		new bool this [T desde, T hasta]{ get; set; }
	}
}