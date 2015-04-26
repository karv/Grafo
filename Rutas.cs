using System;
using System.Collections.Generic;

namespace Graficas.Rutas
{
	/// <summary>
	/// Da herramientas para trabajar con rutas en IGraficas
	/// </summary>
	public static class ExtensionRutas // where T:IDisposable
	{
		public static Ruta<T> MejorRuta<T>(this IGrafica<T> graf, T origen, T destino)
		{
			throw new NotImplementedException();
		}
	}

	public class Ruta<T>
	{
		public List<T> Paso;

		public static bool operator ==(Ruta<T> left, Ruta<T> right)
		{
			if (left.Paso.Count != right.Paso.Count)
				return false;

			for (int i = 0; i < left.Paso.Count; i++)
			{
				if (!left.Paso[i].Equals(right.Paso[i]))
					return false;
			}
			return true;
		}

		public static bool operator !=(Ruta<T> left, Ruta<T> right)
		{
			return !(left == right);
		}

		public override bool Equals(object obj)
		{
			if (obj is Ruta<T>)
				return ((Ruta<T>)obj) == this;
			else
				return false;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
