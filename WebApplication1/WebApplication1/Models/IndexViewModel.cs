﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
    public class IndexViewModel
    {
      public PaginatedList<Cars> Cars { get; set; }
    }
}
