using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodTruckCentral.Mobile.Models
{
    public class FoodTruck
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public Location Location { get; set; }
        public string PhoneNumber { get; set; }
        public string Website { get; set; }
        public List<string> Categories { get; set; }
        public List<Schedule> Schedule { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public bool IsFavorite { get; set; }
    }

    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }

    public class Schedule
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public bool IsClosed { get; set; }
    }

}
