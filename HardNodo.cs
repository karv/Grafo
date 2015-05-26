using System;
using Graficas;
using System.Linq;

namespace Graficas.Nodos
{
	public class HardNodo<T> : IHardNodo<T>
	{
		T _obj;
		System.Collections.Generic.List<IHardNodo<T>> _succ = new System.Collections.Generic.List<IHardNodo<T>>();

		public HardNodo(T obj)
		{
			_obj = obj;
		}

		#region IHardNodo implementation

		System.Collections.Generic.ICollection<IHardNodo<T>> IHardNodo<T>.getSucc
		{
			get
			{
				return _succ;
			}
		}

		#endregion

		#region INodo implementation

		T INodo<T>.getObjeto
		{
			get
			{
				return _obj;
			}
		}

		System.Collections.Generic.IEnumerable<T> INodo<T>.getSucc
		{
			get
			{
				return _succ.ConvertAll<T>(x => x.getObjeto);
			}
		}

		#endregion
	}
}

