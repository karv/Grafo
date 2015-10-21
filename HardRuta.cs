using System;
using Graficas.Rutas;
using System.Collections.Generic;

namespace Graficas
{
	public class HardRuta<T> : IRuta<T> //TEST todo
	{
		List<Nodo<T>> _pasos { get; set; }

		public HardRuta ()
		{
			_pasos = new List<Nodo<T>> ();
		}

		public HardRuta (IEnumerable<Nodo<T>> pasos)
		{
			_pasos = new List<Nodo<T>> (pasos);
		}

		public IRuta<T> Reversa ()
		{
			var ret = new HardRuta<T> ();
			ret._pasos = new List<Nodo<T>> (_pasos);
			ret._pasos.Reverse ();
			return ret;
		}

		public Nodo<T> NodoFinal
		{
			get
			{
				return _pasos [_pasos.Count - 1];
			}
		}

		public void Concat (IPaso<T> paso)
		{
			if (NodoFinal.Equals (paso.Origen))
			{
				var agrega = NodoFinal.Vecindad.Find (x => x.Objeto.Equals (paso.Destino));
				if (agrega == null)
					throw new Exception ("Paso inexsistente en grafo.");
				_pasos.Add (agrega);
			}
			throw new Exception ("Nodo final de la ruta no concide con origen del paso.");
		}

		public void Concat (IRuta<T> ruta)
		{
			if (!NodoFinal.Objeto.Equals (ruta.NodoFinal))
				throw new Exception ("Nodo final de la ruta no concide con origen del paso.");
			foreach (var x in ruta.Pasos)
			{
				Concat (x);
			}
		}

		public void Concat (T nodo)
		{
			
			var agrega = NodoFinal.Vecindad.Find (x => x.Objeto.Equals (nodo));
			if (agrega == null)
				throw new Exception ("Paso inexsistente en grafo.");
			_pasos.Add (agrega);
		}

		void IRuta<T>.Concat (T nodo, float peso)
		{
			Concat (nodo);
		}

		public Nodo<T> NodoInicial
		{
			get
			{
				return _pasos [0];
			}
		}

		T IRuta<T>.NodoInicial
		{
			get
			{
				return NodoInicial.Objeto;
			}
		}


		T IRuta<T>.NodoFinal
		{
			get
			{
				return NodoFinal.Objeto;
			}
		}

		public float Longitud
		{
			get
			{
				return NumPasos;
			}
		}

		public int NumPasos
		{
			get
			{
				return _pasos.Count - 1;
			}
		}

		public IEnumerable<IPaso<T>> Pasos
		{
			get
			{
				for (int i = 0; i < NumPasos; i++)
				{
					yield return new Paso<T> (_pasos [i].Objeto, _pasos [i + 1].Objeto, 1);
				}
			}
		}
	}
}

