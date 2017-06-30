using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class QueryRequest
    {
        public string Model { get; set; }
        public string Year { get; set; }
        public string Milage { get; set; }
        public string GearBox { get; set; }
        public string Fuel { get; set; }
        public string Colour { get; set; }
    }
}
