﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.Contact;
using TOPDER.Service.Dtos.Feedback;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface IFeedbackService
    {
        Task<bool> AddAsync(FeedbackDto feedbackDto);
        Task<bool> UpdateAsync(FeedbackDto feedbackDto);
        Task<bool> RemoveAsync(int id);
        Task<PaginatedList<FeedbackAdminDto>> ListAdminPagingAsync(int pageNumber, int pageSize, int? star, string? content);
        Task<PaginatedList<FeedbackRestaurantDto>> ListRestaurantPagingAsync(int pageNumber, int pageSize, int restaurantId, int? star, string? content);
        Task<PaginatedList<FeedbackCustomerDto>> ListCustomerPagingAsync(int pageNumber, int pageSize, int restaurantId, int? star);
        Task<PaginatedList<FeedbackHistoryDto>> GetHistoryCustomerPagingAsync(int pageNumber, int pageSize, int customerId);
    }
}