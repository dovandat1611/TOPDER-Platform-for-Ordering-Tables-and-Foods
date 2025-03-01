﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Dashboard
{
    public class TopRestaurantDTO
    {
        public int RestaurantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public int TotalOrder { get; set; }
        public double TotalInCome { get; set; }
    }
}
