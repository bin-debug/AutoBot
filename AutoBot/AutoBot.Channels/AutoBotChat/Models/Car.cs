using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AutoBotFramework.Models
{
    [Serializable]
    public class Car
    {
        public Car() { Images = new List<CarImage>(); }

        public string Year { get; set; }

        public string Model { get; set; }

        public string Price { get; set; }

        public string Mileage { get; set; }

        public string Engine { get; set; }

        public string Description { get; set; }

        public string URL { get; set; }

        public List<CarImage> Images { get; set; }
    }
}