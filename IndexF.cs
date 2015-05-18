using System;
using System.Collections.Generic;
using System.Linq;

namespace WindowsFormsApplication1
{
  public class IndexF : IComparable
  {
    public  List<int> indices { get; set;}
    public  int f { get; set;}

    public IndexF (List<int> indices, int f)
    {
      this.indices = indices;
      this.f = f;
    }

    static int Compare(List<int> first, List<int> second) {
      var cmp = first.Count.CompareTo (second.Count);
      if (cmp != 0) {
        return cmp;
      }
      foreach (var tup in first.Zip (second, (f, s) => Tuple.Create (f, s))) {
        var cmp2 = tup.Item1.CompareTo (tup.Item2); 
        if (cmp2!=0) {
          return cmp2;
        }
      }
      return 0;
    }

    int IComparable.CompareTo (object obj)
    {
      if (this.Equals (obj)) {
        return 0;
      }

      var i = (IndexF)obj;
      var cmp = Compare (this.indices, i.indices);
      var result = this.f.CompareTo (i.f);
      return result!=0 ? result : cmp;
    }

    public override bool Equals (object obj)
    {
      var i = (IndexF)obj;
      return i.indices.SequenceEqual (this.indices) && i.f.Equals (f);
    }
    public override int GetHashCode ()
    {
      int hash = 13;
      hash = (hash * 7) + indices.GetHashCode();
      hash = (hash * 7) + f.GetHashCode();
        return hash;
    }
  }
}