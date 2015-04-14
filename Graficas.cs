using System;
using System.Collections.Generic;
using ListasExtra;

namespace Graficas
{
	/// <summary>
	/// Representa una gráfica, en el sentido abstracto.
	/// Los nodos serán del tipo <c>T</c>.
	/// </summary>
	public class Grafica<T>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Graficas.Grafica`1"/> class.
		/// </summary>
		public Grafica()
		{
			Vecinos.Nulo = float.PositiveInfinity;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Graficas.Grafica`1"/> class.
		/// Crea una gráfica al azar
		/// </summary>
		/// <param name="Nods">Nodos de la gráfica</param>
		public Grafica(T[] Nods):this()
		{
			Random r = new Random();
			foreach (var x in Nods)
			{
				AgregaVerticeAzar(x, r);
			}
		}

		public ListaPeso<Tuple<T, T>> Vecinos = new ListaPeso<Tuple<T, T>>();

		/// <summary>
		/// Devuelve la longitud de la ruta.
		/// </summary>
		public float Longitud(Ruta R)
		{
			float ret = 0f;
			for (int i = 0; i < R.Paso.Count - 1; i++)
			{
				ret += this[R.Paso[i], R.Paso[i + 1]];

			}
			return ret;
		}

		/// <summary>
		/// Devuelve o establece si la gráfica es bidireccional.
		/// </summary>
		public bool EsSimetrico = false;

