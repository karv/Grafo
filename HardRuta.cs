using System;
using Graficas.Rutas;
using System.Collections.Generic;
using System.Threading;
using System.Net.Mail;

namespace Graficas
{
	public class HardRuta<T> : IRuta<T>
	{
		List<Nodo<T>> _pasos { get; }

		public HardRuta()
		{
			_pasos = new List<Nodo<T>>();
		}

		public HardRuta(IEnumerable<Nodo<T>> pasos)
		{
			_pasos = new List<Nodo<T>>(pasos);
		}

		public IRuta<T> Reversa()
		{
			throw new NotImplementedException();
		}

		public void Concat(IPaso<T> paso)
		{
			throw new NotImplementedException();
		}

		public void Concat(IRuta<T> ruta)
		{
			throw new NotImplementedException();
		}

		public void Concat(T nodo, float peso)
		{
			throw new NotImplementedException();
		}

		public T NodoInicial
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public T NodoFinal
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public float Longitud
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int NumPasos
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public System.Collections.Generic.IEnumerable<IPaso<T>> Pasos
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}

