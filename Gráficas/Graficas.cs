using System;
using System.Collections.Generic;
using Graficas.Rutas;
using ListasExtra;

namespace Graficas
{
	/// <summary>
	/// Representa una gráfica, en el sentido abstracto.
	/// Los nodos son del tipo <c>T</c>.
	/// <Vecinosy>
	public class Grafica<T> : IGraficaPeso<T>, IGraficaRutas<T> where T : IEquatable<T>
	{
		#region ctor

		public Grafica()
		{
			Vecinos.Nulo = float.PositiveInfinity;
		}

		/// <param name="Nods">Nodos de la gráfica</param>
		public Grafica(T[] Nods)
			: this()
		{
			var r = new Random();
			foreach (var x in Nods)
			{
				AgregaVerticeAzar(x, r);
			}
		}

		#endregion

		#region IGrafica

		float IGraficaPeso<T>.GetPeso(T desde, T hasta)
		{
			return this[desde, hasta];
		}

		void IGraficaPeso<T>.SetPeso(T desde, T hasta, float peso)
		{
			this[desde, hasta] = peso;
		}

		ICollection<T> IGrafica<T>.Nodos
		{
			get
			{
				return Nodos;
			}
		}

		/// <summary>
		/// Calcula la ruta óptima de un nodo a otro.
		/// </summary>
		/// <param name="x">Nodo inicial.</param>
		/// <param name="y">Nodo final.</param>
		/// <returns>Devuelve la ruta de menor <c>Longitud</c>.</returns>
		/// <remarks>Puede ciclar si no existe ruta de x a y.</remarks> // TODO: Arreglar esto.
		public IRuta<T> RutaOptima(T x, T y)
		{
			return CaminoÓptimo(x, y, new HashSet<T>());
		}

		ICollection<T> IGrafica<T>.Vecinos(T nodo)
		{
			return Vecino(nodo);
		}

		bool IGrafica<T>.EsSimétrico
		{
			get
			{
				return EsSimetrico;
			}
		}

		public bool ExisteArista(T desde, T hasta)
		{
			return this[desde, hasta] < float.PositiveInfinity;
		}

		/// <summary>
		/// Agrega una arista (desde, hasta) con peso 1 a esta gráfica
		/// </summary>
		/// <param name="desde">Desde.</param>
		/// <param name="hasta">Hasta.</param>
		public void AgregaArista(T desde, T hasta)
		{
			this[desde, hasta] = 1;
		}

		/// <summary>
		/// Devuelve un clon de la lista de nodos.
		/// </summary>
		public ICollection<T> Nodos
		{
			get
			{
				var ret = new HashSet<T>();
				foreach (var x in Vecinos.Keys)
				{
					if (!ret.Contains(x.Item1))
					{
						ret.Add(x.Item1);
					}

					if (!ret.Contains(x.Item2))
					{
						ret.Add(x.Item2);
					}
				}
				return ret;
			}
		}

		/// <summary>
		/// Agrega una arista II
		/// </summary>
		/// <param name="aris">Aris.</param>
		public void AgregaArista(IArista<T> aris)
		{
			this[aris.Origen, aris.Destino] = 1;
		}

		/// <summary>
		/// Agrega una arista entre dos nodos existentes a la gráfica.
		/// </summary>
		/// <param name="x">Un nodo.</param>
		/// <param name="y">Otro nodo.</param>
		/// <param name="peso">El peso de la arista entre los nodos</param>
		public void AgregaArista(T x, T y, float peso)
		{
			{
				this[x, y] = peso;
			}
		}

		/// <summary>
		/// Devuelve el número de nodos de esta gráfica.
		/// </summary>
		public int NumNodos
		{
			get
			{
				return Nodos.Count;
			}
		}

		public bool ExisteArista(IArista<T> aris)
		{
			return (this[aris.Origen, aris.Destino] < float.PositiveInfinity);
		}

