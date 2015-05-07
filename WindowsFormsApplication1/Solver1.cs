using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsFormsApplication1
{
	public static class Solver1
	{
		public static IList<int> RVals (List<int> rs, List<int> nk)
		{
			return nk.Select (i => rs [i]).ToList ();
		}

		public static int CalcJd (List<int> ds, List<int> nSub)
		{
			var from = 0;
			var i = 0;
			var min = nSub.Min (idx => ds [idx]);
			do {
				i = ds.IndexOf (min, from + 1);
				from = i;
			} while (!nSub.Contains (i));
			return i;
		}

		public static List<Solution> DistinctUntilChanged (IEnumerable<Solution> sols)
		{
			if (sols.Any ()) {
				var last = sols.First ();
				var list = sols.Skip (1).Where (x => {
					var b = x.index != last.index;
					last = x;
					return b;
				}).ToList ();
				list.Insert (0, sols.First ());
				return list;
			} else {
				return sols.ToList ();
			}
		}

		public static List<Solution> Solve (Input input)
		{
			var rs = input.rs;
			var ps = input.ps.ToList ();
			var ds = input.ds;
			var all = Enumerable.Range (0, rs.Count ()).ToList ();
			var tk = Math.Max (0, rs.Min ());
			var solutions = new List<Solution> ();
			while (all.Any ()) {
				var nSub = all.Zip (RVals(rs,all), (x, y) => Tuple.Create (x, y))
					.Where(tup => tup.Item2 <= tk)
					.Select(tup => tup.Item1)
					.ToList();
				if (nSub.Any ()) {
					var vals = RVals (rs, all.Except (nSub).ToList ());
					var jd = CalcJd (ds, nSub);
					var first = tk + ps [jd];
					var second = vals.Any () ? vals.Min () : int.MaxValue;

					var length = first > second ? ps [jd] - first + second : ps [jd];
					tk += length;
					solutions.Add (new Solution (jd, tk));

					var removed = all.Except (new List<int> { jd }).ToList ();
					if (first < second && nSub.Count () > 1) {
						all = removed;
					} else if (first == second || (first < second && nSub.Count () == 1)) {
						all = removed;
					} else {
						var elem = first - second;
						all = elem == 0 ? removed : all;
						ps [jd] = elem;
					}
				} else {
					tk++;
				}
			}
			return solutions;
		}

		public static int F (List<int> ds, List<Solution> sols)
		{
			return sols.Max (sol => sol.t - ds [sol.index]);	
		}
	}
}

