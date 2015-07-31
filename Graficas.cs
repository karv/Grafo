﻿using System;
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

		/// <summary>
		/// Initializes a new instance of the <see creVecinosicas.Grafica`1"/> class.
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
		public Grafica(T[] Nods)
			: this()
		{
			Random r = new Random();
			foreach (var x in Nods)
			{
				AgregaVerticeAzar(x, r);
			}
		}

		#endregion

		#region IGrafica

		float IGraficaPeso<T>.Peso(T desde, T hasta)
		{
			return this[desde, hasta];
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

		bool IGrafica<T>.esSimétrico
		{
			get
			{
				return esSimetrico;
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
				HashSet<T> ret = new HashSet<T>();
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
			this[aris.desde, aris.hasta] = 1;
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
				return Nodos.Count;
			}
		}

		public bool ExisteArista(IArista<T> aris)
		{
			return (this[aris.desde, aris.hasta] < float.PositiveInfinity);
		}

		public Graficas.Rutas.IRuta<T> toRuta(IEnumerable<T> seq)
		{
			throw new NotImplementedException();
		}


		#endregion

		#region Interno

		ListaPeso<Tuple<T, T>> Vecinos = new ListaPeso<Tuple<T, T>>();

		/// <summary>
		/// Devuelve o establece si la gráfica es bidireccional.
		/// </Vecinos>
		bool esSimetrico = false;

		#endregion

		#region Propios

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

		public bool EsSimétrico
		{
			set
			{
				esSimetrico = value;
			}
			get
			{
				return esSimetrico;
			}
		}

		/// Devuelve la lista de vecinos de x (a todos los que apunta x)
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
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
				if (esSimetrico)
					Vecinos[new Tuple<T, T>(y, x)] = value;//TODO hacer que essimetrico haga efecto al leer; no al escribir.
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
		Graficas.Rutas.IRuta<T> CaminoÓptimo(T x, T y, ISet<T> Ignorar)
		{
			//List<T> retLista = new List<T>();
			IRuta<T> ret = new Ruta<T>();
			IRuta<T> RutaBuscar;
			ISet<T> Ignora2;
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
					Ignorar.CopyTo(tmp, 0);
					Ignora2 = new HashSet<T>(tmp);

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

		/// <summary>
		/// Concatena una ruta y un nodo
		/// </summary>
		/// <param name="ruta">Ruta.</param>
		/// <param name="nodo">Nodo.</param>
		public void ConcatRuta(IRuta<T> ruta, T nodo)
		{
			ruta.Concat(new Paso<T>(ruta.NodoFinal, nodo, this[ruta.NodoFinal, nodo]));
		}


		#endregion

		#region Estáticos

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

		#endregion

	}
		
}
