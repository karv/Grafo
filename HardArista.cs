namespace Graficas
{
	public class HardArista<T>:IArista<T>
	{
		public Nodo<T> Origen { get; }

		public Nodo<T> Destino { get; }

		T IArista<T>.Origen
		{
			get
			{
				return Origen.Objeto;
			}
		}

		T IArista<T>.Destino
		{
			get
			{
				return Destino.Objeto;
			}
		}

		public float Peso
		{
			get
			{
				return Origen.Vecindad.Contains(Destino) ? 1 : 0;
			}
		}

		public override string ToString()
		{
			return string.Format("[HardArista: Origen={0}, Destino={1}, Peso={2}]", Origen.Objeto, Destino.Objeto, Peso);
		}

		public HardArista(Nodo<T> desde, Nodo<T> hasta)
		{
			Origen = desde;
			Destino = hasta;
		}
	}
}

