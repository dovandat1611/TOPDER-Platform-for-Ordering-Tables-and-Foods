﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Services;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Restaurant;
using TOPDER.Service.IServices;
using TOPDER.Service.Utils;
using static TOPDER.Service.Common.ServiceDefinitions.Constants;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly IRestaurantService _restaurantService;
        private readonly ICloudinaryService _cloudinaryService;

        public RestaurantController(IRestaurantService restaurantService, ICloudinaryService cloudinaryService)
        {
            _restaurantService = restaurantService;
            _cloudinaryService = cloudinaryService;
        }

        // CUSTOMER SITE
        [HttpGet("ServiceForCustomerSite")]
        [SwaggerOperation(Summary = "Trang dịch vụ để search nhà hàng theo (Address, Name, ProvinceCity,District,Commune, Price(min,max), Capacity, Category Restaurant): Customer")]
        public async Task<IActionResult> GetItems([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10000,
            [FromQuery] string? name = null, [FromQuery] string? address = null,
            [FromQuery] string? provinceCity = null, [FromQuery] string? district = null,
            [FromQuery] string? commune = null, [FromQuery] int? restaurantCategory = null,
            [FromQuery] decimal? minPrice = null, [FromQuery] decimal? maxPrice = null,
            [FromQuery] int? maxCapacity = null)
        {
            var result = await _restaurantService.GetItemsAsync(pageNumber, pageSize, name, address, provinceCity,
                district, commune, restaurantCategory, minPrice, maxPrice, maxCapacity);

            var response = new PaginatedResponseDto<RestaurantDto>(
                result,
                result.PageIndex,
                result.TotalPages,
                result.HasPreviousPage,
                result.HasNextPage
            );
            return Ok(response);
        }


        [HttpGet("HomeForCustomerSite")]
        [SwaggerOperation(Summary = "Trang Home chứa nhà hàng mới, nhà hàng uy tín, nhà hàng yêu thích, blog mới nhất... : Customer")]
        public async Task<IActionResult> GetHomeItems()
        {
            var result = await _restaurantService.GetHomeItemsAsync();
            return Ok(result);
        }

        [HttpGet("RestaurantDetailForCustomerSite/{restaurantId}")]
        [SwaggerOperation(Summary = "Xem chi tiết nhà hàng : Customer")]
        public async Task<IActionResult> GetItem(int restaurantId)
        {
            try
            {
                var result = await _restaurantService.GetItemAsync(restaurantId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // THÔNG TIN NHÀ HÀNG

        [HttpPut("UpdateRestaurantInfo")]
        [SwaggerOperation(Summary = "Cập nhật thông tin nhà hàng")]
        public async Task<IActionResult> UpdateRestaurantInfo(UpdateInfoRestaurantDto restaurantDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Dữ liệu không hợp lệ." });
            }

            var result = await _restaurantService.UpdateInfoRestaurantAsync(restaurantDto);

            if (!result)
            {
                return NotFound(new { Message = "Không tìm thấy nhà hàng." });
            }

            return Ok(new { Message = "Cập nhật thông tin thành công." });
        }


        [HttpGet("GetRelateRestaurant/{restaurantId}/{categoryRestaurantId}")]
        [SwaggerOperation(Summary = "Lấy ra những nhà hàng liên quan theo category(không tính nhà hàng hiện tại): Customer")]
        public async Task<IActionResult> GetRelatedRestaurants(int restaurantId, int categoryRestaurantId)
        {
            try
            {
                var restaurants = await _restaurantService.GetRelateRestaurantByCategoryAsync(restaurantId, categoryRestaurantId);
                return Ok(restaurants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }


        [HttpGet("GetDescription/{restaurantId}")]
        [SwaggerOperation(Summary = "Lấy thông tin mô tả của nhà hàng : Restaurant")]
        public async Task<IActionResult> GetDescription(int restaurantId)
        {
            var description = await _restaurantService.GetDescriptionAsync(restaurantId);

            if (description == null)
            {
                return NotFound(new { message = "Nhà hàng không tồn tại." });
            }

            return Ok(description);
        }

        [HttpPut("UpdateDescription/{restaurantId}")]
        [SwaggerOperation(Summary = "Cập nhật thông tin mô tả của nhà hàng : Restaurant")]
        public async Task<IActionResult> UpdateDescription(
            int restaurantId,
            [FromQuery] string? description,
            [FromQuery] string? subDescription)
        {
            var result = await _restaurantService.UpdateDescriptionAsync(restaurantId, description, subDescription);

            if (result)
            {
                return Ok(new { message = "Cập nhật mô tả thành công." });
            }
            else
            {
                return BadRequest(new { message = "Không có thay đổi nào được thực hiện hoặc nhà hàng không tồn tại." });
            }
        }

        [HttpPut("IsEnabledBooking/{restaurantId}")]
        [SwaggerOperation(Summary = "Thay đổi trạng thái Booking của nhà hàng : Restaurant")]
        public async Task<IActionResult> UpdateBookingEnabled(int restaurantId,bool isEnabledBooking)
        {
            try
            {
                bool result = await _restaurantService.IsEnabledBookingAsync(restaurantId, isEnabledBooking);
                if (result)
                {
                    return Ok(new { Message = "Trạng thái đặt bàn đã được cập nhật thành công." });
                }
                return Ok(new { Message = "Trạng thái đặt bàn không có gì thay đổi." }); // Trả về 204 No Content nếu không có thay đổi
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = $"Đã xảy ra lỗi: {ex.Message}" }); // Trả về 400 Bad Request nếu có lỗi
            }
        }

    }
}
