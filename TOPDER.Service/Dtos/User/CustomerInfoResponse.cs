﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.User
{
    public class CustomerInfoResponse
    {
        public int Uid { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Image { get; set; }
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; }
        public string? Role { get; set; } = string.Empty;
    }
}
