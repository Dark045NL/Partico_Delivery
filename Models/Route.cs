using System.Collections.Generic;

namespace Partico_Delivery.Models
{
    public class Route
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<RouteOrder> Orders { get; set; } = new List<RouteOrder>();
        public int CurrentOrderIndex { get; set; } = 0;
    }

    public class RouteOrder
    {
        public int OrderId { get; set; }
        public int Sequence { get; set; } // volgorde in de route
        public bool Delivered { get; set; } = false;
    }
}
