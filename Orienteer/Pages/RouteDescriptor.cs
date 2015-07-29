using System.Collections.Generic;

namespace Orienteer.Pages
{
    public class RouteDescriptor
    {
        public RouteDescriptor()
        {
            ParameterValues = new List<object>();
        }

        public string Route { get; set; }
        public IList<object> ParameterValues { get; private set; }
    }
}