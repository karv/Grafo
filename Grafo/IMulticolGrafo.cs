using System.Collections.Generic;

namespace Graficas
{
	/// <summary>
	/// Representa una gráfica de muchos 'colores'
	/// </summary>
	public interface IMulticolGrafo<TNodo, TColor>: ILecturaGrafo<TNodo>
	{
		ICollection<TNodo> Vecinos(TNodo nodo, TColor color);

		void AgregaColor(TColor color, ILecturaGrafo<TNodo> grafo);

		ILecturaGrafo<TNodo> GrafoColor(TColor color);

		IEnumerable<TColor> ColoresArista(IArista<TNodo> aris);
	}
}

