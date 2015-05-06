using System;

namespace WindowsFormsApplication1
{
	public class Solution
	{
		public readonly int index;
		public readonly int t;

		public Solution (int index, int t)
		{
			this.index = index;
			this.t = t;
		}

		public override string ToString ()
		{
			return string.Format ("Solution({0},{1})", index, t);
		}
	}
}

