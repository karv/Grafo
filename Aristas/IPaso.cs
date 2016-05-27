using System;

namespace Graficas.Aristas
{
	public interface IPaso<T> : IAristaDirigida<T>
		where T : IEquatable<T>
	{
		float Peso { get; }
	}
}