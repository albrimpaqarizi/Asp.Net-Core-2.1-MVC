using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Models;

namespace WebApplication1.Areas.Admin.Models
{
    public class ManageUsersViewModel
    {
        public int Id { get; set; }
        public IEnumerable<ApplicationUser> AssignedUsers { get; set; }
        public IEnumerable<ApplicationUser> AvailableUsers { get; set; }

        public IEnumerable<SelectListItem> AvailableUsersSelectList
        {
            get
            {
                return AvailableUsers != null ? AvailableUsers.Select(u => new SelectListItem { Text = u.Email, Value = u.Id }) : null;
            }
        }
    }
}
