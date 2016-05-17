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
		ListaPeso<TNodo, TNodo, IRuta<TNodo>> RutasDict { get; }

		/// <summary>
		/// Devuelve el camino óptimo entre dos puntos.
		/// Si no existe algún camino, es null
		/// </summary>
		/// <param name="x">Origen</param>
		/// <param name="y">Destino</param>
		public IRuta<TNodo> CaminoÓptimo (TNodo x, TNodo y)
		{
			return RutasDict [x, y];
		}

		static float Peso (IArista<TNodo> aris)
		{
			return 0;
		}

		bool IntentaAgregarArista (IArista<TNodo> aris)
		{
			var origen = aris.Origen;
			var destino = aris.Destino;
			var peso = Peso (aris);
			if (!(RutasDict [origen, destino]?.Longitud (Peso) < peso))
			{
				var addRuta = new Ruta<TNodo> ();
				addRuta.Concat (aris);
				RutasDict [origen, destino] = addRuta;
				return true;
			}
			return false;
		}

		bool IntentaAgregarArista (IRuta<TNodo> ruta)
		{
			if (!(RutasDict [ruta.NodoInicial, ruta.NodoFinal]?.Longitud (Peso) < ruta.Longitud (Peso)))
			{
				RutasDict [ruta.NodoInicial, ruta.NodoFinal] = ruta;
				return true;
			}
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		protected ConjuntoRutasÓptimas ()
		{
			RutasDict = new ListaPeso<TNodo, TNodo, IRuta<TNodo>> (null, null);
		}

		/// <param name="gr">Gráfica asociada</param>
		public ConjuntoRutasÓptimas (IGrafo<TNodo> gr)
			: this ()
		{
			var aris = new List<IArista<TNodo>> (gr.Aristas ());
			var comp = new Comparison<IArista<TNodo>> ((x, y) => Peso (x) < Peso (y) ? -1 : 1);

			aris.Sort (comp);
			Debug.WriteLine (aris);

			foreach (var x in aris)
			{
				IntentaAgregarArista (x);

				// Ahora rellenar las rutas
				var clone = new Dictionary<Tuple<TNodo, TNodo>, IRuta<TNodo>> (RutasDict);
				foreach (var y in clone)
				{
					// Tomar a los que tienen como destino a x.Origen y concatenarlos con y	
					if (y.Key.Item2.Equals (x.Origen))
					{
						var path = new Ruta<TNodo> (y.Value);
						path.Concat (new Ruta<TNodo> (x));
						if (IntentaAgregarArista (path))
						{
							Console.WriteLine ();
						}
					}
					// Tomar a los que tienen como origen a x.Destino y concatenarlos con y
					else if (y.Key.Item1.Equals (x.Destino))
					{
						var path = new Ruta<TNodo> (x);
						path.Concat (y.Value);
						if (IntentaAgregarArista (path))
						{
							Console.WriteLine ();
						}
					}
				}
			}
		}
	}
}