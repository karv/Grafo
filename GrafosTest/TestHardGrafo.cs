using NUnit.Framework;
using Graficas;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Test
{
	[TestFixture]
	public class TestHardGrafo
	{
		[Test]
		public void AgregarQutar()
		{
			const int MaxSize = 100;
			const int MaxTests = 20;
			var gr = new HardGrafo<int>();

			for (int i = 1; i < MaxSize; i++)
			{
				for (int j = i + 1; j < MaxSize; j++)
				{
					if (j % i == 0)
						gr[i, j] = true;
				}
			}

			Console.WriteLine(gr);

			var r = new Random();
			for (int i = 0; i < MaxTests; i++)
			{
				int a = r.Next(MaxSize);
				int b = r.Next(MaxSize);
				Assert.AreEqual(b % a == 0, gr[a, b]);
			}
		}

		[Test]
		public void TestAristasGet()
		{
			const int MaxSize = 10;
			var gr = new HardGrafo<int>();

			for (int i = 0; i < MaxSize; i++)
			{
				for (int j = i + 1; j < MaxSize; j++)
				{
					gr[i, j] = true;
				}
			}

			Assert.True(gr[1, 2]);

			gr[1, 2] = false;

			Assert.False(gr[1, 2]);

			var aris = gr.Aristas();
			Assert.AreEqual(MaxSize * (MaxSize - 1) / 2, aris.Count);
			Console.WriteLine(aris);

		}
	}
}