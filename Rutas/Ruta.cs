﻿using System;
using System.Collections.Generic;

namespace Graficas.Rutas
{
	public class Ruta<T>: IRuta<T>
	{
		struct NodoPeso
		{
			public T Nodo;
			public float Peso;

			public NodoPeso(T nodo, float peso)
			{
				Nodo = nodo;
				Peso = peso;
			}
		}

		readonly IList<NodoPeso> _paso = new List<NodoPeso>();

		public override string ToString()
		{
			string ret = string.Format("[{0}]: ", NumPasos);
			foreach (var x in _paso)
			{
				ret += string.Format(" {0} ", x.Nodo);

			}
			return ret;
		}

		public IEnumerable<IPaso<T>> Pasos
		{ 
			get
			{ 
				var ret = new List<IPaso<T>>();
				for (int i = 0; i < _paso.Count - 1; i++)
				{
					ret.Add(new Paso<T>(_paso[i].Nodo, _paso[i + 1].Nodo, _paso[i + 1].Peso));
				}
				return ret;
			} 
		}

		#region IMultiRuta implementation

		public void Concat(IPaso<T> paso)
		{
			if (paso == null)
				throw new NullReferenceException();
			if (NumPasos == 0)
			{
				_paso.Add(new NodoPeso(paso.Origen, 0));
				_paso.Add(new NodoPeso(paso.Destino, paso.Peso));
			}
			else
			{
				if (NodoFinal.Equals(paso.Origen))
				{
					_paso.Add(new NodoPeso(paso.Destino, paso.Peso));
				}
				else
				{
					throw new Exception("El nodo final debe coincidir con el origen de el paso para poder concatenar.");
				}
			}
		}

		public void Concat(IRuta<T> ruta)
		{
			if (!NodoFinal.Equals(ruta.NodoInicial))
				throw new Exception("No se puede concatenar si no coinciden los extremos finales e iniciales de los nodos.");

			foreach (var paso in ruta.Pasos)
			{
				_paso.Add(new NodoPeso(paso.Destino, paso.Peso));
			}
		}

		public void Concat(T nodo, float peso)
		{
			_paso.Add(new NodoPeso(nodo, peso));
		}

		public IRuta<T> Reversa()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Devuelve el origen de la ruta
		/// </summary>
		/// <value>The nodo inicial.</value>
		public T NodoInicial
		{
			get
			{
				return _paso[0].Nodo;
			}
		}

		/// <summary>
		/// Devuelve el destino de la ruta
		/// </summary>
		/// <value>The nodo final.</value>
		public T NodoFinal
		{
			get
			{
				if (NumPasos < 0)
					throw new Exception("No existe el nodo final en un path vacío.");
				return _paso[NumPasos].Nodo;
			}
		}

		/// <summary>
		/// Devuelve el peso total de la ruta
		/// </summary>
		/// <value>The longitud.</value>
		public float Longitud
		{
			get
			{
				float ret = 0;
				foreach (var x in _paso)
				{
					ret += x.Peso;
				}
				return ret;
			}
		}

		/// <summary>
		/// Devuelve el número de pasos en la ruta
		/// </summary>
		/// <value>The number pasos.</value>
		public int NumPasos
		{
			get
			{
				return _paso.Count - 1;
			}
		}

		#endregion
	}
}
