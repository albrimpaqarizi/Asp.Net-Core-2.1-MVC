using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Cars
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Location { get; set; }
        public string CompanyName { get; set; }
        public DateTime PostedDate { get; set; }
        public float Price { get; set; }
        public string Fuel { get; set; }
        public string Transmission { get; set; }
        public int Doors { get; set; }
        public int Seats { get; set; }
        public bool AirConditioning { get; set; }
        public bool ABS { get; set; }
        public bool FullToFull { get; set; }
        public bool Casco { get; set; }

        public List<CarUser> CarUsers { get; set; }
    }
}
