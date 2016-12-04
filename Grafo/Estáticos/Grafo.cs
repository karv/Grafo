using System;
using System.Collections.Generic;
using Graficas.Rutas;
using ListasExtra;
using Graficas.Aristas;
using System.Linq;

namespace Graficas.Grafo.Estáticos
{
	/// <summary>
	/// Clase común abstracta de Grafo
	/// </summary>
	[Serializable]
	public abstract class GrafoComún<T>
	{
		#region Ctor

		/// <param name="simétrico">El grafo será construido simétrico</param>
		/// <param name="sóloLectura">El grafo es de sólo lectura</param>
		/// <param name="nodos">Conjunto de nodos</param>
		protected GrafoComún (ICollection<T> nodos,
		                      bool simétrico = false,
		                      bool sóloLectura = false)
		{
			EsSimétrico = simétrico;
			SóloLectura = sóloLectura;
			IntNodos = new T[nodos.Count];

			// Copiar la info de nodos a IntNodo
			int i = 0;
			foreach (var x in nodos)
				IntNodos [i++] = x;

			Data = new AristaBool<T>[NumNodos, NumNodos];
			inicializaData ();
		}

		void inicializaData ()
		{

			for (int i = 0; i < NumNodos; i++)
				for (int j = 0; j < NumNodos; j++)
				{
					var aris = ConstruirNuevaArista (IntNodos [i], IntNodos [j]);
					Data [i, j] = aris;
				}
		}

		#endregion

		#region Control

		/// <summary>
		/// Construye una nueva arista para utilizarla al asignar las rutas de Data
		/// </summary>
		/// <returns>The nueva arista.</returns>
		/// <param name="origen">Nodo origen</param>
		/// <param name="destino">Nodo destino</param>
		protected abstract AristaBool<T> ConstruirNuevaArista (T origen, T destino);

		/// <summary>
		/// Establece a cada arista como inexistente.
		/// </summary>
		protected virtual void ClearData ()
		{
			foreach (var x in Data)
				x.Existe = false;
		}

		/// <summary>
		/// Devuelve la arista con extremos de índices dados.
		/// </summary>
		/// <returns>La arista</returns>
		/// <param name="origen">Índice del origen</param>
		/// <param name="destino">Índice del destino</param>
		protected AristaBool<T> AdyacenciaÍndice (int origen, int destino)
		{
			if (destino < origen || !EsSimétrico)
				return Data [origen, destino];
			return Data [destino, origen];
		}

		/// <summary>
		/// Devuelve el índice de un nodo en el arreglo de nodos
		/// </summary>
		/// <returns>Un entero que representa el índice del nodo dado.</returns>
		/// <param name="nodo">Nodo del grafo.</param>
		protected int ÍndiceDe (T nodo)
		{
			return Array.FindIndex (IntNodos, z => Comparador.Equals (nodo, z));
		}

		/// <summary>
		/// Devuelve la arista coincidiente con un par de nodos dados.
		/// </summary>
		/// <param name="origen">Origen del nodo.</param>
		/// <param name="destino">Destino del nodo.</param>
		protected abstract AristaBool<T> AristaCoincide (T origen, T destino);

		#endregion

		#region Data

		/// <summary>
		/// Devuelve el comparador que se usa para los nodos.
		/// </summary>
		public readonly IEqualityComparer<T> Comparador = EqualityComparer<T>.Default;

		/// <summary>
		/// Colección de aristas
		/// </summary>
		protected AristaBool<T>[,] Data { get; set; }

		/// <summary>
		/// Los nodos del grafo.
		/// </summary>
		protected T[] IntNodos { get; }

		#endregion

		#region Propiedades

		/// <summary>
		/// Devuelve una lista sólo lectura con los nodos del grafo.
		/// </summary>
		public ICollection<T> Nodos
		{
			get
			{
				return new List<T> (IntNodos).AsReadOnly ();
			}
		}

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
		/// Devuelve el número de nodos de esta gráfica.
		/// </summary>
		public int NumNodos
		{
			get
			{
				return Nodos.Count;
			}
		}

		#endregion

		#region General

