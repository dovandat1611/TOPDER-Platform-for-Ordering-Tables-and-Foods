﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOPDER.Service.Dtos.CategoryRestaurant;
using TOPDER.Service.Utils;

namespace TOPDER.Service.IServices
{
    public interface ICategoryRestaurantService
    {
        Task<bool> AddAsync(CategoryRestaurantDto categoryRestaurantDto);
        Task<bool> UpdateAsync(CategoryRestaurantDto categoryRestaurantDto);
        Task<CategoryRestaurantDto> UpdateItemAsync(int id);
        Task<List<CategoryRestaurantDto>> CategoryExistAsync();
        Task<List<CategoryRestaurantViewDto>> GetAllCategoryRestaurantAsync();
        Task<bool> RemoveAsync(int id);
        Task<PaginatedList<CategoryRestaurantViewDto>> ListPagingAsync(int pageNumber, int pageSize, string? categoryRestaurantName);
    }
}
