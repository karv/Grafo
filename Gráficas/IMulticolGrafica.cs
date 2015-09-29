using System.Collections.Generic;

namespace Graficas
{
	/// <summary>
	/// Representa una gráfica de muchos 'colores'
	/// </summary>
	public interface IMulticolGrafica<TNodo, TColor>: IGrafica<TNodo>
	{

		ICollection<TNodo> Vecinos(TNodo nodo, TColor color);

		void AgregaColor(TColor color);

		IGrafica<TNodo> GraficaColor(TColor color);

		IEnumerable<TColor> ColoresArista(IArista<TNodo> aris);
	}
}

