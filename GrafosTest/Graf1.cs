using System;
using Graficas.Grafo;
using NUnit.Framework;
using Graficas.Aristas;

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
			if (other == null)
				return false;
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

		public override string ToString ()
		{
			return Hash.ToString ();
		}
	}

	/// <summary>
	/// Pruebas de Grafo[1]
	/// </summary>
	[TestFixture]
	public class Graf1
	{
		[Test]
		public void Clear ()
		{
			var Grafo = new Grafo<Objeto> ();
			Grafo [0, 1] = true;
			Grafo.Clear ();

			Assert.IsEmpty (Grafo.Aristas ());
			Assert.IsEmpty (Grafo.Nodos);
		}

		[Test]
		public void SóloLectura ()
		{
			var Grafo = new Grafo<Objeto> ();
			Grafo [0, 1] = true;
			var gr2 = new Grafo<Objeto> (Grafo);
			Assert.Throws<InvalidOperationException> (new TestDelegate (gr2.Clear));
			Assert.Throws<OperaciónAristaInválidaException> (new TestDelegate (delegate
			{
				gr2 [0, 1] = false;
			}));
			Assert.Throws<OperaciónAristaInválidaException> (new TestDelegate (delegate
			{
				gr2 [0, 2] = true;
			}));
		}

		[Test]
		public void CtorClonado ()
		{
			var Grafo = new Grafo<Objeto> ();
			Grafo [0, 1] = true;
			Grafo [0, 2] = true;
			Grafo [0, 3] = true;

			var clón = new Grafo<Objeto> (Grafo);
			Assert.AreEqual (true, clón [0, 1]);
			Assert.AreEqual (true, clón [0, 2]);
			Assert.AreEqual (true, clón [0, 3]);
			Assert.AreEqual (false, clón [1, 2]);
		}
	}
}