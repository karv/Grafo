using System;
using System.Collections.Generic;

namespace Graficas.Continuo
{
	/// <summary>
	/// Una ruta de ContinuoPuntos
	/// </summary>
	[Serializable]
	public class Ruta<T> : Graficas.Rutas.Ruta<T>
	{
		/// <summary>
		/// Devuelve el origen de la ruta
		/// </summary>
		/// <value>The nodo inicial.</value>
		public new Punto<T> NodoInicial { get; }

		/// <summary>
		/// Devuelve el destino de la ruta
		/// </summary>
		/// <value>The nodo final.</value>
		public new Punto<T> NodoFinal { get; private set; }

		/// <summary>
		/// Initializes a new instance of the class
		/// </summary>
		/// <param name="inicial">Punto de origen</param>
		public Ruta (Punto<T> inicial)
		{
			NodoInicial = inicial;
			NodoFinal = inicial;
		}

		/// <summary>
		/// Elimina el primer paso.
		/// </summary>
		public void EliminarPrimero ()
		{
			Paso.RemoveAt (0);
		}

		/// <summary>
		/// Concatena finalmente con un punto
		/// </summary>
		/// <param name="final">Final.</param>
		public void ConcatFinal (Punto<T> final)
		{
			NodoFinal = final;
		}

		/// <summary>
		/// Devuelve el peso total de la ruta
		/// </summary>
		/// <value>The longitud.</value>
		public new float Longitud
		{
			get
			{
				var lbase = 0f;
				foreach (var x in Paso)
					lbase += x.Peso;

				return lbase + NodoInicial.DistanciaAExtremo (base.NodoInicial) + NodoFinal.DistanciaAExtremo (base.NodoFinal);
			}
		}

		/// <summary>
		/// Devuelve el número de pasos en la ruta
		/// </summary>
		/// <value>The number pasos.</value>
		public new int NumPasos
		{
			get
			{
				return base.NumPasos + 2;
			}
		}

		/// <summary>
		/// Revisa si un punto dado pertenece a esta ruta.
		/// </summary>
		/// <param name="punto">Punto.</param>
		public bool Contiene (Punto<T> punto)
		{
			// Hay de tres:
			// 0) Está en el semiintervalo inicial
			// 1) Está en el semiintervalo final
			// 2) Está en un intervalo intermedio

			// 0)
			if (NodoInicial.EnMismoIntervalo (punto))
			{
				T MyA = punto.A;
				if (NodoInicial.DistanciaAExtremo (MyA) <= punto.Loc)
					return true;
			}

			// 2)
			foreach (var x in Pasos)
			{
				if (punto.EnIntervaloInmediato (x.Origen, x.Destino))
					return true;
			}

			// 1)
			if (NodoFinal.EnMismoIntervalo (punto))
			{
				T MyB = punto.B;
				if (NodoFinal.DistanciaAExtremo (MyB) < punto.Aloc)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Devuelve una enumeración de los puntos contenidos en esta ruta
		/// </summary>
		public List<Punto<T>> PuntosEnRuta (Continuo<T> gr)
		{
			return gr.Puntos.FindAll (Contiene);
		}
	}
		
}
