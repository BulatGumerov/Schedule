using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsFormsApplication1
{
  public class PriorityQueue<T> where T:IComparable
  {
    public SortedSet<T> data {get; set;}

    public PriorityQueue() {
      data = new SortedSet<T> ();
    }

    public void Enqueue (T t)
    {
      data.Add (t);
    }

    public T Dequeue ()
    {
      var l = data.First ();
      data.Remove (l);
      return l;
    }

    public bool Any ()
    {
      return data.Any ();
    }

    public void RemoveWhere (Predicate<T> match)
    {
      data.RemoveWhere (match);
    }
  }
}