using NUnit.Framework;
using CE.Graph.Rutas;
using CE.Graph.Edges;

namespace Test
{
	[TestFixture]
	public class Rutas
	{
		[Test]
		public void ComparaRuta ()
		{
			const int lon = 100;

			var r1 = new Path<int> ();
			var r2 = new Path<int> ();
			var r3 = new Path<int> ();
			for (int i = 0; i < lon; i++)
			{
				r1.Concat (new Step<int> (i, i + 1, i));
				r2.Concat (new Step<int> (i, i + 1, i));
				r3.Concat (new Step<int> (i, i == lon - 1 ? lon + 1 : i + 1, i));
			}

			var compa = new StepPathComparer<int> ();
			Assert.True (compa.Equals (r1, r2));
			Assert.False (compa.Equals (r1, r3));
			Assert.False (compa.Equals (r2, r3));
		}

		[Test]
		public void TestRutaNula ()
		{
			var rn = Path<int>.Empty;
			rn.Concat (new Step<int> (0, 1));
			Assert.True (rn.StepCount == 1);
			Assert.AreEqual (rn.StartNode, 0);
			Assert.AreEqual (rn.EndNode, 1);

			var rn2 = Path<int>.Empty;
			rn2.Concat (rn);
			Assert.True (rn2.StepCount == 1);

			rn2.Concat (Path<int>.Empty);
			Assert.True (rn2.StepCount == 1);

			Assert.True (Path<int>.NullOrEmpty (null));
			Assert.True (Path<int>.NullOrEmpty (Path<int>.Empty));
			Assert.False (Path<int>.NullOrEmpty (rn2));
		}
	}
}