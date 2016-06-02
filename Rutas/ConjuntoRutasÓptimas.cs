using System;
using System.Collections.Generic;
using System.Diagnostics;
using Graficas.Grafo;
using Graficas.Aristas;
using ListasExtra;
using System.Linq;

namespace Graficas.Rutas
{
	/// <summary>
	/// Representa un conjunto de mejores rutas en una gráfica
	/// </summary>
	[Serializable]
	public class ConjuntoRutasÓptimas<TNodo>
		where TNodo : IEquatable<TNodo>
	{
		HashSet<IRuta<TNodo>> rutas;

		/// <summary>
		/// Devuelve el camino óptimo entre dos puntos.
		/// Si no existe algún camino, es null
		/// </summary>
		/// <param name="x">Origen</param>
		/// <param name="y">Destino</param>
		public IRuta<TNodo> CaminoÓptimo (TNodo x, TNodo y)
		{
			if (rutas == null)
				throw new Exception (@"No se inicializó esta clase
Ejecute Calcular () antes de llamar esta función");

			var cmp = EqualityComparer<TNodo>.Default;
			var ret = rutas.FirstOrDefault (r => cmp.Equals (r.NodoInicial, x) && cmp.Equals (
				          r.NodoFinal,
				          y));
			return ret;
		}

		void reemplazaRuta (IRuta<TNodo> reemplazando)
		{
			var eliminar = CaminoÓptimo (
				               reemplazando.NodoInicial,
				               reemplazando.NodoFinal);
			if (eliminar != null)
				rutas.Remove (eliminar);
			rutas.Add (reemplazando);
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
				if (!(CaminoÓptimo (p0, p1)?.Longitud < peso))
				{
					var addRuta = new Ruta<TNodo> (p0, p1, peso);
					reemplazaRuta (addRuta);
					ret = true;
				}
			}
			if (aris.Coincide (p1, p0))
			{
				if (!(CaminoÓptimo (p1, p0)?.Longitud < peso))
				{
					var addRuta = new Ruta<TNodo> (p1, p0, peso);
					reemplazaRuta (addRuta);
					ret = true;
				}
			}
			return ret;
		}

		bool IntentaAgregarArista (IRuta<TNodo> ruta)
		{
			var rta = CaminoÓptimo (ruta.NodoInicial, ruta.NodoFinal);

			if (rta == null || rta.Longitud > ruta.Longitud)
			{
				reemplazaRuta (ruta);
				return true;
			}
			return false;
		}

		void agregarDiagonal (ICollection<TNodo> coll)
		{
			foreach (var x in coll)
			{
				var rutaEstática = new Ruta<TNodo> (x, x, 0);
				rutas.Add (rutaEstática);
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

			rutas = new HashSet<IRuta<TNodo>> ();
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
				var clone = new HashSet<IRuta<TNodo>> (rutas);
				var cmp = EqualityComparer<TNodo>.Default;
				foreach (var z in clone)
				{
					// Tomar a los que tienen como destino a x.Origen y concatenarlos con x
					if (cmp.Equals (z.NodoFinal, x.Origen))
					{
						var path = new Ruta<TNodo> (z);
						var paso = new Paso<TNodo> (x);
						path.Concat (paso);
						if (IntentaAgregarArista (path))
							Debug.WriteLine (string.Format ("Nueva mejor ruta: {0}", path));
					}
					// Tomar a los que tienen como origen a x.Destino y concatenarlos con x
					else if (cmp.Equals (z.NodoInicial, x.Destino))
					{
						var path = new Ruta<TNodo> (x.Origen, x.Destino, Peso (x));
						path.Concat (z);
						if (IntentaAgregarArista (path))
							Debug.WriteLine (string.Format ("Nueva mejor ruta: {0}", path));
					}
				}
			}
		}
	}
}