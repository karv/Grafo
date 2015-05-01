using System;
using System.Collections;
using System.Collections.Generic;

namespace Graficas.Rutas
{
	/// <summary>
	/// Promete enumerar nodos en una ruta.
	/// </summary>
	/// <typeparam name="T">Tipo de nodos</typeparam>
	public interface IRuta<T> : IEnumerable<T> where T : IEquatable<T>
	{
		T nodoInicial { get; }
		T nodoFinal { get; }
		IGrafica<T> getGrafica();
		/// <summary>
		/// Devuelve en número de nodos en la ruta
		/// </summary>
		int numPasos { get; }
	}

	/// <summary>
	/// Da herramientas para trabajar con rutas en IGraficas
	/// </summary>
	public static class ExtensionRutas
	{
		public static Ruta<T> Concatenar<T>(IRuta<T> left, IRuta<T> right) where T : IEquatable<T>
		{
			if (!left.getGrafica().Equals(right.getGrafica()))
				throw new Exception("No se pueden concatenar rutas de distintas gráficas.");
			if (!left.nodoFinal.Equals(right.nodoInicial))
				throw new Exception(string.Format("El nodo final de left ({0}) debe coincidir con el nodo incial de right ({1})", left, right));
			Ruta<T> ret = new Ruta<T>(left.getGrafica());

			foreach (T x in left)
			{
				ret.Paso.Add(x);
			}

			bool inicial = true;
			foreach (T x in right)
			{
				if (!inicial)
					ret.Paso.Add(x);
				else
					inicial = false;
			}
			return ret;
		}
		/// <summary>
		/// Devuelve la longitud de una ruta.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="ruta"></param>
		/// <returns>Devuelve la longitud de una ruta, según su gráfica.</returns>
		public static float Longitud<T>(this IRuta<T> ruta) where T : IEquatable<T>
		{
			if (ruta.getGrafica() is IGraficaPeso<T>)
			{
				IGraficaPeso<T> grafica = (IGraficaPeso<T>)(ruta.getGrafica());
				float ret = 0;
				T nodoPre = default(T);
				T nodoPost = default(T);
				bool primeraIter = true;
				foreach (var x in ruta)
				{
					nodoPost = x;
					if (!primeraIter)
					{
						ret += grafica.Peso(nodoPre, nodoPost);
					}
					nodoPre = nodoPost;
					primeraIter = false;
				}
				return ret;
			}
			else return ruta.numPasos - 1;	// Devuelve el número de pasos si la gráfica no es de peso.
		}

		/// <summary>
		/// Devuelve la(una) ruta de menor longitud entre dos nodos de la gráfica.
		/// </summary>
		/// <typeparam name="T">Tipo de nodos</typeparam>
		/// <param name="graf"></param>
		/// <param name="origen">Nodos inicial</param>
		/// <param name="destino">Nodo final</param>
		/// <param name="nodosIgnorar">Lista de nodos a ignorar</param>
		/// <returns>Devuelve la ruta de menor longitud entre origen y destino, si existe.
		/// Si no existe devuelve null.</returns>
		public static Ruta<T> MejorRuta<T>(this IGrafica<T> graf, T origen, T destino, List<T> nodosIgnorar = null) where T : IEquatable<T>
		{
			Ruta<T> ret = new Ruta<T>(graf);
			if (nodosIgnorar == null) nodosIgnorar = new List<T>();
			ret.Paso.Add(origen);

			if (origen.Equals(destino))
				return ret;
			else
			{
				Ruta<T> mejorTemporal = new Ruta<T>(graf);
				foreach (T vec in graf.Vecinos(origen))
				{
					if (!nodosIgnorar.Contains(vec))
					{
						List<T> IgnoraIterador = new List<T>(nodosIgnorar);	// Copia nodos ignorar
						Ruta<T> rutaIterador = graf.MejorRuta(vec, destino, IgnoraIterador);
						if (mejorTemporal.numPasos == 0 || rutaIterador.Longitud() < mejorTemporal.Longitud())
							mejorTemporal = rutaIterador;
					}
				}
				if (mejorTemporal == null) return null;
				foreach (T x in mejorTemporal.Paso)
				{
					ret.Paso.Add(x);
				}
				return ret;
			}
		}
	}

	/// <summary>
	/// Una ruta en una gráfica.
	/// </summary>
	/// <typeparam name="T">Tipo de nodos.</typeparam>
	public class Ruta<T> : IRuta<T> where T : IEquatable<T>
	{
		IGrafica<T> _grafica;
		public List<T> Paso;

		public Ruta(IGrafica<T> grafica)
		{
			_grafica = grafica;
		}

		public static bool operator ==(Ruta<T> left, Ruta<T> right)
		{
			if (left.Paso.Count != right.Paso.Count)
				return false;

			for (int i = 0; i < left.Paso.Count; i++)
			{
				if (!left.Paso[i].Equals(right.Paso[i]))
					return false;
			}
			return true;
		}

		public static bool operator !=(Ruta<T> left, Ruta<T> right)
		{
			return !(left == right);
		}

		public override bool Equals(object obj)
		{
			if (obj is Ruta<T>)
				return ((Ruta<T>)obj) == this;
			else
				return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		IGrafica<T> IRuta<T>.getGrafica()
		{
			return _grafica;
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return (IEnumerator<T>)Paso;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return (IEnumerator)Paso;
		}

		public T nodoInicial
		{
			get { return Paso[0]; }
		}

		public T nodoFinal
		{
			get { return Paso[Paso.Count - 1]; }
		}

		public int numPasos
		{
			get { return Paso.Count; }
		}
	}
}