		/// <summary>
		/// Elimina las aristas sin modificar referencias.
		/// </summary>
		public void Clear ()
		{
			if (SóloLectura)
				throw new InvalidOperationException ("Grafo es sólo lectura.");
			ClearData ();
			AlLimpiar?.Invoke ();
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
		/// Devuelve la Arista? con extremos dados.
		/// </summary>
		/// <returns>Devuelve la arista, posiblemente inexistente.</returns>
		/// <param name="origen">Origen.</param>
		/// <param name="destino">Destino.</param>
		public AristaBool<T> EncuentraArista (T origen, T destino)
		{
			int index0;
			int index1;

			int indexOri = ÍndiceDe (origen);
			int indexDes = ÍndiceDe (destino);

			if (EsSimétrico)
			{
				index1 = Math.Min (indexOri, indexDes);
				index0 = Math.Max (indexOri, indexDes);
			}
			else
			{
				index0 = indexOri;
				index1 = indexDes;
			}

			if (index0 == -1 || index1 == -1)
				throw new NodoInexistenteException ();
			return Data [index0, index1];
		}

		/// <summary>
		/// Devuelve la lista de vecinos de x (a todos los que apunta x)
		/// </summary>
		/// <param name="x">Nodo</param>
		public ISet<T> Vecino (T x)
		{
			ISet<T> ret = new HashSet<T> ();
			var ix = ÍndiceDe (x);
			for (int i = 0; i < NumNodos; i++)
			{
				var ar = AdyacenciaÍndice (ix, i);
				if (ar.Existe)
					ret.Add (IntNodos [i]);
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
			var ix = ÍndiceDe (x);
			for (int i = 0; i < NumNodos; i++)
			{
				var ar = AdyacenciaÍndice (i, ix);
				if (ar.Existe)
					ret.Add (IntNodos [i]);
			}

			return ret;
		}

		/// <summary>
		/// Convierte una sucesión consistente de nodos en una ruta.
		/// </summary>
		/// <returns>The ruta.</returns>
		/// <param name="seq">Sucesión consistente</param>
		public IRuta<T> ToRuta (IEnumerable<T> seq)
		{
			var ret = new Ruta<T> ();
			bool iniciando = true; // Flag que indica que está construyendo el primer nodo (no paso)
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
					var ar = AristaCoincide (last, x);
					if (!ar.Existe)
						throw new RutaInconsistenteException ("La sucesión dada no representa una ruta.");
					ret.Concat (ar, last);

				}
				last = x;
			}
			return ret;
		}

		/// <summary>
		/// Devuelve una colección con las aristas
		/// </summary>
		public ICollection<AristaBool<T>> Aristas ()
		{
			var ret = new HashSet<AristaBool<T>> ();
			for (int i = 0; i < NumNodos; i++)
			// Si es simétrico, no repetir aristas.
				for (int j = 0; j < (EsSimétrico ? i + 1 : NumNodos); j++)
					if (Data [i, j].Existe)
						ret.Add (Data [i, j]);
			return ret;
		}

		#endregion

		#region Eventos

		/// <summary>
		/// Se ejecuta al ejecutar Clear ()
		/// </summary>
		public event Action AlLimpiar;

		#endregion
	}

	/// <summary>
	/// Representa una gráfica, en el sentido abstracto.
	/// Los nodos son del tipo <c>T</c>; y las aristas almacenan un valor del tipo <c>TData</c>
	/// </summary>
	[Serializable]
	public class Grafo<T, TData> : GrafoComún<T>, IGrafo<T>
	{
		#region ctor

		/// <summary>
		/// Construye un Grafo de peso modificable
		/// </summary>
		/// <param name="nodos">Colección de nodos del grafo</param>
		/// <param name="simétrico">If set to <c>true</c> es simétrico.</param>
		public Grafo (ICollection<T> nodos, bool simétrico = false)
			: base (nodos, simétrico, false)
		{
		}

		/// <summary>
		/// Construye un Grafo de peso
		/// </summary>
		/// <param name="nodos">Colección de nodos del grafo</param>
		/// <param name="simétrico">If set to <c>true</c> es simétrico.</param>
		/// <param name="sóloLectura">If set to <c>true</c> sólo lectura.</param>
		protected Grafo (ICollection<T> nodos, bool simétrico, bool sóloLectura)
			: base (nodos, simétrico, sóloLectura)
		{
		}

