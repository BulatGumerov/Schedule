using System;
using System.Collections.Generic;

namespace WindowsFormsApplication1
{
	public class Input
	{
		public readonly List<int> rs;
		public readonly List<int> ps;
		public readonly List<int> ds;

		public Input (List<int> rs, List<int> ps, List<int> ds)
		{
			this.rs = rs;
			this.ps = ps;
			this.ds = ds;
		}
	}
}

