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

		bool IntentaAgregarArista (TNodo origen, TNodo destino, float peso)
		{
			if (!(RutasDict [origen, destino]?.Longitud < peso))
			{
				var addRuta = new Ruta<TNodo> ();
				addRuta.Concat (origen, 0);
				addRuta.Concat (destino, peso);
				RutasDict [origen, destino] = addRuta;
				return true;
			}
			return false;
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

		/// <summary>
		/// 
		/// </summary>
		protected ConjuntoRutasÓptimas ()
		{
			RutasDict = new ListaPeso<TNodo, TNodo, IRuta<TNodo>> (null, null);
		}

		/// <param name="gr">Gráfica asociada</param>
		public ConjuntoRutasÓptimas (ILecturaGrafo<TNodo> gr)
			: this ()
		{
			var aris = new List<IArista<TNodo>> (gr.Aristas ());
			var comp = new Comparison<IArista<TNodo>> ((x, y) => x.Peso < y.Peso ? -1 : 1);

			aris.Sort (comp);
			Debug.WriteLine (aris);

			foreach (var x in gr.Nodos)
				IntentaAgregarArista (x, x, 0);

			foreach (var x in aris)
			{
				IntentaAgregarArista (x.Origen, x.Destino, x.Peso);

				// Ahora rellenar las rutas
				var clone = new Dictionary<Tuple<TNodo, TNodo>, IRuta<TNodo>> (RutasDict);
				foreach (var y in clone)
				{
					// Tomar a los que tienen como destino a x.Origen y concatenarlos con y	
					if (y.Key.Item2.Equals (x.Origen)) // ¿Por qué no entra incluso si la igualdad es true?
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