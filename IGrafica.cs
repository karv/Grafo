using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Graficas.Rutas;

namespace Graficas
{

	/// <summary>
	/// Promete lista de vecinos.
	/// </summary>
	public interface IGrafica<T>
	{
		ICollection<T> Nodos
		{
			get;
		}

		bool ExisteArista(IArista<T> aris);

		void AgregaArista(IArista<T> aris);

		ICollection<T> Vecinos(T nodo);

		Graficas.Rutas.IRuta<T> toRuta(IEnumerable<T> seq);

		bool esSimétrico{ get; }
	}

	/// <summary>
	/// Provee método para encontrar ruta óptima entre puntos
	/// </summary>
	public interface IGraficaRutas<T> : IGrafica<T>
	{
		IRuta<T> RutaOptima(T x, T y);
	}

	/// <summary>
	/// Promete habilidad para pedir origen y destino (de esta arista).
	/// </summary>
	/// <typeparam name="T">Tipo de nodos</typeparam>
	public interface IArista<T>
	{
		T desde { get; }

		T hasta { get; }
	}


	public static class ExtIGrafica
	{
		public static IEnumerable<IArista<T>> EnumerarNodos<T>(this IGrafica<T> graf) where T : IEquatable<T>
		{
			List<Arista<T>> ret = new List<Arista<T>>();
			foreach (T x in graf.Nodos)
			{
				foreach (T y in graf.Vecinos(x))
				{
					ret.Add(new Arista<T>(x, y));
				}
			}
			return (IEnumerable<IArista<T>>)ret;
		}
	}

}
