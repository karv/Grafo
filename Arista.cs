using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Graficas
{
	public class Arista<T> : IArista<T>
	{
		T _desde;
		T _hasta;

		public T desde
		{
			get
			{
				return _desde;
			}
		}

		public T hasta
		{
			get
			{
				return _hasta;
			}
		}

		public Arista(T nDesde, T nHasta)
		{
			_desde = nDesde;
			_hasta = nHasta;
		}
	}
}
