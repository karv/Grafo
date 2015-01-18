using System;
using System.Collections.Generic;
using ListasExtra;

namespace Gráficas
{
    /// <summary>
    /// Representa una gráfica, en el sentido abstracto.
    /// Los nodos serán del tipo <c>T</c>.
    /// </summary>
    public class Gráfica<T>
    {
        public ListaPeso<Tuple<T, T>> Vecinos = new ListaPeso<Tuple<T, T>>();

        /// <summary>
        /// Devuelve la longitud de la ruta.
        /// </summary>
        public float Longitud (Ruta R)
        {
            float ret = 0f;
            for (int i = 0; i < R.Paso.Count - 1; i++)
			{
                ret += this[R.Paso[i], R.Paso[i + 1]];

			}
            return ret;
        }

        public bool EsSimétrico = false;

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
                if (!float.IsPositiveInfinity(this[x, y])) ret.Add(y);
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
                if (!float.IsPositiveInfinity(this[y, x])) ret.Add(y);
            }
            return ret;
        }

        /// <summary>
        /// Representa una ruta en un árbol.
        /// </summary>
        public class Ruta
        {
            public List<T> Paso;

            public static bool operator == (Ruta left, Ruta right)
            {
                if (left.Paso.Count!=right.Paso.Count) return false;

                for (int i = 0; i < left.Paso.Count; i++)
			    {
			        if (!left.Paso[i].Equals(right.Paso[i])) return false;
			    }
                return true;
            }

            public static bool operator !=(Ruta left, Ruta right)
            {
                return !(left == right);
            }

            public override bool Equals(object obj)
            {
                if (obj.GetType() is Gráfica<T>.Ruta)
                {
                    Gráfica<T>.Ruta Obj = (Gráfica<T>.Ruta)obj;
                    return this == Obj;
                }
                else return false;                    
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
        /// <param name="x">Un vértice.</param>
        /// <param name="y">Otro vértice.</param>
        /// <returns>Devuelve el peso de la arista que une estos nodos. <see cref="float.PositiveInfinity"/> si no existe arista.</returns>
        public float this[T x, T y]
        {
            get
            {
                return Vecinos[new Tuple<T,T>(x,y)];
            }
            set
            {
                Vecinos[new Tuple<T,T>(x,y)] = value;
                if (EsSimétrico) Vecinos[new Tuple<T,T>(y,x)] = value;
            }
        }

        /// <summary>
        /// Agrega un vértice entre dos nodos existentes a la gráfica.
        /// </summary>
        /// <param name="x">Un nodo.</param>
        /// <param name="y">Otro nodo.</param>
        /// <param name="Peso">El peso de la arista entre los nodos</param>
        public void AgregaVértice(T x, T y, float Peso)
        {
            {
                this[x,y] = Peso;
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
            T[] tmp = {};


            if (x.Equals(y)) {
                ret.Paso.Add(x);
                return ret;            
            }
            // else
            foreach (var n in AntiVecino(y))
	        {
		        if (!Ignorar.Contains(n))
                {
                    Ignorar.CopyTo(tmp);
                    Ignora2 = new List<T> (tmp);

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
        public Ruta CaminoÓptimo (T x, T y)
        {
            return CaminoÓptimo (x,y, new List<T>());
        }

        /// <summary>
        /// Genera una gráfica aleatoria.
        /// Por ahora no es aleatoria. Es completa, con peso aleatorio.
        /// </summary>
        /// <param name="Nodos">El conjunto de nodos que se usarán.</param>
        /// <param name="r">El generador aleatorio.</param>
        /// <returns></returns>
        public static Gráfica<T> GeneraGráficaAleatoria(T[] Nods, Random r)
        {
            Gráfica<T> ret = new Gráfica<T>();

            for (int i = 0; i < Nods.Length; i++)
            {
                for (int j = i + 1; j < Nods.Length; j++)
                {
                    ret.AgregaVértice(Nods[i], Nods[j], (float)r.NextDouble());
                }                
            }
            return ret;
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
        List<object> SeleccionaPeso (Random r, int n, ListasExtra.ListaPeso<object> Lista)
        {
            List<object> ret;
            float Suma = 0;
            float rn;
            if (n == 0) return new List<object>(); else {
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
