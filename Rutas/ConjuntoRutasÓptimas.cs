using System;
using System.Collections.Generic;
using System.Diagnostics;
using Graficas.Grafo;
using Graficas.Aristas;
using ListasExtra;
using System.Linq;
using System.Threading.Tasks;

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

		bool IntentaAgregarArista (IArista<TNodo> aris, TNodo ori, TNodo des)
		{
			if (!aris.Existe)
				return false;
			
			bool ret = false;

			var peso = Peso (aris);
			if (!(CaminoÓptimo (ori, des)?.Longitud < peso))
			{
				var addRuta = new Ruta<TNodo> (ori, des, peso);
				reemplazaRuta (addRuta);
				ret = true;
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
			var comp = new Comparison<IArista<TNodo>> ((x, y) => Peso (x) < Peso (y) ? -1 : 1);

			rutas = new HashSet<IRuta<TNodo>> ();
			agregarDiagonal (gr.Nodos);

			var cmp = EqualityComparer<TNodo>.Default;

			foreach (var x in gr.Nodos)
			{
				foreach (var y in gr.Nodos)
				{
					var ar = gr [x, y];
					if (!ar.Existe)
						continue;

					IntentaAgregarArista (ar, x, y);

					// Ahora rellenar las rutas
					var clone = new HashSet<IRuta<TNodo>> (rutas);
					foreach (var z in clone)
					{
						// Tomar a los que tienen como destino a x.Origen y concatenarlos con x
						if (cmp.Equals (z.NodoFinal, x))
						{
							var path = new Ruta<TNodo> (z);
							var paso = new Paso<TNodo> (x, y, Peso (ar));
							path.Concat (paso);
							if (IntentaAgregarArista (path))
								Debug.WriteLine (string.Format ("Nueva mejor ruta: {0}", path));
						}
						// Tomar a los que tienen como origen a x.Destino y concatenarlos con x
						else if (cmp.Equals (z.NodoInicial, y))
						{
							var path = new Ruta<TNodo> (x, y, Peso (ar));
							path.Concat (z);
							if (IntentaAgregarArista (path))
								Debug.WriteLine (string.Format ("Nueva mejor ruta: {0}", path));
						}
					}
				}
			}
		}
	}
}