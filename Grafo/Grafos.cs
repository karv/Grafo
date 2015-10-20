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
	public class Grafo<T> : IGrafoPeso<T>, IGrafoRutas<T> , IGrafo<T>
		where T : IEquatable<T>
	{
		#region ctor

		public Grafo()
		{
			Vecinos.Nulo = float.PositiveInfinity;
		}

		/// <param name="nods">Nodos de la gráfica</param>
		public Grafo(T[] nods)
			: this()
		{
			var r = new Random();
			foreach (var x in nods)
			{
				AgregaVerticeAzar(x, r);
			}
		}

		#endregion

		#region IGrafica

		bool IGrafo<T>.this [T desde, T hasta]
		{ 
			get
			{
				return this[desde, hasta] == 0;
			}
			set
			{
				this[desde, hasta] = (value ? 1 : 0);
			}
		}

		/// <summary>
		/// Calcula el subgrafo generado por un subconjutno de Nodos
		/// </summary>
		/// <param name="conjunto">Conjunto de nodos para calcular el subgrafo</param>
		public Grafo<T> Subgrafo(IEnumerable<T> conjunto)
		{
			var ret = new Grafo<T>();
			foreach (var x in conjunto)
			{
				ret.Nodos.Add(x);
			}

			foreach (var x in new List<T> (conjunto))
			{
				foreach (var y in new List<T> (conjunto))
				{
					ret[x, y] = this[x, y];
				}
			}
			return ret;
		}

		ILecturaGrafo<T> ILecturaGrafo<T>.Subgrafo(IEnumerable<T> conjunto)
		{
			return Subgrafo(conjunto);
		}

		bool ILecturaGrafo<T>.this [T desde, T hasta]
		{ get { return ExisteArista(desde, hasta); } }

		public ICollection<IArista<T>> Aristas()
		{
			var ret = new List<IArista<T>>();
			foreach (var x in Vecinos)
			{
				ret.Add(new Arista<T>(x.Key.Item1, x.Key.Item2, x.Value));
			}
			return ret;
		}

		float IGrafoPeso<T>.this [T desde, T hasta]
		{ 
			get
			{
				return this[desde, hasta];
			}
			set
			{
				this[desde, hasta] = value;
			}
		}

		ICollection<T> ILecturaGrafo<T>.Nodos
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
		public IRuta<T> RutaÓptima(T x, T y)
		{
			return CaminoÓptimo(x, y, new HashSet<T>());
		}

		ICollection<T> ILecturaGrafo<T>.Vecinos(T nodo)
		{
			return Vecino(nodo);
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

		public IRuta<T> ToRuta(IEnumerable<T> seq) //TEST
		{
			IRuta<T> ret = new Ruta<T>();
			bool iniciando = true;
			T last = default(T);
			foreach (var x in seq)
			{
				if (iniciando)
				{
					iniciando = false;
					ret = new Ruta<T>(x);
				}
				else
				{
					ret.Concat(x, this[last, x]);
				}
				last = x;
			}
			return ret;
		}


		#endregion

		#region Interno

		ListaPeso<Tuple<T, T>> Vecinos = new ListaPeso<Tuple<T, T>>();

		#endregion

		#region Propios

		/// <summary>
		/// Agrega un vértice al grafo, generando aristas al azar a nodos antiguos.
		/// </summary>
		/// <param name="vértice">Vertice.</param>
		/// <param name="r">Generador aleatorio </param>
		public void AgregaVerticeAzar(T vértice, Random r)
		{
			if (NumNodos == 0)
			{
				this[vértice, vértice] = 0;
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

			AgregaArista(vértice, v, (float)p);
		}

		/// <summary>
		/// Selecciona al azar un elemento.
		/// </summary>
		/// <param name="prob">La función de probabilidad. ¡Debe estar normalizada!</param>
		/// /// <param name="r">Aleatorio</param>
		/// <returns></returns>
		static T SelecciónAzar(IDictionary<T, float> prob, Random r)
		{
			double q = r.NextDouble();
			foreach (var x in prob.Keys)
			{
				if (q < prob[x])
					return x;
				q -= prob[x];
			}
			throw new Exception("No sé cómo llegó el algoritmo aquí D:");
		}

		public bool EsSimétrico { get; set; }

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
				return EsSimétrico ? Math.Min(Vecinos[new Tuple<T, T>(x, y)], Vecinos[new Tuple<T, T>(y, x)]) : Vecinos[new Tuple<T, T>(x, y)];
			}
			set
			{
				Vecinos[new Tuple<T, T>(x, y)] = value;
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
		/// <param name="ignorar">Lista de nodos a evitar.</param>
		/// <returns>Devuelve la ruta de menor <c>Longitud</c>.</returns>
		/// <remarks>Puede ciclar si no existe ruta de x a y.</remarks> // TODO: Arreglar esto.
		IRuta<T> CaminoÓptimo(T x, T y, ISet<T> ignorar)
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

			Ignora2 = new HashSet<T>(ignorar);
			Ignora2.Add(y);

			foreach (var n in AntiVecino(y))
			{
				if (!ignorar.Contains(n))
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
		/// <param name="lista">Lista de dónde seleccionar la sublista.</param>
		/// <returns>Devuelve una lista con los elementos seleccionados.</returns>
		List<object> SeleccionaPeso(Random r, int n, ListaPeso<object> lista)
		{
			List<object> ret;
			float Suma = 0;
			float rn;
			if (n == 0)
				return new List<object>();
			else
			{
				ret = SeleccionaPeso(r, n - 1, lista);

				foreach (var x in ret)
				{
					lista[x] = 0;
				}

				// Ahora seleecionar uno.
				Suma = 0;
				rn = (float)r.NextDouble() * lista.SumaTotal();

				foreach (var x in lista.Keys)
				{
					Suma += lista[x];
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
		/// <param name="nods">El conjunto de nodos que se usarán.</param>
		/// <returns>Devuelve una gráfica aleatoria.</returns>
		public static Grafo<T> GeneraGraficaAleatoria(List<T> nods)
		{
			var r = new Random();
			if (nods.Count < 2)
				throw new Exception("No se puede generar una gráfica aleatoria con menos de dos elementos.");
			var ret = new Grafo<T>();

			T v0, v1;
			v0 = nods[0];
			v1 = nods[1];
			nods.RemoveAt(0);
			nods.RemoveAt(0);

			ret.AgregaArista(v0, v1, 1);

			foreach (var v in nods)
			{
				ret.AgregaVerticeAzar(v, r);
			}
			return ret;
		}

		#endregion

	}
		
}