		/// <summary>
		/// Clonae un Grafo
		/// </summary>
		/// <param name="sóloLectura">If set to <c>true</c> sólo lectura.</param>
		/// <param name="graf">Grafo a clonar</param>
		public Grafo (IGrafo<T> graf, bool sóloLectura = true)
			: base (graf.Nodos, false, sóloLectura)
		{
			for (int i = 0; i < NumNodos; i++)
				for (int j = 0; j < NumNodos; j++)
				{
					var ori = IntNodos [i];
					var des = IntNodos [j];
					var ari = graf [ori, des].Existe;
					if (ari)
						Data [i, j] = new AristaPeso<T, TData> (
							ori,
							des, 
							default(TData),
							sóloLectura);
					else
						Data [i, j] = new AristaPeso<T, TData> (
							ori,
							des, 
							sóloLectura);
				}
			

		}

		#endregion

		#region IGrafo

		IGrafo<T> IGrafo<T>.Subgrafo (IEnumerable<T> conjunto)
		{
			return Subgrafo (conjunto);
		}

		/// <summary>
		/// Calcula el subgrafo generado por un subconjutno de Nodos
		/// </summary>
		/// <param name="conjunto">Conjunto de nodos para calcular el subgrafo</param>
		/// <remarks>No construye nuevas aristas, por lo que preserva refs.</remarks>
		public Grafo<T, TData> Subgrafo (IEnumerable<T> conjunto)
		{
			if (conjunto.Any (z => !Nodos.Contains (z)))
				throw new ArgumentException ("Nodos no es un cobcojunto", "conjunto");
			var ret = new Grafo<T, TData> (
				          new HashSet<T> (conjunto),
				          EsSimétrico,
				          SóloLectura);

			for (int i = 0; i < ret.NumNodos; i++)
			{
				// ii  es el índice en this que contiene al i-ésimo elemento de ret.Nodos
				var ii = ret.ÍndiceDe (ret.IntNodos [i]); 
				for (int j = 0; j < ret.NumNodos; j++)
				{
					// ij  es el índice en this que contiene al j-ésimo elemento de ret.Nodos
					var ij = ret.ÍndiceDe (ret.IntNodos [j]); 
					ret.Data [i, j] = Data [ii, ij];
				}
			}
			return ret;
		}

		/// <summary>
		/// Devuelve una nueva colección con las aristas existentes
		/// </summary>
		ICollection<IArista<T>> IGrafo<T>.Aristas ()
		{
			return new HashSet<IArista<T>> (Data.Cast<AristaPeso<T, TData>> ().Where (x => x.Existe));
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
				if (SóloLectura)
					throw new OperaciónAristaInválidaException ("Grafo es sólo lectura.");
				AristaPeso<T, TData> aris = EncuentraArista (x, y);
				aris.Existe = true;
				aris.Data = value;
			}
		}


		#endregion

		#region Común

		/// <summary>
		/// Construye una nueva arista para utilizarla al asignar las rutas de Data.
		/// </summary>
		/// <returns>Devuelve una AristaPeso correspondiente.</returns>
		/// <param name="origen">Nodo origen</param>
		/// <param name="destino">Nodo destino</param>
		protected override AristaBool<T> ConstruirNuevaArista (T origen, T destino)
		{
			return new AristaPeso<T ,TData> (origen, destino, SóloLectura, EsSimétrico);
		}

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
		public Grafo<T, TData> Clonar (bool sóloLectura = false)
		{
			var ret = new Grafo<T, TData> (Nodos, EsSimétrico, sóloLectura);
			for (int i = 0; i < NumNodos; i++)
				for (int j = 0; j < (EsSimétrico ? i + 1 : NumNodos); j++)
				{
					var x = Data [i, j] as AristaPeso<T, TData>; // La arista iterando
					if (x.Existe)
					{
						ret.Data [i, j] = new AristaPeso<T, TData> (
							x.Origen,
							x.Destino,
							x.Data,
							x.SóloLectura,
							x.EsSimétrico);
					}
				}
			return ret;
		}