		public IRuta<T> ToRuta(IEnumerable<T> seq)
		{
			throw new NotImplementedException();
		}


		#endregion

		#region Interno

		ListaPeso<Tuple<T, T>> Vecinos = new ListaPeso<Tuple<T, T>>();

		#endregion

		#region Propios

		/// <summary>
		/// Agrega un vértice al grafo, generando aristas al azar a nodos antiguos.
		/// </summary>
		/// <param name="Vertice">Vertice.</param>
		/// <param name="r">Generador aleatorio </param>
		public void AgregaVerticeAzar(T Vertice, Random r)
		{
			if (NumNodos == 0)
			{
				this[Vertice, Vertice] = 0;
				return;
			}

			// Genera la lista de probabilidad.
			// Obtener los pesos
			var Prob = new ListaPeso<T>();
			foreach (var x in Nodos)
			{
				foreach (var y in Vecino(x))
				{
					if (this[x, y] == 0)
						throw new Exception(string.Format("La distancia entro {0} y {1} es cero", x, y));
					Prob[x] += this[x, y];
				}
				Prob[x] = 1 / (Prob[x] + 1);
			}
			// Normalizar Prob
			float S = Prob.SumaTotal();
			// Clonar a Prob.keys
			var P = new List<T>(Prob.Keys);

			foreach (var x in P)
			{
				Prob[x] = Prob[x] / S;
			}

			// Seleccionar un vértice
			T v = SelecciónAzar(Prob, r);

			// Pues entonces hay que agregar arista de x a P[i];
			double p = r.NextDouble() + 0.5d;

			AgregaArista(Vertice, v, (float)p);
		}

		/// <summary>
		/// Selecciona al azar un elemento.
		/// </summary>
		/// <param name="Prob">La función de probabilidad. ¡Debe estar normalizada!</param>
		/// /// <param name="r">Aleatorio</param>
		/// <returns></returns>
		static T SelecciónAzar(IDictionary<T, float> Prob, Random r)
		{
			double q = r.NextDouble();
			foreach (var x in Prob.Keys)
			{
				if (q < Prob[x])
					return x;
				q -= Prob[x];
			}
			throw new Exception("No sé cómo llegó el algoritmo aquí D:");
		}

		public bool EsSimetrico { get; set; }

		/// <summary>
		/// Devuelve la lista de vecinos de x (a todos los que apunta x)
		/// </summary>
		/// <param name="x">Nodo</param>
		public ISet<T> Vecino(T x)
		{
			ISet<T> ret = new HashSet<T>();
			IEnumerable<T> Nods = Nodos;
			foreach (var y in Nods)
			{
				if (!float.IsPositiveInfinity(this[x, y]))
					ret.Add(y);
			}
			return ret;
		}

		/// <summary>
		/// Devuelve la lista de antivecinos de x (todos los que apuntan a x)
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public ISet<T> AntiVecino(T x)
		{
			ISet<T> ret = new HashSet<T>();
			IEnumerable<T> Nods = Nodos;
			foreach (var y in Nods)
			{
				if (!float.IsPositiveInfinity(this[y, x]))
					ret.Add(y);
			}
			return ret;
		}

		/// <summary>
		/// Devuelve o establece el peso de la arista que une dos vértices.
		/// </summary>
		/// <param name="x">Vértice origen.</param>
		/// <param name="y">Vértice destino.</param>
		/// <returns>Devuelve el peso de la arista que une estos nodos. <see cref="float.PositiveInfinity"/> si no existe arista.</returns>
		public float this [T x, T y]
		{
			get
			{
				return Vecinos[new Tuple<T, T>(x, y)];
			}
			set
			{
				Vecinos[new Tuple<T, T>(x, y)] = value;
				if (EsSimetrico)
					Vecinos[new Tuple<T, T>(y, x)] = value;//TODO hacer que essimetrico haga efecto al leer; no al escribir.
			}
		}


