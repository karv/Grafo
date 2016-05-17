using System;
using Graficas.Grafo;
using Graficas.Aristas;

namespace GrafosTest
{
	class MainClass
	{
		public static void Main2 (string [] args)
		{
			var g = new Grafo<int, float> ();
			g.EsSimétrico = true;

			GrafoClan<int> gr = new GrafoClan<int> ();
			gr.AgregaArista (0, 1);
			gr.AgregaArista (0, 2);
			gr.AgregaArista (1, 2);
			gr.AgregaArista (0, 4);
			if (gr.ExisteArista (new AristaPeso<int, float> (1, 2, 1)))
				Console.WriteLine ("Hi");
		}
	}
}
