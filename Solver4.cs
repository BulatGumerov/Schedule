using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsFormsApplication1
{
  public class Solver4
  {
    static  IEnumerable<T> Single<T>(T t)
    { 
      yield return t;
    }

    static void Enqueue(Input input, PriorityQueue<IndexF> queue, IEnumerable<int> indices, IEnumerable<int> interm) {
      foreach (var i in indices) {
        var s = interm.Concat(Single (i)).ToList ();
        queue.Enqueue (new IndexF(s, Math.Max(0,Solver1.F (input.ds, Solver2.FromIndicies (input, s)))));
      }
    }

    public static List<Solution> Solve (Input input)
    {
      var indices = Enumerable.Range (0, input.ds.Count).ToList();
      var queue = new PriorityQueue<IndexF> ();
      Enqueue (input, queue, indices, Enumerable.Empty<int>());
      IndexF saved = null;
      while (queue.Any ()) {
        var d = queue.Dequeue ();
        Enqueue (input, queue, indices.Except (d.indices), d.indices);

        if (d.indices.Count == indices.Count) {
          saved = d;
          queue.RemoveWhere (i => i.indices.Count <= d.indices.Count && i.f >= d.f);
        }
      }
      return Solver2.FromIndicies(input,saved.indices);
    }
  }
}