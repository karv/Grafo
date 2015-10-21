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

		public void Concat (T nodo, float peso)
		{
			throw new NotImplementedException ();
		}

		public T NodoInicial
		{
			get
			{
				throw new NotImplementedException ();
			}
		}

		T IRuta<T>.NodoFinal
		{
			get
			{
				throw new NotImplementedException ();
			}
		}

		public float Longitud
		{
			get
			{
				throw new NotImplementedException ();
			}
		}

		public int NumPasos
		{
			get
			{
				throw new NotImplementedException ();
			}
		}

		public System.Collections.Generic.IEnumerable<IPaso<T>> Pasos
		{
			get
			{
				throw new NotImplementedException ();
			}
		}
	}
}

