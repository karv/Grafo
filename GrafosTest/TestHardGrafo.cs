using System;
using Graficas.Grafo;
using Xunit;

namespace Test
{
	public class TestHardGrafo
	{
		[Fact]
		public void AgregarQutar ()
		{
			const int MaxSize = 100;
			const int MaxFacts = 20;
			var gr = new HardGrafo<int> ();

			for (int i = 1; i < MaxSize; i++)
				for (int j = i + 1; j < MaxSize; j++)
					gr [i, j] = j % i == 0;
			Console.WriteLine (gr);

			var r = new Random ();
			for (int i = 0; i < MaxFacts; i++)
			{
				int a = r.Next (MaxSize - 1) + 1;
				int b = r.Next (MaxSize - 1) + 1;
				var max = Math.Max (a, b);
				var min = Math.Min (a, b);

				Assert.Equal (max % min == 0, gr [a, b]);
			}
		}

		[Fact]
		public void TestAristasGet ()
		{
			const int MaxSize = 50;
			var gr = new HardGrafo<int> ();

			for (int i = 0; i < MaxSize; i++)
			{
				for (int j = i + 1; j < MaxSize; j++)
				{
					gr [i, j] = true;
				}
			}

			Assert.True (gr [1, 2]);

			gr [1, 2] = false;

			Assert.False (gr [1, 2]);

			gr [1, 2] = true;

			var aris = gr.Aristas ();
			foreach (var x in aris)
			{
				Console.Write (x + "\t");
			}
			Console.WriteLine ();
			Assert.Equal (MaxSize * (MaxSize - 1) / 2, aris.Count);

		}

	}
}