		public IRuta<T> CaminoÓptimo(T x, T y)
		{
			return CaminoÓptimo(x, y, new HashSet<T>());
		}

		/// <summary>
		/// Calcula la ruta óptima de un nodo a otro.
		/// </summary>
		/// <param name="x">Nodo inicial.</param>
		/// <param name="y">Nodo final.</param>
		/// <param name="Ignorar">Lista de nodos a evitar.</param>
		/// <returns>Devuelve la ruta de menor <c>Longitud</c>.</returns>
		/// <remarks>Puede ciclar si no existe ruta de x a y.</remarks> // TODO: Arreglar esto.
		IRuta<T> CaminoÓptimo(T x, T y, ISet<T> Ignorar)
		{
			//List<T> retLista = new List<T>();
			IRuta<T> ret = new Ruta<T>();
			IRuta<T> RutaBuscar;
			ISet<T> Ignora2;

			if (x.Equals(y))
			{
				ConcatRuta(ret, x);
				return ret;
			}

			Ignora2 = new HashSet<T>(Ignorar);
			Ignora2.Add(y);

			foreach (var n in AntiVecino(y))
			{
				if (!Ignorar.Contains(n))
				{
					RutaBuscar = CaminoÓptimo(x, n, Ignora2);

					if (ret.NumPasos <= 0 || ret.Longitud > RutaBuscar.Longitud)
					{
						if (RutaBuscar.NumPasos >= 0)
						{
							ConcatRuta(RutaBuscar, y);
							ret = RutaBuscar;
						}
					}
				}
			}
			return ret;
		}

		/// <summary>
		/// Selecciona pseudoaleatoriamente una sublista de tamaño fijo de una lista dada.
		/// </summary>
		/// <param name="r"></param>
		/// <param name="n">Número de elementos a seleccionar.</param>
		/// <param name="Lista">Lista de dónde seleccionar la sublista.</param>
		/// <returns>Devuelve una lista con los elementos seleccionados.</returns>
		List<object> SeleccionaPeso(Random r, int n, ListaPeso<object> Lista)
		{
			List<object> ret;
			float Suma = 0;
			float rn;
			if (n == 0)
				return new List<object>();
			else
			{
				ret = SeleccionaPeso(r, n - 1, Lista);

				foreach (var x in ret)
				{
					Lista[x] = 0;
				}

				// Ahora seleecionar uno.
				Suma = 0;
				rn = (float)r.NextDouble() * Lista.SumaTotal();

				foreach (var x in Lista.Keys)
				{
					Suma += Lista[x];
					if (Suma >= rn)
					{
						ret.Add(x);
						return ret;
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Concatena una ruta y un nodo
		/// </summary>
		/// <param name="ruta">Ruta.</param>
		/// <param name="nodo">Nodo.</param>
		public void ConcatRuta(IRuta<T> ruta, T nodo)
		{
			ruta.Concat(nodo, ruta.NumPasos >= 0 ? this[ruta.NodoFinal, nodo] : 0);
		}


		#endregion

		#region Estáticos

		/// <summary>
		/// Genera una gráfica aleatoria.
		/// </summary>
		/// <param name="Nods">El conjunto de nodos que se usarán.</param>
		/// <returns>Devuelve una gráfica aleatoria.</returns>
		public static Grafica<T> GeneraGraficaAleatoria(List<T> Nods)
		{
			var r = new Random();
			if (Nods.Count < 2)
				throw new Exception("No se puede generar una gráfica aleatoria con menos de dos elementos.");
			var ret = new Grafica<T>();

			T v0, v1;
			v0 = Nods[0];
			v1 = Nods[1];
			Nods.RemoveAt(0);
			Nods.RemoveAt(0);

			ret.AgregaArista(v0, v1, 1);

			foreach (var v in Nods)
			{
				ret.AgregaVerticeAzar(v, r);
			}
			return ret;
		}

		#endregion

	}
		
}
