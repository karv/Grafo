using System;
using System.Collections.Generic;
using ListasExtra;
using Graficas.Rutas;

namespace Graficas
{
	/// <summary>
	/// Representa una gráfica, en el sentido abstracto.
	/// Los nodos son del tipo <c>T</c>.
	/// </summary>
	public class Grafica<T> : IGraficaPeso<T> where T : IEquatable<T>
	{

		ICollection<T> IGrafica<T>.Vecinos(T nodo)
		{
			return Vecino(nodo).ToArray();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Graficas.Grafica`1"/> class.
		/// </summary>
		public Grafica()
		{
			Vecinos.Nulo = float.PositiveInfinity;
		}

		public ListaPeso<Tuple<T, T>> Vecinos = new ListaPeso<Tuple<T, T>>();

		/// <summary>
		/// Initializes a new instance of the <see cref="Graficas.Grafica`1"/> class.
		/// Crea una gráfica al azar
		/// </summary>
		/// <param name="Nods">Nodos de la gráfica</param>
		public Grafica(T[] Nods)
			: this()
		{
			Random r = new Random();
			foreach (var x in Nods)
			{
				AgregaVerticeAzar(x, r);
			}
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
		public List<T> AntiVecino(T x)
		{
			List<T> ret = new List<T>();
			IEnumerable<T> Nods = Nodos;
			foreach (var y in Nods)
			{
				if (!float.IsPositiveInfinity(this[y, x]))
					ret.Add(y);
			}
			return ret;
		}

		/*
		/// <summary>
		/// Representa una ruta en un árbol.
		/// </summary>
		public class Ruta: IRuta<T>
		{
			public List<T> Paso;

			#region IRuta implementation

			public IRuta<T> Reversa()
			{
				throw new NotImplementedException();
			}

			public T NodoInicial
			{
				get
				{
					return Paso[0];
				}
			}

			public T NodoFinal
			{
				get
				{
					return Paso[Paso.Count - 1];
				}
			}

			public float Longitud
			{
				get
				{
					throw new NotImplementedException();
				}
			}

			public int NumPasos
			{
				get
				{
					return Paso.Count;
				}
			}

			#endregion

			#region IEnumerable implementation

			public IEnumerator<T> GetEnumerator()
			{
				throw new NotImplementedException();
			}

			#endregion

			#region IEnumerable implementation

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			#endregion

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

*/
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

		public Graficas.Rutas.IRuta<T> toRuta(IEnumerable<T> seq)
		{
			throw new NotImplementedException();
		}

		#region IEquatable implementation

		public bool Equals(T other)
		{
			throw new NotImplementedException();
		}

		#endregion

		/// <summary>
		/// Calcula la ruta óptima de un nodo a otro.
		/// </summary>
		/// <param name="x">Nodo inicial.</param>
		/// <param name="y">Nodo final.</param>
		/// <param name="Ignorar">Lista de nodos a evitar.</param>
		/// <returns>Devuelve la ruta de menor <c>Longitud</c>.</returns>
		/// <remarks>Puede ciclar si no existe ruta de x a y.</remarks> // TODO: Arreglar esto.
		public Graficas.Rutas.IRuta<T> CaminoÓptimo(T x, T y, List<T> Ignorar)
		{
			//List<T> retLista = new List<T>();
			IRuta<T> ret = new Ruta<T>();
			IRuta<T> RutaBuscar;
			List<T> Ignora2;
			T[] tmp = { };


			if (x.Equals(y))
			{
				ConcatRuta(ret, x);
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
					ConcatRuta(RutaBuscar, y);
					//RutaBuscar.Concat(new Paso<T> ()  y);

					if (ret.NumPasos > 0 && ret.Longitud > RutaBuscar.Longitud)
						ret = RutaBuscar;
				}
			}
			return ret;
		}


		/// <summary>
		/// Concatena una ruta y un nodo
		/// </summary>
		/// <param name="ruta">Ruta.</param>
		/// <param name="nodo">Nodo.</param>
		public void ConcatRuta(IRuta<T> ruta, T nodo)
		{
			ruta.Concat(new Paso<T>(ruta.NodoFinal, nodo, this[ruta.NodoFinal, nodo]));
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

		ICollection<T> IGrafica<T>.Nodos
		{
			get
			{
				return Nodos;
			}
		}

		float IGraficaPeso<T>.Peso(T desde, T hasta)
		{
			return this[desde, hasta];
		}
	}

	public class GraficaNoPeso<T> : IGrafica<T> where T : IEquatable<T>
	{
		class Nodo
		{
			public T obj;
			public List<T> Vecinos = new List<T>();

			public Nodo(T nod)
			{
				obj = nod;
			}
		}

		/// <summary>
		/// Devuelve la lista de nodos.
		/// </summary>
		/// <returns>The nodos.</returns>
		public T[] getNodos()
		{
			int num = nodos.Count;
			T[] ret = new T[num];
			for (int i = 0; i < num; i++)
			{
				ret[i] = nodos[i].obj;
			}
			return ret;
		}

		List<Nodo> nodos = new List<Nodo>();

		ICollection<T> IGrafica<T>.Nodos
		{
			get
			{
				return getNodos();
			}
		}

		/// <summary>
		/// Devuelve la lista de vecinos de un nodo.
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		public ICollection<T> Vecinos(T nodo)
		{
			return nodos.Find(x => x.Equals(nodo)).Vecinos.ToArray();
		}

		/// <summary>
		/// Devuelve un arreglo con los vecinos de un nodo específico.
		/// </summary>
		/// <param name="nodo">Nodo.</param>
		public T[] this [T nodo]
		{
			get
			{
				return nodos.Find(x => x.obj.Equals(nodo)).Vecinos.ToArray();
			}
		}

		public IRuta<T> RutaOptima(T x, T y)
		{
			throw new NotImplementedException();
		}

		public void AgregaNodo(T nodo)
		{
			// Resiva si existe
			if (nodos.Exists(x => x.obj.Equals(nodo)))
				throw new Exception("Nodo ya existente.");
			nodos.Add(new Nodo(nodo));
		}

		/// <summary>
		/// Devuelve el nodo que le corresponde a un objeto tipo T.
		/// </summary>
		/// <returns>The nodo.</returns>
		/// <param name="nod">Nod.</param>
		Nodo getNodo(T nod)
		{
			return nodos.Find(x => x.obj.Equals(nod));
		}

		/// <summary>
		/// Agrega un vértice dado su origen y final.
		/// </summary>
		/// <param name="desde">Origen.</param>
		/// <param name="hasta">Destino.</param>
		public void AgregaVertice(T desde, T hasta)
		{
			getNodo(desde).Vecinos.Add(hasta);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Graficas.GraficaNoPeso`1"/> class.
		/// </summary>
		/// <param name="nods">Nodos de la gráfica.</param>
		public GraficaNoPeso(T[] nods)
			: this()
		{
			foreach (var x in nods)
			{
				AgregaNodo(x);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Graficas.GraficaNoPeso`1"/> class.
		/// </summary>
		public GraficaNoPeso()
		{
		}

		public IRuta<T> toRuta(IEnumerable<T> seq)
		{
			throw new NotImplementedException();
		}

	}


	[Serializable]
	public class NodoInexistenteException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:NodoInexistenteException"/> class
		/// </summary>
		public NodoInexistenteException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:NodoInexistenteException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		public NodoInexistenteException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:NodoInexistenteException"/> class
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		/// <param name="inner">The exception that is the cause of the current exception. </param>
		public NodoInexistenteException(string message, Exception inner)
			: base(message, inner)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:NodoInexistenteException"/> class
		/// </summary>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <param name="info">The object that holds the serialized object data.</param>
		protected NodoInexistenteException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{
		}
	}
}
