﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Report
{
    public class ReportDto
    {
        public int ReportId { get; set; }
        public int ReportedBy { get; set; }
        public int ReportedOn { get; set; }
        public string ReportType { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string? Status { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}