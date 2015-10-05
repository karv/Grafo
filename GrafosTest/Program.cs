using System;
using Graficas;
using System.Collections.Generic;

namespace GrafosTest
{
	class MainClass
	{
		public static void Main2(string[] args)
		{
			Grafica<int> g = new Grafica<int>();
			g.EsSimetrico = true;

			GraficaClan<int> gr = new GraficaClan<int>();
			gr.AgregaArista(0, 1);
			gr.AgregaArista(0, 2);
			gr.AgregaArista(1, 2);
			gr.AgregaArista(0, 4);
			if (gr.ExisteArista(new Arista<int>(1, 2, 1)))
				Console.WriteLine("Hi");
		}
	}
}
