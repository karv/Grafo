using System;
using System.Collections.Generic;
using Graficas.Rutas;
using ListasExtra;
using Graficas.Aristas;
using System.Linq;
using Graficas.Nodos;
using System.Runtime.InteropServices;

namespace Graficas.Grafo
{
	/// <summary>
	/// Clase común abstracta de Grafo
	/// </summary>
	[Serializable]
	public abstract class GrafoComún<T>
		where T : IEquatable<T>
	{
		/// <param name="simétrico">El grafo será construido simétrico</param>
		/// <param name="sóloLectura">El grafo es de sólo lectura</param>
		protected GrafoComún (bool simétrico = false, bool sóloLectura = false)
		{
			EsSimétrico = simétrico;
			SóloLectura = sóloLectura;
			Data = new HashSet<AristaBool<T>> ();
		}

		/// <summary>
		/// Colección de aristas
		/// </summary>
		protected ICollection<AristaBool<T>> Data { get; set; }

		/// <summary>
		/// Devuelve o establece si este grafo y sus aristas son de sólo lectura.
		/// </summary>
		public bool SóloLectura { get; }

		/// <summary>
		/// Es simétrico
		/// </summary>
		/// <value><c>true</c> si es simétrico; otherwise, <c>false</c>.</value>
		public bool EsSimétrico { get; }

		/// <summary>
		/// Elimina cada arista y nodo del grafo
		/// </summary>
		public void Clear ()
		{
			if (SóloLectura)
				throw new InvalidOperationException ("Grafo es sólo lectura.");
			Data.Clear ();
		}

		/// <summary>
		/// Revisa si existe una arista entre dos nodos.
		/// </summary>
		/// <returns><c>true</c>, si existe una arista, <c>false</c> otherwise.</returns>
		/// <param name="desde">Desde.</param>
		/// <param name="hasta">Hasta.</param>
		public bool ExisteArista (T desde, T hasta)
		{
			return AristaCoincide (desde, hasta).Existe;
		}

		/// <summary>
		/// Devuelve la arista coincidiente con un par de nodos dados.
		/// </summary>
		/// <param name="origen">Origen del nodo.</param>
		/// <param name="destino">Destino del nodo.</param>
		protected abstract AristaBool<T> AristaCoincide (T origen, T destino);

		/// <summary>
		/// Construye su subgrafo dados los nodos
		/// </summary>
		/// <param name="conjunto">Nodos</param>
		/// <param name="ret">Grafo que se convierte en un subgrafo.</param>
		protected void Subgrafo (IEnumerable<T> conjunto, GrafoComún<T> ret)
		{
			ret.Data.Clear ();
			foreach (var x in conjunto)
			{
				ret.Nodos.Add (x);
			}

			var arrConj = new List<T> (conjunto);
			for (int i = 0; i < arrConj.Count; i++)
			{
				for (int j = 0; j < arrConj.Count; j++)
				{
					AristaBool<T> aris = AristaCoincide (arrConj [i], arrConj [j]);
					if (aris != null)
						ret.Data.Add (aris);
				}
			}
		}

		/// <summary>
		/// Devuelve la lista de vecinos de x (a todos los que apunta x)
		/// </summary>
		/// <param name="x">Nodo</param>
		public ISet<T> Vecino (T x)
		{
			ISet<T> ret = new HashSet<T> ();
			IEnumerable<T> Nods = Nodos;
			foreach (var y in Data)
			{
				var ap = y.Antipodo (x);
				if (y.Coincide (x, ap))
					ret.Add (ap);
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
			foreach (var y in Data)
			{
				var ap = y.Antipodo (x);
				if (y.Coincide (ap, x))
					ret.Add (ap);
			}
			return ret;
		}


		/// <summary>
		/// Devuelve un clon de la lista de nodos.
		/// </summary>
		public ICollection<T> Nodos
		{
			get
			{
				var ret = new HashSet<T> (new NodosCollectionComparer<T> ());
				foreach (var x in Data)
					ret.UnionWith (x.ComoPar ().AsSet ());
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

		#region IGrafica


		/// <summary>
		/// Convierte una sucesión consistente de nodos en una ruta.
		/// </summary>
		/// <returns>The ruta.</returns>
		/// <param name="seq">Sucesión consistente</param>
		public IRuta<T> ToRuta (IEnumerable<T> seq)
		{
			var ret = new Ruta<T> ();
			bool iniciando = true;
			T last = default(T);
			foreach (var x in seq)
			{
				if (iniciando)
				{
					iniciando = false;
					ret = new Ruta<T> ();
				}
				else
				{
					if (ret.NumPasos == 0 || ret.NodoFinal.Equals (last))
						ret.Concat (AristaCoincide (last, x), last);
					else
					{
						// Invertir estado de la arista
						var ar = AristaCoincide (last, x);
						Data.Remove (ar);
						ar = AristaCoincide (x, last);
						ret.Concat (ar, last);
					}
				}
				last = x;
			}
			return ret;
		}

		#endregion

	}

	/// <summary>
	/// Representa una gráfica, en el sentido abstracto.
	/// Los nodos son del tipo <c>T</c>; y las aristas almacenan un valor del tipo <c>TData</c>
	/// </summary>
	[Serializable]
	public class Grafo<T, TData> : GrafoComún<T>, IGrafo<T>
		where T : IEquatable<T>
	{
		#region ctor

		/// <summary>
		/// Construye un Grafo de peso modificable
		/// </summary>
		/// <param name="simétrico">If set to <c>true</c> es simétrico.</param>
		public Grafo (bool simétrico = false)
			: base (simétrico, false)
		{
		}

		protected Grafo (bool simétrico, bool sóloLectura)
			: base (simétrico, sóloLectura)
		{
		}

		public Grafo (IGrafo<T> graf, bool sólolectura = true)
			: base (false, sólolectura)
		{
			foreach (var x in graf.Aristas ())
			{
				var par = x.ComoPar ().AsSet ();
				var n0 = par.PickRemove ();
				var n1 = par.PickRemove ();

				if (x.Coincide (n0, n1))
					Data.Add (new AristaPeso<T, TData> (n0, n1, sólolectura));

				if (x.Coincide (n1, n0))
					Data.Add (new AristaPeso<T, TData> (n1, n0, sólolectura));

			}
		}

		#endregion

		#region IGrafo

		IGrafo<T> IGrafo<T>.Subgrafo (IEnumerable<T> conjunto)
		{
			var ret = new Grafo<T, TData> (EsSimétrico, true);
			Subgrafo (conjunto, ret);
			return ret;
		}

		/// <summary>
		/// Devuelve una nueva colección con las aristas
		/// </summary>
		ICollection<IArista<T>> IGrafo<T>.Aristas ()
		{
			return new HashSet<IArista<T>> (Data.Cast<AristaPeso<T, TData>> ());
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

		IArista<T> IGrafo<T>.this [T desde, T hasta]
		{ 
			get
			{
				return EncuentraArista (desde, hasta);
			}
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
				var ar = EncuentraArista (x, y);
				return ar.Data; // Remark: verificación de existencia lo hace la propiedad Data
			}
			set
			{
				AristaPeso<T, TData> aris = EncuentraArista (x, y);
				aris.Existe = true;
				aris.Data = value;
			}
		}


		#endregion

		#region Común

		/// <summary>
		/// Devuelve la arista coincidiente con un par de nodos dados.
		/// </summary>
		/// <param name="origen">Origen del nodo.</param>
		/// <param name="destino">Destino del nodo.</param>
		/// <returns>The coincide.</returns>
		protected override AristaBool<T> AristaCoincide (T origen, T destino)
		{
			return EncuentraArista (origen, destino);
		}

		/// <summary>
		/// Clona las aristas y las agrega a un grafo.
		/// </summary>
		/// <param name="sóloLectura">Si el grafo que devuelve es de sólo lectura</param>
		/// <returns>Un grafo clón</returns>
		/// <remarks>Las aristas son clonadas y por lo tanto no se preserva referencia </remarks>
		public Grafo<T, TData> Clonar (bool sóloLectura = false) // TEST
		// TODO implementar en Grafo<T>
		{
			var ret = new Grafo<T, TData> (EsSimétrico, sóloLectura);
			foreach (var x in Data)
			{
				ret.Data.Add (new AristaPeso<T, TData> (
					x.Origen,
					x.Destino,
					x.SóloLectura,
					x.EsSimétrico)); 
			}
			return ret;
		}

		/// <summary>
		/// Devuelve un grafo preservando referencias.
		/// </summary>
		/// <returns>Un grafo sólo lectura clonado</returns>
		public Grafo<T, TData> ComoSóloLectura () // TEST
		// TODO implementar en Grafo<T>
		{
			var ret = new Grafo<T, TData> (EsSimétrico, true);
			foreach (var x in Data)
				ret.Data.Add (x);
			return ret;
		}

		#endregion

		#region Propios

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
			{
				aris = new AristaPeso<T, TData> (
					origen,
					destino,
					SóloLectura,
					EsSimétrico);
				Data.Add (aris);
			}
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
			foreach (var x in Data)
			{
				if (x.Coincide (origen, destino))
				{
					aris = x as AristaPeso<T, TData>;
					if (aris == null)
						throw new OperaciónAristaInválidaException ("Data interna corrpta en grafo.");
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
		                       ISet<T> ignorar)
		{
			var ret = new Ruta<T> ();
			Ruta<T> RutaBuscar;
			ISet<T> Ignora2;

			if (x.Equals (y))
				return ret; // Devuelve ruta vacía si origen == destino

			Ignora2 = new HashSet<T> (ignorar);
			Ignora2.Add (y);

			foreach (var n in AntiVecino(y))
			{
				if (!ignorar.Contains (n))
				{
					RutaBuscar = (Ruta<T>)CaminoÓptimo (x, n, peso, Ignora2);

					try
					{
						if (ret.NumPasos <= 0 || ret.Longitud (z => (peso ((AristaPeso<T, TData>)z))) > RutaBuscar.Longitud (z => (peso ((AristaPeso<T, TData>)z))))
						{
							if (RutaBuscar.NumPasos >= 0)
							{
								RutaBuscar.Concat (
									EncuentraArista (RutaBuscar.NodoFinal, y),
									RutaBuscar.NodoFinal);
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
					catch (Exception ex)
					{
						throw new Exception ("Error desconocido", ex);
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
	public class Grafo<T> : GrafoComún<T>, IGrafo<T>
		where T : IEquatable<T>
	{
		#region ctor

		/// <summary>
		/// Construye un Grafo de peso modificable
		/// </summary>
		/// <param name="simétrico">If set to <c>true</c> es simétrico.</param>
		public Grafo (bool simétrico = false)
			: base (simétrico, false)
		{
		}

		protected Grafo (bool simétrico, bool sóloLectura)
			: base (simétrico, sóloLectura)
		{
		}

		public Grafo (IGrafo<T> graf, bool sólolectura = true)
			: base (false, sólolectura)
		{
			foreach (var x in graf.Aristas ())
			{
				var par = x.ComoPar ().AsSet ();
				var n0 = par.PickRemove ();
				var n1 = par.PickRemove ();

				if (x.Coincide (n0, n1))
					Data.Add (new AristaBool<T> (n0, n1, sólolectura));

				if (x.Coincide (n1, n0))
					Data.Add (new AristaBool<T> (n1, n0, sólolectura));

			}
		}

		#endregion

		/// <summary>
		/// Clona las aristas y las agrega a un grafo.
		/// </summary>
		/// <param name="sóloLectura">Si el grafo que devuelve es de sólo lectura</param>
		/// <returns>Un grafo clón</returns>
		/// <remarks>Las aristas son clonadas y por lo tanto no se preserva referencia </remarks>
		public Grafo<T> Clonar (bool sóloLectura = false) // TEST
		{
			var ret = new Grafo<T> (EsSimétrico, sóloLectura);
			foreach (var x in Data)
			{
				ret.Data.Add (new AristaBool<T> (
					x.Origen,
					x.Destino,
					x.SóloLectura,
					x.EsSimétrico)); 
			}
			return ret;
		}

		/// <summary>
		/// Devuelve un grafo preservando referencias.
		/// </summary>
		/// <returns>Un grafo sólo lectura clonado</returns>
		public Grafo<T> ComoSóloLectura () // TEST
		{
			var ret = new Grafo<T> (EsSimétrico, true);
			foreach (var x in Data)
				ret.Data.Add (x);
			return ret;
		}

		#region IGrafica

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
			return new HashSet<AristaBool<T>> (Data);
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

		ICollection<IArista<T>> IGrafo<T>.Aristas ()
		{
			return new HashSet<IArista<T>> (Data);
		}

		#endregion

		#region Propios

		/// <summary>
		/// Devuelve la arista coincidiente con un par de nodos dados.
		/// </summary>
		/// <param name="origen">Origen del nodo.</param>
		/// <param name="destino">Destino del nodo.</param>
		/// <returns>The coincide.</returns>
		protected override AristaBool<T> AristaCoincide (T origen, T destino)
		{
			return EncuentraArista (origen, destino);
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
				if (SóloLectura)
					throw new OperaciónAristaInválidaException ("Grafo es sólo lectura.");
				AristaBool<T> aris;
				if (EncuentraArista (x, y, out aris))
					Data.Remove (aris);
				Data.Add (new AristaBool<T> (x, y, value, true, EsSimétrico));
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
			foreach (var x in Data)
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
		                       ISet<T> ignorar)
		{
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
					catch (Exception ex)
					{
						throw new Exception ("Error desconocido", ex);
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