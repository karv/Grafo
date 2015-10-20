using System;
using Graficas;
using System.Collections.Generic;

namespace GrafosTest
{
	class MainClass
	{
		public static void Main2(string[] args)
		{
			Grafo<int> g = new Grafo<int>();
			g.EsSimétrico = true;

			GrafoClan<int> gr = new GrafoClan<int>();
			gr.AgregaArista(0, 1);
			gr.AgregaArista(0, 2);
			gr.AgregaArista(1, 2);
			gr.AgregaArista(0, 4);
			if (gr.ExisteArista(new Arista<int>(1, 2, 1)))
				Console.WriteLine("Hi");
		}
	}
}
