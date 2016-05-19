using ListasExtra;
using System;
using System.Runtime.Remoting.Metadata.W3cXsd2001;


namespace Graficas.Aristas
{
	/// <summary>
	/// Promete habilidad para pedir origen y destino (de esta arista).
	/// </summary>
	/// <typeparam name="T">Tipo de nodos</typeparam>
	public interface IArista<T>
		where T : IEquatable<T>
	{
		/// <summary>
		/// Si esta arista coincide con extremos
		/// </summary>
		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		bool Coincide (T origen, T destino);

		/// <summary>
		/// Existe la arista
		/// </summary>
		/// <value><c>true</c> Si esta arista existe; si no <c>false</c>.</value>
		bool Existe { get; }

		/// <summary>
		/// Devuelve un par que representa a la arista.
		/// </summary>
		ParNoOrdenado<T> ComoPar ();

		/// <summary>
		/// Devuelve el nodo de la arista que no es el dado
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		T Antipodo (T nodo);

		/// <summary>
		/// Devuelve si esta arista toca a un nodo dado
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		bool Corta (T nodo);
	}

	public interface IAristaDirigida <T> : IArista<T>
		where T : IEquatable<T>
	{
		/// <summary>
		/// Origen
		/// </summary>
		/// <value>The origen.</value>
		T Origen { get; }

		/// <summary>
		/// Destino
		/// </summary>
		/// <value>The destino.</value>
		T Destino { get; }
	}
}