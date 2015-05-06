using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsFormsApplication1
{
	public static class Solver2
	{
		public static List<Solution> FromIndicies (Input input, List<int> indicies)
		{
			var b = new List<Solution> (indicies.Count ());
			var last = -1;
			foreach (var i in indicies) {
				var t = input.rs [i] > last 
					? input.rs [i] + input.ps [i] 
					: last + input.ps [i];
				last = t;
				b.Add (new Solution (i, t));
			}
			return b;
		}

		public static List<int> DistinctReverse (IEnumerable<int> list)
		{
			return list.Reverse ().Distinct ().Reverse ().ToList ();
		}

		public	static List<Solution>
		Solve (Input input)
		{
			var initial = Solver1.Solve (input);

			var solutions = Solver1.DistinctUntilChanged (initial);
			var indicies = solutions.Select (s => s.index).ToList ();
			return FromIndicies (input, DistinctReverse (indicies));
		}
	}
}

