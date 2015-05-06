using System.Collections.Generic;
using System.Linq;

namespace WindowsFormsApplication1
{
	public class RingBuffer<A>
	{
		public readonly int index;
		public readonly List<A> data;

		public RingBuffer (int index, List<A> data)
		{
			this.index = index;
			this.data = data;
		}

		public RingBuffer<A> ShiftLeft ()
		{
			return new RingBuffer<A> ((index + 1) % data.Count (), data);
		}

		public RingBuffer<A> ShiftRight ()
		{
			return new RingBuffer<A> ((index + data.Count () - 1) % data.Count (), data);
		}

		public int Count { get { return data.Count; } }

		public A this [int i] {
			get {
				return data [(index + i) % data.Count ()];
			}
		}

		public List<A> ToList ()
		{
			var list = new List<A> (Count);
			for (int i = 0; i < Count; i++) {
				list.Add (this [i]);
			}
			return list;
		}

		public A Last ()
		{
			return this [Count - 1];
		}
	}
}