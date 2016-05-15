using System.Collections.Generic;
using ListasExtra.Extensiones;
using Graficas.Grafo;
using System;
using System.Linq;

namespace Graficas.Extensiones
{
	/// <summary>
	/// Métodos extendidos de grafos
	/// </summary>
	public static class Extensiones
	{
		/// <summary>
		/// Devuelve una colección con las componentes conexas de una gráfica dada
		/// </summary>
		/// <param name="gr">Gráfo</param>
		/// <typeparam name="T">Tipo de nodos de la gráfica</typeparam>
		public static ICollection<ILecturaGrafo<T>> ComponentesConexas<T> (this ILecturaGrafo<T> gr)
		{
			var nodosRestantes = new HashSet<T> (gr.Nodos);
			HashSet<T> Verdes;
			var ret = new List<ILecturaGrafo<T>> ();

			while (nodosRestantes.Count > 0)
			{
				T nodo = nodosRestantes.Aleatorio ();

				// Calcular la nube de nodo
				var nubeActual = new HashSet<T> ();
				nubeActual.Add (nodo);
				Verdes = new HashSet<T> (nubeActual);
				HashSet<T> nubeAgregando;
				do
				{
					nubeAgregando = new HashSet<T> ();
					foreach (var x in Verdes)
					{
						nubeAgregando.UnionWith (gr.Vecinos (x));
						nubeAgregando.ExceptWith (nubeActual);
						nubeAgregando.ExceptWith (Verdes);
					}
					nubeActual.UnionWith (Verdes);
					Verdes = new HashSet<T> (nubeAgregando);
				}
				while (Verdes.Count > 0);

				ret.Add (gr.Subgrafo (nubeActual));
				nodosRestantes.ExceptWith (nubeActual);
			}

			return ret;
		}
	}

	/// <summary>
	/// Extensión de objetos tipo diccionario.
	/// </summary>
	public static class DictExt
	{
		/// <summary>
		/// Devuelve el valor de una entrada en un diccionario, devulviendo su correspondiente en
		/// el diccionario si existe, en caso contrario devuelve el valor predeterminado de TVal.
		/// </summary>
		/// <param name="dict">Diccionario.</param>
		/// <param name="key">Key.</param>
		public static TVal GetValueOrDefault<TKey, TVal> (this IDictionary<TKey, TVal> dict,
		                                                  TKey key)
			where TKey : IEquatable<TKey>
		{
			TVal ret;
			return dict.TryGetValue (key, out ret) ? ret : default(TVal);
		}

		/// <summary>
		/// Devuelve el valor de una entrada en un diccionario, devulviendo su correspondiente en
		/// el diccionario si existe, en caso contrario devuelve un valor dado.
		/// </summary>
		/// <param name="dict">Diccionario.</param>
		/// <param name="key">Key.</param>
		/// <param name="def">Valor default</param>
		public static TVal GetValueOrDefault<TKey, TVal> (this IDictionary<TKey, TVal> dict,
		                                                  TKey key,
		                                                  TVal def)
		{
			TVal ret;
			return dict.TryGetValue (key, out ret) ? ret : def;
		}

		/// <summary>
		/// Devuelve el valor de una entrada en un diccionario, devulviendo su correspondiente en
		/// el diccionario si existe, en caso contrario devuelve un valor dado.
		/// </summary>
		/// <param name="dict">Diccionario.</param>
		/// <param name="key1">Primera entrada</param>
		/// <param name="key2">Segunda entrada</param>
		/// <param name="def">Valor default</param>
		public static TVal GetValueOrDefault<TKey, TVal> (this IDictionary<Tuple<TKey, TKey>, TVal> dict,
		                                                  TKey key1,
		                                                  TKey key2,
		                                                  TVal def)
		{
			return dict.GetValueOrDefault (new Tuple<TKey, TKey> (key1, key2), def);
		}

		/// <summary>
		/// Devuelve el valor de una entrada en un diccionario, devulviendo su correspondiente en
		/// el diccionario si existe, en caso contrario devuelve el valor predeterminado
		/// </summary>
		/// <param name="dict">Diccionario.</param>
		/// <param name="key1">Primera entrada</param>
		/// <param name="key2">Segunda entrada</param>
		public static TVal GetValueOrDefault<TKey, TVal> (this IDictionary<Tuple<TKey, TKey>, TVal> dict,
		                                                  TKey key1,
		                                                  TKey key2)
		{
			return dict.GetValueOrDefault (
				new Tuple<TKey, TKey> (key1, key2),
				default (TVal));
		}

		/// <summary>
		/// Establece el valor de una entrada
		/// </summary>
		public static void SetValue<TKey, TVal> (this IDictionary<Tuple<TKey, TKey>, TVal> dict,
		                                         TKey key1,
		                                         TKey key2,
		                                         TVal val)
		{
			var entrada = new Tuple<TKey, TKey> (key1, key2);
			if (dict.ContainsKey (entrada))
				dict [entrada] = val;
			else
				dict.Add (entrada, val);
		}

		/// <summary>
		/// Establece el valor de una entrada
		/// </summary>
		public static void SetValue<TKey, TVal> (this IDictionary<TKey, TVal> dict,
		                                         TKey key,
		                                         TVal val)
		{
			if (dict.ContainsKey (key))
				dict [key] = val;
			else
				dict.Add (key, val);
		}
	}
}