		/// <summary>
		/// Devuelve la lista de vecinos de x (a todos los que apunta x)
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public List<T> Vecino(T x)
		{
			List<T> ret = new List<T>();
			T[] Nods = Nodos;
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
		public List<T> AntiVecino(T x)
		{
			List<T> ret = new List<T>();
			T[] Nods = Nodos;
			foreach (var y in Nods)
			{
				if (!float.IsPositiveInfinity(this[y, x]))
					ret.Add(y);
			}
			return ret;
		}

		/// <summary>
		/// Representa una ruta en un árbol.
		/// </summary>
		public class Ruta
		{
			public List<T> Paso;

			public static bool operator ==(Ruta left, Ruta right)
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

			public static bool operator !=(Ruta left, Ruta right)
			{
				return !(left == right);
			}

			public override bool Equals(object obj)
			{
				if (obj is Grafica<T>.Ruta)
				{
					Grafica<T>.Ruta Obj = (Grafica<T>.Ruta)obj;
					return this == Obj;
				}
				else
					return false;
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
			}
		}

		/// <summary>
		/// Devuelve un clon de la lista de nodos.
		/// </summary>
		public T[] Nodos
		{
			get
			{
				List<T> ret = new List<T>();
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
				return ret.ToArray();
			}
		}
		/*
		/// <summary>
		/// Agrega un nodo al árbol.
		/// </summary>
		/// <param name="nodo"></param>
		public Nodo AgregaNodo (T nodo)
		{
			Nodo ret = new Nodo(nodo);
			_Nodos.Add(ret);
			return ret;
		}
		 * */
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
					Vecinos[new Tuple<T, T>(y, x)] = value;
			}
		}

		/// <summary>
		/// Agrega un vértice entre dos nodos existentes a la gráfica.
		/// </summary>
		/// <param name="x">Un nodo.</param>
		/// <param name="y">Otro nodo.</param>
		/// <param name="Peso">El peso de la arista entre los nodos</param>
		public void AgregaVertice(T x, T y, float Peso)
		{
			{
				this[x, y] = Peso;
				//x.Vecinos[y] = Peso;
				//y.Vecinos[x] = Peso;
			}
		}

		/// <summary>
		/// Devuelve el número de nodos de esta gráfica.
		/// </summary>
		public int NumNodos
		{
			get
			{
				return Nodos.Length;
			}
		}

		/// <summary>
		/// Calcula la ruta óptima de un nodo a otro.
		/// </summary>
		/// <param name="x">Nodo inicial.</param>
		/// <param name="y">Nodo final.</param>
		/// <param name="Ignorar">Lista de nodos a evitar.</param>
		/// <returns>Devuelve la ruta de menor <c>Longitud</c>.</returns>
		/// <remarks>Puede ciclar si no existe ruta de x a y.</remarks> // TODO: Arreglar esto.
		public Ruta CaminoÓptimo(T x, T y, List<T> Ignorar)
		{
			Ruta ret = new Ruta();
			Ruta RutaBuscar;
			List<T> Ignora2;
			T[] tmp = { };


			if (x.Equals(y))
			{
				ret.Paso.Add(x);
				return ret;
			}
			// else
			foreach (var n in AntiVecino(y))
			{
				if (!Ignorar.Contains(n))
				{
					Ignorar.CopyTo(tmp);
					Ignora2 = new List<T>(tmp);

					RutaBuscar = CaminoÓptimo(x, n, Ignora2);
					RutaBuscar.Paso.Add(y);

					if (ret.Paso.Count > 0 && Longitud(ret) > Longitud(RutaBuscar)) ret = RutaBuscar;
				}
			}
			return ret;
		}

		/// <summary>
		/// Calcula la ruta óptima de un nodo a otro.
		/// </summary>
		/// <param name="x">Nodo inicial.</param>
		/// <param name="y">Nodo final.</param>
		/// <returns>Devuelve la ruta de menor <c>Longitud</c>.</returns>
		/// <remarks>Puede ciclar si no existe ruta de x a y.</remarks> // TODO: Arreglar esto.
		public Ruta CaminoÓptimo(T x, T y)
		{
			return CaminoÓptimo(x, y, new List<T>());
		}

		/// <summary>
		/// Genera una gráfica aleatoria.
		/// </summary>
		/// <param name="Nodos">El conjunto de nodos que se usarán.</param>
		/// <param name="r">El generador aleatorio.</param>
		/// <returns>Devuelve una gráfica aleatoria.</returns>
		public static Grafica<T> GeneraGraficaAleatoria(List<T> Nods)
		{
			Random r = new Random();
			if (Nods.Count < 2)
				throw new Exception("No se puede generar una gráfica aleatoria con menos de dos elementos.");
			Grafica<T> ret = new Grafica<T>();

			T v0, v1;
			v0 = Nods[0];
			v1 = Nods[1];
			Nods.RemoveAt(0);
			Nods.RemoveAt(0);

			ret.AgregaVertice(v0, v1, 1);

			foreach (var v in Nods)
			{
				ret.AgregaVerticeAzar(v, r);
			}
			return ret;
		}

		/// <summary>
		/// Agrega un vértice al grafo, generando aristas al azar a nodos antiguos.
		/// </summary>
		/// <param name="Vertice">Vertice.</param>
		public void AgregaVerticeAzar(T Vertice, Random r)
		{
			if (NumNodos == 0)
			{
				this[Vertice, Vertice] = 0;
				return;
			}

			// Genera la lista de probabilidad.
			// Obtener los pesos
			ListaPeso<T> Prob = new ListaPeso<T>();
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
			List<T> P = new List<T>(Prob.Keys);

			foreach (var x in P)
			{
				Prob[x] = Prob[x] / S;
			}

			// Seleccionar un vértice
			T v = SelecciónAzar(Prob, r);

			// Pues entonces hay que agregar arista de x a P[i];
			double p = r.NextDouble() + 0.5d;

			AgregaVertice(Vertice, v, (float)p);
		}

		/// <summary>
		/// Selecciona al azar un elemento.
		/// </summary>
		/// <param name="Prob">La función de probabilidad. ¡Debe estar normalizada!</param>
		/// <returns></returns>
		T SelecciónAzar(ListaPeso<T> Prob, Random r)
		{
			double q = r.NextDouble();
			foreach (var x in Prob.Keys)
			{
				if (q < Prob[x])
					return x;
				q -= Prob[x];
			}
			throw new Exception("No sé cómo llegó el algoritmo aquí D:");
			//return default(T);
		}



		/* // TODO: Esto...

		/// <summary>
		/// Selecciona al azar algunos nodos T.
		/// Con peso según su número de vecinos.
		/// </summary>
		/// <param name="NumSel">Cantidad de nodos a seleccionar.</param>
		/// <param name="r">Generador.</param>
		/// <returns>La lista de nodos seleccionados.</returns>
		List<T> SeleccionaAleatorio (int NumSel, Random r, List<T> lst)
		{
			ListaPeso<T> Pesos = new ListaPeso<T>();
			List<T> ret = new List<T>();

			foreach (var x in Nodos)
				{
					Pesos[x.Objeto] = 1/(float)Math.Pow(2, x.CantidadVecinos);
				}

            

			if (NumSel > NumNodos)
			{
				throw new Exception("No se pueden seleccionar más nodos de los que existen.");
			}

			List<T> m = new List<T>();
            

			foreach (var x in Nodos)
			{
				float prob = (NumSel - ret.Count) / num
			}
			while (ret.Count < NumNodos)
			{

			}

			return ret;
		}
		 */


		/// <summary>
		/// Selecciona pseudoaleatoriamente una sublista de tamaño fijo de una lista dada.
		/// </summary>
		/// <param name="r"></param>
		/// <param name="n">Número de elementos a seleccionar.</param>
		/// <param name="Lista">Lista de dónde seleccionar la sublista.</param>
		/// <returns>Devuelve una lista con los elementos seleccionados.</returns>
		List<object> SeleccionaPeso(Random r, int n, ListasExtra.ListaPeso<object> Lista)
		{
			List<object> ret;
			float Suma = 0;
			float rn;
			if (n == 0) return new List<object>();
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
	}

}
