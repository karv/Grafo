using System;
using System.Collections.Generic;
using System.Diagnostics;
using Graficas.Grafo;
using Graficas.Aristas;
using ListasExtra;

namespace Graficas.Rutas
{
	/// <summary>
	/// Representa un conjunto de mejores rutas en una gráfica
	/// </summary>
	[Serializable]
	public class ConjuntoRutasÓptimas<TNodo>
		where TNodo : IEquatable<TNodo>
	{
		ListaPeso<TNodo, TNodo, IRuta<TNodo>> RutasDict;

		/// <summary>
		/// Devuelve el camino óptimo entre dos puntos.
		/// Si no existe algún camino, es null
		/// </summary>
		/// <param name="x">Origen</param>
		/// <param name="y">Destino</param>
		public IRuta<TNodo> CaminoÓptimo (TNodo x, TNodo y)
		{
			if (RutasDict == null)
				throw new Exception (@"No se inicializó esta clase
Ejecute Calcular () antes de llamar esta función");
			return RutasDict [x, y];
		}

		static float Peso (IArista<TNodo> aris)
		{
			var ar = aris as AristaPeso<TNodo, float>;
			return ar == null ? 1 : ar.Data;
		}

		bool IntentaAgregarArista (IArista<TNodo> aris)
		{
			bool ret = false;
			var par = aris.ComoPar ();
			var p0 = par [0];
			var p1 = par [1];
			var peso = Peso (aris);
			if (aris.Coincide (p0, p1))
			{
				if (!(RutasDict [p0, p1]?.Longitud < peso))
				{
					var addRuta = new Ruta<TNodo> (p0, p1, peso);
					RutasDict [p0, p1] = addRuta;
					ret = true;
				}
			}
			if (aris.Coincide (p1, p0))
			{
				if (!(RutasDict [p1, p0]?.Longitud < peso))
				{
					var addRuta = new Ruta<TNodo> (p1, p0, peso);
					RutasDict [p1, p0] = addRuta;
					ret = true;
				}
			}
			return ret;
		}

		bool IntentaAgregarArista (IRuta<TNodo> ruta)
		{
			if (!(RutasDict [ruta.NodoInicial, ruta.NodoFinal]?.Longitud < ruta.Longitud))
			{
				RutasDict [ruta.NodoInicial, ruta.NodoFinal] = ruta;
				return true;
			}
			return false;
		}

		void agregarDiagonal (ICollection<TNodo> coll)
		{
			foreach (var x in coll)
			{
				var rutaEstática = new Ruta<TNodo> (x, x, 0);
				RutasDict [x, x] = rutaEstática;
			}
		}

		/// <summary>
		/// Calcula las rutas óptimas
		/// </summary>
		/// <param name="gr">Gráfica asociada</param>
		public void Calcular (IGrafo<TNodo> gr)
		{
			var aris = new List<IArista<TNodo>> (gr.Aristas ());
			var comp = new Comparison<IArista<TNodo>> ((x, y) => Peso (x) < Peso (y) ? -1 : 1);

			RutasDict = new ListaPeso<TNodo, TNodo, IRuta<TNodo>> (null, null);
			agregarDiagonal (gr.Nodos);

			aris.Sort (comp);
			Debug.WriteLine (aris);

			foreach (var y in aris)
			{
				if (y == null)
					throw new InvalidOperationException ("Grafo tiene arista nula.");
				var x = y as IAristaDirigida<TNodo>;
				if (x == null)
					throw new NotSupportedException ("Para calcular rutas, el grafo debe ser de aristas dirigidas.");
				
				IntentaAgregarArista (x);

				// Ahora rellenar las rutas
				var clone = new Dictionary<Tuple<TNodo, TNodo>, IRuta<TNodo>> (RutasDict);
				foreach (var z in clone)
				{
					// Tomar a los que tienen como destino a x.Origen y concatenarlos con x
					if (z.Key.Item2.Equals (x.Origen))
					{
						var path = new Ruta<TNodo> (z.Value);
						var paso = new Paso<TNodo> (x);
						path.Concat (paso);
						if (IntentaAgregarArista (path))
							Debug.WriteLine ("");
					}
					// Tomar a los que tienen como origen a x.Destino y concatenarlos con x
					else if (z.Key.Item1.Equals (x.Destino))
					{
						var path = new Ruta<TNodo> (x.Origen, x.Destino, Peso (x));
						path.Concat (z.Value);
						if (IntentaAgregarArista (path))
							Debug.WriteLine ("");
					}
				}
			}
		}
	}
}