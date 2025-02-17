﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Menu;
using TOPDER.Service.Dtos.Report;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IReportService
    {
        Task<bool> AddAsync(ReportDto reportDto);
        Task<bool> RemoveAsync(int id);
        Task<PaginatedList<ReportListDto>> GetPagingAsync(int pageNumber, int pageSize);
        Task<bool> HandleReportAsync(HandleReportDto handleReportDto);
    }
}