		/// <summary>
		/// Devuelve un grafo preservando referencias.
		/// </summary>
		/// <returns>Un grafo sólo lectura clonado</returns>
		public Grafo<T, TData> ComoSóloLectura ()
		{
			var ret = new Grafo<T, TData> (Nodos, EsSimétrico, true);
			for (int i = 0; i < NumNodos; i++)
				for (int j = 0; j < (EsSimétrico ? i + 1 : NumNodos); j++)
					ret.Data [i, j] = Data [i, j];
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
		public new AristaPeso<T, TData> EncuentraArista (T origen, T destino)
		{
			return base.EncuentraArista (origen, destino) as AristaPeso<T, TData>;
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
			if (x.Equals (y))
				return null;
			var ign = new HashSet<T> ();
			ign.Add (x);
			ign.Add (y);
			return CaminoÓptimo (x, y, peso, ign);
		}

		/// <summary>
		/// Calcula la ruta óptima de un nodo a otro.
		/// </summary>
		/// <param name="x">Nodo inicial.</param>
		/// <param name="y">Nodo final.</param>
		/// <param name="ignorar">Lista de nodos a evitar.</param>
		/// <param name="peso">Función que asigna a cada arista su peso</param>
		/// <returns>Devuelve la ruta de menor <c>Longitud</c>.</returns>
		/// <remarks>Puede ciclar si no existe ruta de x a y.</remarks> 
		/// <remarks>Devuelve ruta vacía (no nula) si origen es destino </remarks>
		/// <remarks>Devuelve null si toda ruta de x a y toca a ignorar</remarks>
		Ruta<T> CaminoÓptimo (T x,
		                      T y,
		                      Func<AristaPeso<T, TData>, float> peso,
		                      ISet<T> ignorar)
		{
			Ruta<T> ret = null;
			float longRet = 0;

			var arisXY = EncuentraArista (x, y);
			if (arisXY.Existe)
				return new Ruta<T> (x, y);

			var consideradNodos = new HashSet<T> (AntiVecino (y));
			consideradNodos.ExceptWith (ignorar);

			if (!consideradNodos.Any ())
				return null;
			foreach (var v in consideradNodos)
			{
				var ignorarRecursivo = new HashSet<T> (ignorar);
				ignorarRecursivo.Add (v);
				var mejorRuta = CaminoÓptimo (x, v, peso, ignorarRecursivo);
				if (mejorRuta != null)
				{
					var últAris = EncuentraArista (v, y);
					if (!últAris.Existe)
						throw new Exception ("???");
					mejorRuta.Concat (últAris, v);
					float longÚlt = 0;
					foreach (var p in mejorRuta.Pasos)
						longÚlt += peso (EncuentraArista (p.Origen, p.Destino));
					if (ret == null || longÚlt < longRet)
					{
						ret = mejorRuta;
						longRet = longÚlt;
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
		/// Construye un Grafo booleano modificable
		/// </summary>
		/// <param name="nodos">Una colección con los nodos del grafo</param>
		/// <param name="simétrico">If set to <c>true</c> es simétrico.</param>
		public Grafo (ICollection<T> nodos, bool simétrico = false)
			: base (nodos, simétrico, false)
		{
		}

		/// <summary>
		/// Construye un Grafo booleano
		/// </summary>
		/// <param name="nodos">Una colección con los nodos del grafo</param>
		/// <param name="simétrico">If set to <c>true</c> es simétrico.</param>
		/// <param name="sóloLectura">If set to <c>true</c> sólo lectura.</param>
		protected Grafo (ICollection<T> nodos, bool simétrico, bool sóloLectura)
			: base (nodos, simétrico, sóloLectura)
		{
		}

		/// <summary>
		/// Clona un Grafo booleano
		/// </summary>
		/// <param name="sóloLectura">If set to <c>true</c> sólo lectura.</param>
		/// <param name="graf">Grafo a clonar.</param>
		public Grafo (IGrafo<T> graf, bool sóloLectura = true)
			: base (graf.Nodos, false, sóloLectura)
		{
			for (int i = 0; i < NumNodos; i++)
				for (int j = 0; j < NumNodos; j++)
				{
					var ori = IntNodos [i];
					var des = IntNodos [j];
					Data [i, j] = new AristaBool<T> (
						ori,
						des,
						graf [ori, des].Existe,
						sóloLectura);
				}
		}

		/// <summary>
		/// Clona las aristas y las agrega a un grafo.
		/// </summary>
		/// <param name="sóloLectura">Si el grafo que devuelve es de sólo lectura</param>
		/// <returns>Un grafo clón</returns>
		/// <remarks>Las aristas son clonadas y por lo tanto no se preserva referencia </remarks>
		public Grafo<T> Clonar (bool sóloLectura = false)
		{
			var ret = new Grafo<T> (Nodos, EsSimétrico, sóloLectura);
			for (int i = 0; i < NumNodos; i++)
				for (int j = 0; j < (EsSimétrico ? i + 1 : NumNodos); j++)
				{
					var x = Data [i, j]; // La arista iterando
					ret.Data [i, j] = new AristaBool<T> (
						x.Origen,
						x.Destino,
						x.Existe,
						x.SóloLectura,
						x.EsSimétrico);
				}
			return ret;
		}

		/// <summary>
		/// Devuelve un grafo preservando referencias.
		/// </summary>
		/// <returns>Un grafo sólo lectura clonado</returns>
		public Grafo<T> ComoSóloLectura ()
		{
			var ret = new Grafo<T> (Nodos, EsSimétrico, true);
			for (int i = 0; i < NumNodos; i++)
				for (int j = 0; j < (EsSimétrico ? i + 1 : NumNodos); j++)
				{
					var x = Data [i, j]; // La arista iterando
					ret.Data [i, j] = new AristaBool<T> (
						x.Origen,
						x.Destino,
						x.Existe,
						x.SóloLectura,
						x.EsSimétrico);
				}
			return ret;
		}

		#endregion

		#region Común

		/// <summary>
		/// Construye una nueva arista para utilizarla al asignar las rutas de Data
		/// </summary>
		/// <returns>Devuelve una nueva AristaBool correspondiente</returns>
		/// <param name="origen">Nodo origen</param>
		/// <param name="destino">Nodo destino</param>
		protected override AristaBool<T> ConstruirNuevaArista (T origen, T destino)
		{
			return new AristaBool<T> (origen, destino, false, SóloLectura, EsSimétrico);
		}

		#endregion

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
		public Grafo<T> Subgrafo (ICollection<T> conjunto)
		{
			var ret = new Grafo<T> (conjunto, EsSimétrico, SóloLectura);

			if (conjunto.Any (x => !Nodos.Contains (x)))
				throw new ArgumentException (
					"Se requere que el conjunto de nodos esté contenido en los nodos del grafo original.",
					"conjunto");

			foreach (var x in new List<T> (conjunto))
				foreach (var y in new List<T> (conjunto))
					ret [x, y] = this [x, y];
			return ret;
		}

		IGrafo<T> IGrafo<T>.Subgrafo (IEnumerable<T> conjunto)
		{
			return Subgrafo (new HashSet<T> (conjunto));
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
			return new HashSet<IArista<T>> (Aristas ().Cast <IArista<T>> ());
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
				EncuentraArista (x, y).Existe = value;
			}
		}

		/// <summary>
		/// Devuelve a ruta de menor longitud entre dos puntos.
		/// </summary>
		/// <returns>The óptimo.</returns>
		/// <param name="x">Origen</param>
		/// <param name="y">Destino</param>
		public IRuta<T> CaminoÓptimo (T x, T y)
		{
			if (x.Equals (y))
				return null;
			var ign = new HashSet<T> ();
			ign.Add (x);
			ign.Add (y);
			return CaminoÓptimo (x, y, ign);
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
		/// <remarks>Devuelve null si toda ruta de x a y toca a ignorar</remarks>
		Ruta<T> CaminoÓptimo (T x,
		                      T y,
		                      ISet<T> ignorar)
		{
			Ruta<T> ret = null;

			var arisXY = EncuentraArista (x, y);
			if (arisXY.Existe)
				return new Ruta<T> (x, y);

			var consideradNodos = new HashSet<T> (AntiVecino (y));
			consideradNodos.ExceptWith (ignorar);

			if (!consideradNodos.Any ())
				return null;
			foreach (var v in consideradNodos)
			{
				var ignorarRecursivo = new HashSet<T> (ignorar);
				ignorarRecursivo.Add (v);
				var mejorRuta = CaminoÓptimo (x, v, ignorarRecursivo);
				if (mejorRuta != null)
				{
					var últAris = EncuentraArista (v, y);
					if (!últAris.Existe)
						throw new Exception ("???");
					mejorRuta.Concat (últAris, v);
					if (ret == null || mejorRuta.NumPasos < ret.NumPasos)
						ret = mejorRuta;
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