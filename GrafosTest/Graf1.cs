using System;
using Graficas.Grafo;

namespace Test
{
	public class Objeto : IEquatable<Objeto>
	{
		public readonly int Hash;

		public Objeto (int hash)
		{
			Hash = hash;
		}

		public override bool Equals (object obj)
		{
			if (obj == null)
				return false;
			if (ReferenceEquals (this, obj))
				return true;
			if (obj.GetType () != typeof (Objeto))
				return false;
			var other = (Objeto)obj; 
			return Equals (other);
		}

		public bool Equals (Objeto other)
		{
			return Hash == other.Hash;
		}

		public override int GetHashCode ()
		{
			unchecked
			{
				return Hash;
			}
		}

		public static implicit operator int (Objeto obj)
		{
			return obj.Hash;
		}

		public static implicit operator Objeto (int i)
		{
			return new Objeto (i);
		}
	}

	/// <summary>
	/// Pruebas de Grafo[1]
	/// </summary>
	public class Graf1 : IDisposable
	{
		public Grafo<Objeto> Grafo;

		public Graf1 ()
		{
			Grafo = new Grafo<Objeto> ();
		}

		public void Dispose ()
		{
			Grafo = new Grafo<Objeto> ();
		}

		[Test]
		public void Clear ()
		{
			Grafo [0, 1] = true;
			Grafo.Clear ();

			//Assert.Empty (Grafo.Aristas ());
			//Assert.Empty (Grafo.Nodos);
		}
	}
}