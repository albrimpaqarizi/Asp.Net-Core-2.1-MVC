using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class CarUser
    {
        public int CarsId { get; set; }
        public Cars Cars { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
