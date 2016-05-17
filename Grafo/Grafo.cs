using System;
using System.Collections.Generic;
using Graficas.Rutas;
using ListasExtra;
using Graficas.Aristas;

namespace Graficas.Grafo
{
	/// <summary>
	/// Representa una gráfica, en el sentido abstracto.
	/// Los nodos son del tipo <c>T</c>; y las aristas almacenan un valor del tipo <c>TData</c>
	/// </summary>
	[Serializable]
	public class Grafo<T, TData> : IGrafo<T>
		where T : IEquatable<T>
	{
		#region ctor

		/// <summary>
		/// </summary>
		public Grafo ()
		{
		}

		#endregion

		#region IGrafica

		/// <summary>
		/// Clear this instance.
		/// </summary>
		public void Clear ()
		{
			if (SóloLectura)
				throw new InvalidOperationException ("Grafo es sólo lectura.");
			_data.Clear ();
		}

		IArista<T> IGrafo<T>.this [T desde, T hasta]
		{ 
			get
			{
				return EncuentraArista (desde, hasta);
			}
		}

		/// <summary>
		/// Calcula el subgrafo generado por un subconjutno de Nodos
		/// </summary>
		/// <param name="conjunto">Conjunto de nodos para calcular el subgrafo</param>
		/// <remarks>Hace una copia referenciada de las aristas </remarks>
		public Grafo<T, TData> Subgrafo (IEnumerable<T> conjunto)
		{
			var ret = new Grafo<T, TData> ();
			foreach (var x in conjunto)
			{
				ret.Nodos.Add (x);
			}

			foreach (var x in new List<T> (conjunto))
			{
				foreach (var y in new List<T> (conjunto))
				{
					AristaPeso<T, TData> aris;
					if (EncuentraArista (x, y, out aris))
						ret._data.Add (aris);
				}
			}
			return ret;
		}

		IGrafo<T> IGrafo<T>.Subgrafo (IEnumerable<T> conjunto)
		{
			return Subgrafo (conjunto);
		}

		/// <summary>
		/// Devuelve una colección con las aristas
		/// </summary>
		public ICollection<AristaPeso<T, TData>> Aristas ()
		{
			return new HashSet<AristaPeso<T, TData>> (_data);
		}

		ICollection<T> IGrafo<T>.Nodos
		{
			get
			{
				return Nodos;
			}
		}

		ICollection<T> IGrafo<T>.Vecinos (T nodo)
		{
			return Vecino (nodo);
		}

		/// <summary>
		/// Revisa si existe una arista entre dos nodos.
		/// </summary>
		/// <returns><c>true</c>, si existe una arista, <c>false</c> otherwise.</returns>
		/// <param name="desde">Desde.</param>
		/// <param name="hasta">Hasta.</param>
		public bool ExisteArista (T desde, T hasta)
		{
			return EncuentraArista (desde, hasta).Existe;
		}

		/// <summary>
		/// Agrega una arista (desde, hasta) con peso 1 a esta gráfica
		/// </summary>
		/// <param name="desde">Desde.</param>
		/// <param name="hasta">Hasta.</param>
		/// <returns>>Devuelve la arista agregada</returns>
		[Obsolete ("Usar EncuentraArista y asignarle valor.")]
		public AristaPeso<T, TData> AgregaArista (T desde, T hasta)
		{
			return EncuentraArista (desde, hasta);
		}

		/// <summary>
		/// Devuelve un clon de la lista de nodos.
		/// </summary>
		public ICollection<T> Nodos
		{
			get
			{
				var ret = new HashSet<T> ();
				foreach (var x in _data)
				{
					ret.Add (x.Origen);
					ret.Add (x.Destino);
				}
				return ret;
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

		/// <summary>
		/// Revisa si existe una arista consistente a una dada.
		/// </summary>
		/// <returns><c>true</c>, if arista was existed, <c>false</c> otherwise.</returns>
		/// <param name="aris">Aris.</param>
		public bool ExisteArista (IArista<T> aris)
		{
			return aris.Existe;
		}

		/// <summary>
		/// Convierte una sucesión consistente de nodos en una ruta.
		/// </summary>
		/// <returns>The ruta.</returns>
		/// <param name="seq">Sucesión consistente</param>
		public IRuta<T> ToRuta (IEnumerable<T> seq)
		{
			IRuta<T> ret = new Ruta<T> ();
			bool iniciando = true;
			T last = default(T);
			foreach (var x in seq)
			{
				if (iniciando)
				{
					iniciando = false;
					ret = new Ruta<T> (x);
				}
				else
				{
					ret.Concat (EncuentraArista (last, x));
				}
				last = x;
			}
			return ret;
		}

		ICollection<IArista<T>> IGrafo<T>.Aristas ()
		{
			return new HashSet<IArista<T>> (_data);
		}

		#endregion

		#region Interno

		HashSet<AristaPeso<T, TData>> _data = new HashSet<AristaPeso<T, TData>> ();
		//ListaPeso<T, T, TArista> Vecinos  = new ListaPeso<T, T, TArista> (null, )

		#endregion

		#region Propios

		/// <summary>
		/// Selecciona al azar un elemento.
		/// </summary>
		/// <param name="prob">La función de probabilidad. ¡Debe estar normalizada!</param>
		/// /// <param name="r">Aleatorio</param>
		/// <returns></returns>
		[Obsolete]
		static T SelecciónAzar (IDictionary<T, float> prob, Random r)
		{
			double q = r.NextDouble ();
			foreach (var x in prob.Keys)
			{
				if (q < prob [x])
					return x;
				q -= prob [x];
			}
			throw new System.Exception ("No sé cómo llegó el algoritmo aquí D:");
		}

		bool _esSimétrico;

		/// <summary>
		/// Es simétrico
		/// </summary>
		/// <value><c>true</c> si es simétrico; otherwise, <c>false</c>.</value>
		public bool EsSimétrico
		{
			get
			{
				return _esSimétrico;
			}
			set
			{
				if (SóloLectura)
					throw new InvalidOperationException ("Grafo es sólo lectura.");
				_esSimétrico = value;
			}
		}

		/// <summary>
		/// Devuelve o establece si este grafo y sus aristas son de sólo lectura.
		/// </summary>
		public bool SóloLectura { get; set; }

		/// <summary>
		/// Devuelve la lista de vecinos de x (a todos los que apunta x)
		/// </summary>
		/// <param name="x">Nodo</param>
		public ISet<T> Vecino (T x)
		{
			ISet<T> ret = new HashSet<T> ();
			IEnumerable<T> Nods = Nodos;
			foreach (var y in _data)
			{
				if (y.Origen.Equals (x))
					ret.Add (y.Destino);
			}
			return ret;
		}

		/// <summary>
		/// Devuelve la lista de antivecinos de x (todos los que apuntan a x)
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public ISet<T> AntiVecino (T x)
		{
			ISet<T> ret = new HashSet<T> ();
			IEnumerable<T> Nods = Nodos;
			foreach (var y in _data)
			{
				if (y.Destino.Equals (x))
					ret.Add (y.Origen);
			}
			return ret;
		}

		/// <summary>
		/// Devuelve o establece el peso de la arista que une dos vértices.
		/// </summary>
		/// <param name="x">Vértice origen.</param>
		/// <param name="y">Vértice destino.</param>
		/// <returns>Devuelve el peso de la arista que une estos nodos. <see cref="float.PositiveInfinity"/> si no existe arista.</returns>
		public TData this [T x, T y]
		{
			get
			{
				return EncuentraArista (x, y).Data;
			}
			set
			{
				AristaPeso<T ,TData> aris;
				if (EncuentraArista (x, y, out aris))
					_data.Remove (aris);
				_data.Add (new AristaPeso<T, TData> (x, y, value, SóloLectura));
			}
		}

		/// <summary>
		/// Devuelve la Arista? con extremos dados.
		/// </summary>
		/// <returns>Devuelve la arista, posiblemente inexistente.</returns>
		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		public AristaPeso<T, TData> EncuentraArista (T origen, T destino)
		{
			AristaPeso<T ,TData> aris;
			if (!EncuentraArista (origen, destino, out aris))
				aris = new AristaPeso<T, TData> (origen, destino, SóloLectura);
			return aris;
		}

		/// <summary>
		/// Revisa si hay una arista, y la devuelve
		/// </summary>
		/// <returns><c>true</c>, si hay una arista en _data. <c>false</c> otherwise.</returns>
		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		/// <param name="aris">Por aquí devuelve la arista si hay, null si no</param>
		protected bool EncuentraArista (T origen,
		                                T destino,
		                                out AristaPeso<T, TData> aris)
		{
			foreach (var x in _data)
			{
				if (x.Origen.Equals (origen) && x.Destino.Equals (destino))
				{
					aris = x;
					return true;
				}				
				if (EsSimétrico && (x.Origen.Equals (destino) && x.Destino.Equals (origen)))
				{
					aris = x;
					return true;
				}			
			}
			aris = null;
			return false;

		}

		/// <summary>
		/// Devuelve a ruta de menor longitud entre dos puntos.
		/// </summary>
		/// <returns>The óptimo.</returns>
		/// <param name="x">Origen</param>
		/// <param name="y">Destino</param>
		/// <param name="peso">Forma de asignar peso a cada arista</param>
		public IRuta<T> CaminoÓptimo (T x,
		                              T y,
		                              Func<AristaPeso<T, TData>, float> peso)
		{
			return CaminoÓptimo (x, y, peso, new HashSet<T> ());
		}

		/// <summary>
		/// Calcula la ruta óptima de un nodo a otro.
		/// </summary>
		/// <param name="x">Nodo inicial.</param>
		/// <param name="y">Nodo final.</param>
		/// <param name="ignorar">Lista de nodos a evitar.</param>
		/// <param name="peso">Forma de asignar peso a cada arista</param>
		/// <returns>Devuelve la ruta de menor <c>Longitud</c>.</returns>
		/// <remarks>Puede ciclar si no existe ruta de x a y.</remarks> 
		/// <remarks>Devuelve ruta vacía (no nula) si origen es destino </remarks>
		IRuta<T> CaminoÓptimo (T x,
		                       T y,
		                       Func<AristaPeso<T, TData>, float> peso,
		                       ISet<T> ignorar) // TODO FIX
		{
			//List<T> retLista = new List<T>();
			IRuta<T> ret = new Ruta<T> ();
			IRuta<T> RutaBuscar;
			ISet<T> Ignora2;

			if (x.Equals (y))
				return ret; // Devuelve ruta vacía si origen == destino

			Ignora2 = new HashSet<T> (ignorar);
			Ignora2.Add (y);

			foreach (var n in AntiVecino(y))
			{
				if (!ignorar.Contains (n))
				{
					RutaBuscar = CaminoÓptimo (x, n, peso, Ignora2);

					try
					{
						if (ret.NumPasos <= 0 || ret.Longitud (z => (peso ((AristaPeso<T, TData>)z))) > RutaBuscar.Longitud (z => (peso ((AristaPeso<T, TData>)z))))
						{
							if (RutaBuscar.NumPasos >= 0)
							{
								RutaBuscar.Concat (EncuentraArista (RutaBuscar.NodoFinal, y));
								ret = RutaBuscar;
							}
						}
					}
					catch (InvalidCastException ex)
					{
						throw new InvalidCastException (
							"Error haciendo cast de aristas al calcular camino óptimo.",
							ex);
					}
					catch (System.Exception ex)
					{
						throw new System.Exception ("Error desconocido", ex);
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
		List<object> SeleccionaPeso (Random r, int n, ListaPeso<object> lista)
		{
			List<object> ret;
			float Suma = 0;
			float rn;
			if (n == 0)
				return new List<object> ();
			else
			{
				ret = SeleccionaPeso (r, n - 1, lista);

				foreach (var x in ret)
				{
					lista [x] = 0;
				}

				// Ahora seleecionar uno.
				Suma = 0;
				rn = (float)r.NextDouble () * lista.SumaTotal ();

				foreach (var x in lista.Keys)
				{
					Suma += lista [x];
					if (Suma >= rn)
					{
						ret.Add (x);
						return ret;
					}
				}
				return null;
			}
		}

		#endregion
	}

	/// <summary>
	/// Representa una gráfica, en el sentido abstracto.
	/// Los nodos son del tipo <c>T</c>.
	/// </summary>
	public class Grafo<T> : IGrafo<T>
		where T : IEquatable<T>
	{
		#region ctor

		/// <summary>
		/// </summary>
		public Grafo ()
		{
		}

		#endregion

		#region IGrafica

		/// <summary>
		/// Clear this instance.
		/// </summary>
		public void Clear ()
		{
			if (SóloLectura)
				throw new InvalidOperationException ("Grafo es sólo lectura.");
			_data.Clear ();
		}

		IArista<T> IGrafo<T>.this [T desde, T hasta]
		{ 
			get
			{
				return EncuentraArista (desde, hasta);
			}
		}

		/// <summary>
		/// Calcula el subgrafo generado por un subconjutno de Nodos
		/// </summary>
		/// <param name="conjunto">Conjunto de nodos para calcular el subgrafo</param>
		public Grafo<T> Subgrafo (IEnumerable<T> conjunto)
		{
			var ret = new Grafo<T> ();
			foreach (var x in conjunto)
			{
				ret.Nodos.Add (x);
			}

			foreach (var x in new List<T> (conjunto))
			{
				foreach (var y in new List<T> (conjunto))
				{
					ret [x, y] = this [x, y];
				}
			}
			return ret;
		}

		IGrafo<T> IGrafo<T>.Subgrafo (IEnumerable<T> conjunto)
		{
			return Subgrafo (conjunto);
		}

		/// <summary>
		/// Devuelve una colección con las aristas
		/// </summary>
		public ICollection<AristaBool<T>> Aristas ()
		{
			return new HashSet<AristaBool<T>> (_data);
		}

		ICollection<T> IGrafo<T>.Nodos
		{
			get
			{
				return Nodos;
			}
		}

		ICollection<T> IGrafo<T>.Vecinos (T nodo)
		{
			return Vecino (nodo);
		}

		/// <summary>
		/// Revisa si existe una arista entre dos nodos.
		/// </summary>
		/// <returns><c>true</c>, si existe una arista, <c>false</c> otherwise.</returns>
		/// <param name="desde">Desde.</param>
		/// <param name="hasta">Hasta.</param>
		public bool ExisteArista (T desde, T hasta)
		{
			return this [desde, hasta];
		}

		/// <summary>
		/// Devuelve un clon de la lista de nodos.
		/// </summary>
		public ICollection<T> Nodos
		{
			get
			{
				var ret = new HashSet<T> ();
				foreach (var x in _data)
				{
					ret.Add (x.Origen);
					ret.Add (x.Destino);
				}
				return ret;
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

		/// <summary>
		/// Revisa si existe una arista consistente a una dada.
		/// </summary>
		/// <returns><c>true</c>, if arista was existed, <c>false</c> otherwise.</returns>
		/// <param name="aris">Aris.</param>
		public bool ExisteArista (IArista<T> aris)
		{
			return aris.Existe;
		}

		/// <summary>
		/// Convierte una sucesión consistente de nodos en una ruta.
		/// </summary>
		/// <returns>The ruta.</returns>
		/// <param name="seq">Sucesión consistente</param>
		public IRuta<T> ToRuta (IEnumerable<T> seq)
		{
			IRuta<T> ret = new Ruta<T> ();
			bool iniciando = true;
			T last = default(T);
			foreach (var x in seq)
			{
				if (iniciando)
				{
					iniciando = false;
					ret = new Ruta<T> (x);
				}
				else
				{
					ret.Concat (EncuentraArista (last, x));
				}
				last = x;
			}
			return ret;
		}

		ICollection<IArista<T>> IGrafo<T>.Aristas ()
		{
			return new HashSet<IArista<T>> (_data);
		}

		#endregion

		#region Interno

		HashSet<AristaBool<T>> _data = new HashSet<AristaBool<T>> ();
		//ListaPeso<T, T, TArista> Vecinos  = new ListaPeso<T, T, TArista> (null, )

		#endregion

		#region Propios

		/// <summary>
		/// Selecciona al azar un elemento.
		/// </summary>
		/// <param name="prob">La función de probabilidad. ¡Debe estar normalizada!</param>
		/// /// <param name="r">Aleatorio</param>
		/// <returns></returns>
		[Obsolete]
		static T SelecciónAzar (IDictionary<T, float> prob, Random r)
		{
			double q = r.NextDouble ();
			foreach (var x in prob.Keys)
			{
				if (q < prob [x])
					return x;
				q -= prob [x];
			}
			throw new System.Exception ("No sé cómo llegó el algoritmo aquí D:");
		}

		bool _esSimétrico;

		/// <summary>
		/// Es simétrico
		/// </summary>
		/// <value><c>true</c> si es simétrico; otherwise, <c>false</c>.</value>
		public bool EsSimétrico
		{
			get
			{
				return _esSimétrico;
			}
			set
			{
				if (SóloLectura)
					throw new InvalidOperationException ("Grafo es sólo lectura.");
				_esSimétrico = value;
			}
		}

		/// <summary>
		/// Devuelve o establece si este grafo y sus aristas son de sólo lectura.
		/// </summary>
		public bool SóloLectura { get; set; }

		/// <summary>
		/// Devuelve la lista de vecinos de x (a todos los que apunta x)
		/// </summary>
		/// <param name="x">Nodo</param>
		public ISet<T> Vecino (T x)
		{
			ISet<T> ret = new HashSet<T> ();
			IEnumerable<T> Nods = Nodos;
			foreach (var y in _data)
			{
				if (y.Origen.Equals (x))
					ret.Add (y.Destino);
			}
			return ret;
		}

		/// <summary>
		/// Devuelve la lista de antivecinos de x (todos los que apuntan a x)
		/// </summary>
		/// <param name="x"></param>
		/// <returns></returns>
		public ISet<T> AntiVecino (T x)
		{
			ISet<T> ret = new HashSet<T> ();
			IEnumerable<T> Nods = Nodos;
			foreach (var y in _data)
			{
				if (y.Destino.Equals (x))
					ret.Add (y.Origen);
			}
			return ret;
		}

		/// <summary>
		/// Devuelve o establece el peso de la arista que une dos vértices.
		/// </summary>
		/// <param name="x">Vértice origen.</param>
		/// <param name="y">Vértice destino.</param>
		/// <returns>Devuelve el peso de la arista que une estos nodos. <see cref="float.PositiveInfinity"/> si no existe arista.</returns>
		public bool this [T x, T y]
		{
			get
			{
				return EncuentraArista (x, y).Existe;
			}
			set
			{
				AristaBool<T> aris;
				if (EncuentraArista (x, y, out aris))
					_data.Remove (aris);
				_data.Add (new AristaBool<T> (x, y, value, true));
			}
		}

		/// <summary>
		/// Devuelve la Arista? con extremos dados.
		/// </summary>
		/// <returns>Devuelve la arista, posiblemente inexistente.</returns>
		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		public AristaBool<T> EncuentraArista (T origen, T destino)
		{
			AristaBool<T> aris;
			if (!EncuentraArista (origen, destino, out aris))
				aris = new AristaBool<T> (origen, destino, false, SóloLectura);
			return aris;
		}

		/// <summary>
		/// Revisa si hay una arista, y la devuelve
		/// </summary>
		/// <returns><c>true</c>, si hay una arista en _data. <c>false</c> otherwise.</returns>
		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		/// <param name="aris">Por aquí devuelve la arista si hay, null si no</param>
		protected bool EncuentraArista (T origen,
		                                T destino,
		                                out AristaBool<T> aris)
		{
			foreach (var x in _data)
			{
				if (x.Origen.Equals (origen) && x.Destino.Equals (destino))
				{
					aris = x;
					return true;
				}				
				if (EsSimétrico && (x.Origen.Equals (destino) && x.Destino.Equals (origen)))
				{
					aris = x;
					return true;
				}			
			}
			aris = null;
			return false;

		}

		/// <summary>
		/// Devuelve a ruta de menor longitud entre dos puntos.
		/// </summary>
		/// <returns>The óptimo.</returns>
		/// <param name="x">Origen</param>
		/// <param name="y">Destino</param>
		public IRuta<T> CaminoÓptimo (T x, T y)
		{
			return CaminoÓptimo (x, y, new HashSet<T> ());
		}

		/// <summary>
		/// Calcula la ruta óptima de un nodo a otro.
		/// </summary>
		/// <param name="x">Nodo inicial.</param>
		/// <param name="y">Nodo final.</param>
		/// <param name="ignorar">Lista de nodos a evitar.</param>
		/// <returns>Devuelve la ruta de menor <c>Longitud</c>.</returns>
		/// <remarks>Puede ciclar si no existe ruta de x a y.</remarks> 
		/// <remarks>Devuelve ruta vacía (no nula) si origen es destino </remarks>
		IRuta<T> CaminoÓptimo (T x,
		                       T y,
		                       ISet<T> ignorar) // TODO FIX
		{
			//List<T> retLista = new List<T>();
			IRuta<T> ret = new Ruta<T> ();
			IRuta<T> RutaBuscar;
			ISet<T> Ignora2;

			if (x.Equals (y))
				return ret; // Devuelve ruta vacía si origen == destino

			Ignora2 = new HashSet<T> (ignorar);
			Ignora2.Add (y);

			foreach (var n in AntiVecino(y))
			{
				if (!ignorar.Contains (n))
				{
					RutaBuscar = CaminoÓptimo (x, n, Ignora2);

					try
					{
						if (ret.NumPasos <= 0 || ret.NumPasos > RutaBuscar.NumPasos)
						{
							if (RutaBuscar.NumPasos >= 0)
							{
								RutaBuscar.Concat (EncuentraArista (RutaBuscar.NodoFinal, y));
								ret = RutaBuscar;
							}
						}
					}
					catch (InvalidCastException ex)
					{
						throw new InvalidCastException (
							"Error haciendo cast de aristas al calcular camino óptimo.",
							ex);
					}
					catch (System.Exception ex)
					{
						throw new System.Exception ("Error desconocido", ex);
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
		List<object> SeleccionaPeso (Random r, int n, ListaPeso<object> lista)
		{
			List<object> ret;
			float Suma = 0;
			float rn;
			if (n == 0)
				return new List<object> ();
			else
			{
				ret = SeleccionaPeso (r, n - 1, lista);

				foreach (var x in ret)
				{
					lista [x] = 0;
				}

				// Ahora seleecionar uno.
				Suma = 0;
				rn = (float)r.NextDouble () * lista.SumaTotal ();

				foreach (var x in lista.Keys)
				{
					Suma += lista [x];
					if (Suma >= rn)
					{
						ret.Add (x);
						return ret;
					}
				}
				return null;
			}
		}

		#endregion
	}
}