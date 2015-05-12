using System.Collections.Generic;

namespace Orienteer.Data
{
	public class GroupedData<TData> : List<TData>
	{
		public string Key { get; set; }
	}
}