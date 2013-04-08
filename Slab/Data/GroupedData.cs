using System.Collections.Generic;

namespace Slab.Data
{
	public class GroupedData<TData> : List<TData>
	{
		public string Key { get; set; }
	}
}