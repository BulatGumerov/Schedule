using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsFormsApplication1
{
	public static class Solver3
	{
		public static List<int> FirstSeq (List<int> ds, List<int> indexes)
		{
			var jd = Solver1.CalcJd (ds, indexes);
			return indexes.Where (i => i <= jd).OrderBy (i => i).ToList ();
		}

		public static List<List<int>> Combinations (List<int> ds)
		{
			var rb = new RingBuffer<int> (0, ds.Take (ds.Count () - 1).ToList ());
			var b = new List<List<int>> ();
			for (int i = 0; i < rb.Count; i++) {
				rb = rb.ShiftLeft ();
				var what = rb.ToList ();
				what.Add (rb.Last ());
				what [rb.Count - 1] = ds.Last ();
				b.Add (what);
			}		
			return b;
		}

		public static List<int> Mins (List<int> x)
		{
			var min = x.Min ();
			return x.Zip (Enumerable.Range (0, x.Count ()), (v, i) => Tuple.Create (v, i))
				.Where (t => t.Item1 == min)
				.Select (t => t.Item2)
				.ToList ();
		}

		public static List<int> Strip (List<int> indicies)
		{
			return indicies.Take (indicies.IndexOf (indicies.Max ()) + 1).ToList ();
		}

		public static List<int> Rest (List<int > all, List<int> stripped)
		{
			return all.Except (stripped).ToList ();
		}

		public static int F (Input input, List<int > s)
		{
			return Solver1.F (input.ds, Solver2.FromIndicies (input, s));
		}

		public static List<List<int>> SolveAll (Input input)
		{
			var q = new Queue<Tuple<List<int>,List<int>>> (1);
			q.Enqueue (Tuple.Create (new List<int> (), Enumerable.Range (0, input.ds.Count ()).ToList ()));
			var lastRoundB = new List<List<int>> ();
			while (q.Any ()) {
				var tuple = q.Dequeue ();
				var intermediate = tuple.Item1;
				var indicies = tuple.Item2;
				if (indicies.Any ()) {
					var ints = FirstSeq (input.ds, indicies);
					var combs = Combinations (ints);
					if (combs.Any ()) {
						var fs = combs.Select (s => F (input, s)).ToList ();
						var minIndexes = Mins (fs);
						var stripped = minIndexes.Select (i => Strip (combs [i])).ToList ();
						var rests = stripped.Select (s => Rest (indicies, s));
						foreach (var pair in stripped.Zip(rests, (s,r) => Tuple.Create(s,r))) {
							q.Enqueue (Tuple.Create (intermediate.Concat (pair.Item1).ToList (), pair.Item2));
						}
					} else {
						q.Enqueue (Tuple.Create (intermediate.Concat (ints).ToList (), Rest (indicies, ints)));
					}
				} else {
					lastRoundB.Add (intermediate);
				}
			}
			return Mins (lastRoundB.Select (x => F (input, x)).ToList ()).Select (m => lastRoundB [m]).ToList ();
		}

		public static List<Solution> Solve (Input input)
		{
			var indicies = SolveAll (input).First ();
			return Solver2.FromIndicies (input, Solver2.DistinctReverse (indicies));
		}
	}
}

