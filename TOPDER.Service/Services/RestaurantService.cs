﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TOPDER.Repository.Entities;
using TOPDER.Repository.IRepositories;
using TOPDER.Repository.Repositories;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;
using TOPDER.Service.Dtos.Blog;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.IdentityModel.Tokens;
using TOPDER.Service.Dtos.Customer;

namespace TOPDER.Service.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IMapper _mapper;
        private readonly IRestaurantRepository _restaurantRepository;
        private readonly IBlogRepository _blogRepository;

        public RestaurantService(IRestaurantRepository restaurantRepository, IMapper mapper, IBlogRepository blogRepository)
        {
            _restaurantRepository = restaurantRepository;
            _mapper = mapper;
            _blogRepository = blogRepository;
        }

        public async Task<Restaurant> AddAsync(CreateRestaurantRequest restaurantRequest)
        {
            var restaurant = _mapper.Map<Restaurant>(restaurantRequest);
            restaurant.TableGapTime = 15;
            return await _restaurantRepository.CreateAndReturnAsync(restaurant);
        }

        public async Task<DescriptionRestaurant> GetDescriptionAsync(int restaurantId)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId);

            DescriptionRestaurant descriptionRestaurant = new DescriptionRestaurant()
            {
                RestaurantId = restaurantId,
                Description = restaurant.Description,
                Subdescription = restaurant.Subdescription
            };

            return descriptionRestaurant;
        }

        public async Task<bool> UpdateDescriptionAsync(int restaurantId, string? description, string? subDescription)
        {
            var restaurant = await _restaurantRepository.GetByIdAsync(restaurantId);

            if (restaurant == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(description))
            {
                restaurant.Description = description;
            }

            if (!string.IsNullOrEmpty(subDescription))
            {
                restaurant.Subdescription = subDescription;
            }

            return await _restaurantRepository.UpdateAsync(restaurant);
        }

        public async Task<bool> UpdateInfoRestaurantAsync(UpdateInfoRestaurantDto restaurant)
        {
            var existingRestaurant = await _restaurantRepository.GetByIdAsync(restaurant.Uid);
            if (existingRestaurant == null)
            {
                return false;
            }
            existingRestaurant.CategoryRestaurantId = restaurant.CategoryRestaurantId;
            existingRestaurant.NameOwner = restaurant.NameOwner;
            existingRestaurant.NameRes = restaurant.NameRes;
            if (!string.IsNullOrEmpty(restaurant.Logo))
            {
                existingRestaurant.Logo = restaurant.Logo;
            }
            existingRestaurant.OpenTime = restaurant.OpenTime;
            existingRestaurant.CloseTime = restaurant.CloseTime;
            existingRestaurant.ProvinceCity = restaurant.ProvinceCity;
            existingRestaurant.District = restaurant.District;
            existingRestaurant.Commune = restaurant.Commune;
            existingRestaurant.Address = restaurant.Address;
            existingRestaurant.Phone = restaurant.Phone;
            existingRestaurant.MaxCapacity = restaurant.MaxCapacity;
            existingRestaurant.Price = restaurant.Price;
            existingRestaurant.Description = restaurant.Description;
            existingRestaurant.TableGapTime = restaurant.TableGapTime;
            existingRestaurant.Discount = restaurant.Discount;
            return await _restaurantRepository.UpdateAsync(existingRestaurant);
        }


        public async Task<RestaurantHomeDto> GetHomeItemsAsync()
        {
            var queryable = await _restaurantRepository.QueryableAsync();
            var queryableBlog = await _blogRepository.QueryableAsync();

            var activeBlogs = queryableBlog
                .Include(x => x.Bloggroup)
                .Include(x => x.Admin)
                .Where(x => x.Status == Common_Status.ACTIVE);

            var enabledRestaurants = queryable
                .Include(x => x.CategoryRestaurant)
                .Include(x => x.Feedbacks)
                .Include(x => x.UidNavigation)
                .Where(r => r.IsBookingEnabled == true && r.UidNavigation.Status == Common_Status.ACTIVE);

            var restaurantDtos = enabledRestaurants.Select(r => _mapper.Map<RestaurantDto>(r)).ToList();

            var blogDtos = activeBlogs.Select(b => _mapper.Map<BlogListCustomerDto>(b)).ToList();

            var topBookingRestaurants = restaurantDtos.OrderByDescending(x => x.TotalFeedbacks).ThenByDescending(x => x.ReputationScore).Take(4).ToList();

            var topStarRestaurants = restaurantDtos.OrderByDescending(x => x.Star)
                                                   .ThenByDescending(x => x.ReputationScore)
                                                   .ThenByDescending(x => x.TotalFeedbacks)
                                                   .Take(4).ToList();

            var newRestaurants = restaurantDtos.OrderByDescending(x => x.Uid).Take(4).ToList();

            var topBlogs = blogDtos.OrderByDescending(x => x.BlogId).Take(4).ToList();

            return new RestaurantHomeDto
            {
                TopBookingRestaurants = topBookingRestaurants,
                TopRatingRestaurant = topStarRestaurants,
                NewRestaurants = newRestaurants,
                Blogs = topBlogs
            };
        }


        public async Task<RestaurantDetailDto> GetItemAsync(int id)
        {
            var query = await _restaurantRepository.QueryableAsync();

            var restaurant = await query
                .Include(x => x.CategoryRestaurant)
                .Include(x => x.Feedbacks)
                .Include(x => x.Images)
                .Include(x => x.Menus)
                .Include(x => x.UidNavigation)
                .FirstOrDefaultAsync(x => x.Uid == id && x.UidNavigation.Status == Common_Status.ACTIVE);

            if (restaurant == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy nhà hàng với ID {id}.");
            }

            if (restaurant.IsBookingEnabled == false)
            {
                throw new InvalidOperationException("Nhà hàng này hiện không cho phép đặt chỗ.");
            }

            var restaurantDto = _mapper.Map<RestaurantDetailDto>(restaurant);

            return restaurantDto;
        }

        public async Task<List<RestaurantDto>> GetRelateRestaurantByCategoryAsync(int restaurantId, int restaurantCategoryId)
        {
            var query = await _restaurantRepository.QueryableAsync();

            var relateRestaurants = await query
                .Include(x => x.CategoryRestaurant)
                .Include(x => x.Feedbacks)
                .Include(x => x.UidNavigation)
                .Where(x => x.CategoryRestaurantId == restaurantCategoryId
                && x.Uid != restaurantId && x.IsBookingEnabled == true && x.UidNavigation.Status == Common_Status.ACTIVE)
                .Take(10)
                .OrderByDescending(x => x.ReputationScore)
                .ToListAsync();

            var relateRestaurantDto = _mapper.Map<List<RestaurantDto>>(relateRestaurants);

            return relateRestaurantDto;
        }



        public async Task<PaginatedList<RestaurantDto>> GetItemsAsync(int pageNumber, int pageSize, string? name,
            string? address, string? provinceCity, string? district, string? commune , int? restaurantCategory, decimal? minPrice, decimal? maxPrice, int? maxCapacity)
        {
            // setup tra ra all luon hehe
            pageSize = 1000;

            var queryable = await _restaurantRepository.QueryableAsync();

            queryable = queryable
                .Include(x => x.CategoryRestaurant)
                .Include(x => x.Feedbacks)
                .Include(x => x.UidNavigation)
                .Where(r => r.IsBookingEnabled == true && r.UidNavigation.Status == Common_Status.ACTIVE)
                .OrderByDescending(x => x.ReputationScore);

            if (!string.IsNullOrEmpty(name))
            {
                queryable = queryable.Where(r => r.NameRes.Contains(name));
            }

            if (!string.IsNullOrEmpty(address))
            {
                queryable = queryable.Where(r => r.Address.Contains(address));
            }

            if (!string.IsNullOrEmpty(provinceCity))
            {
                queryable = queryable.Where(r => r.ProvinceCity == provinceCity);
            }

            if (!string.IsNullOrEmpty(district))
            {
                queryable = queryable.Where(r => r.District == district);
            }

            if (!string.IsNullOrEmpty(commune))
            {
                queryable = queryable.Where(r => r.Commune == commune);
            }

            if (restaurantCategory.HasValue)
            {
                queryable = queryable.Where(r => r.CategoryRestaurantId == restaurantCategory.Value);
            }

            if (minPrice.HasValue)
            {
                queryable = queryable.Where(r => r.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                queryable = queryable.Where(r => r.Price <= maxPrice.Value);
            }

            if (maxCapacity.HasValue)
            {
                queryable = queryable.Where(r => r.MaxCapacity >= maxCapacity.Value);
            }

            var queryDTO = queryable.Select(r => _mapper.Map<RestaurantDto>(r));

            var paginatedDTOs = await PaginatedList<RestaurantDto>.CreateAsync(
                queryDTO.AsNoTracking(),
                pageNumber > 0 ? pageNumber : 1,
                pageSize > 0 ? pageSize : 1000
            );

            return paginatedDTOs;
        }


        public async Task<bool> IsEnabledBookingAsync(int id, bool isEnabledBooking)
        {
            var existingRestaurant = await _restaurantRepository.GetByIdAsync(id);
            if (existingRestaurant == null)
            {
                throw new Exception("Restaurant not found."); // Hoặc bạn có thể ném ra ngoại lệ cụ thể hơn
            }

            if (isEnabledBooking == existingRestaurant.IsBookingEnabled)
            {
                return false; // Không có sự thay đổi
            }

            existingRestaurant.IsBookingEnabled = isEnabledBooking;
            return await _restaurantRepository.UpdateAsync(existingRestaurant);
        }

        public async Task<RestaurantProfileDto?> Profile(int uid)
        {
            var query = await _restaurantRepository.QueryableAsync();

            var restaurant = query
                .Include(x => x.CategoryRestaurant)
                .Include(x => x.UidNavigation)
                .FirstOrDefault(x => x.Uid == uid);

            if (restaurant == null) return null;

            return _mapper.Map<RestaurantProfileDto>(restaurant);
        }

        public async Task<bool> UpdateReputationScore(int uid)
        {
            var query = await _restaurantRepository.GetByIdAsync(uid);

            query.ReputationScore = query.ReputationScore - 1;

            return await _restaurantRepository.UpdateAsync(query);
        }
    }
}
