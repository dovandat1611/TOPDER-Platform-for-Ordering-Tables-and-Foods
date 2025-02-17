﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TOPDER.Service.Common.CommonDtos;
using TOPDER.Service.Dtos.Blog;
using TOPDER.Service.Dtos.CategoryMenu;
using TOPDER.Service.IServices;
using TOPDER.Service.Services;

namespace TOPDER.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryMenuController : ControllerBase
    {
        private readonly ICategoryMenuService _categoryMenuService;

        public CategoryMenuController(ICategoryMenuService categoryMenuService)
        {
            _categoryMenuService = categoryMenuService;
        }

        [HttpPost("Create")]
        [SwaggerOperation(Summary = "Tạo Category Menu: Restaurant")]
        public async Task<IActionResult> CreateCategoryMenu([FromBody] CreateCategoryMenuDto categoryMenuDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryMenuService.AddAsync(categoryMenuDto);
            if (result)
            {
                return Ok("Tạo Category Menu thành công.");
            }

            return BadRequest("Tạo Category Menu thất bại.");
        }

        [HttpGet("GetAllCategoryMenus/{restaurantId}")]
        [SwaggerOperation(Summary = "Lấy ra tất cả danh sách Category Menu để chọn UPDATE và ADD: Restaurant")]
        public async Task<IActionResult> GetAllCategoryMenus(int restaurantId)
        {
            var result = await _categoryMenuService.GetAllCategoryMenuAsync(restaurantId);
            return Ok(result);
        }


        [HttpGet("GetCategoryMenu/{restaurantId}/{categoryMenuId}")]
        [SwaggerOperation(Summary = "Lấy ra một Category Menu để Update: Restaurant")]
        public async Task<IActionResult> GetCategoryMenu(int restaurantId, int categoryMenuId)
        {
            if (restaurantId <= 0)
            {
                return BadRequest("Restaurant ID must be greater than zero.");
            }

            try
            {   
                var categoryMenu = await _categoryMenuService.GetItemAsync(categoryMenuId, restaurantId);
                return Ok(categoryMenu);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Category Menu với ID {categoryMenuId} không tồn tại.");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid($"Category Menu với ID {categoryMenuId} không thuộc về nhà hàng với ID {restaurantId}.");
            }
        }


        [HttpGet("GetCategoryMenuList/{restaurantId}")]
        [SwaggerOperation(Summary = "Lấy danh sách Category Menu của Nhà Hàng (có thể Search theo Name): Restaurant")]
        public async Task<IActionResult> ListPaging(
            int restaurantId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? categoryMenuName = null)
        {
            if (restaurantId <= 0)
            {
                return BadRequest("Restaurant ID must be greater than zero.");
            }

            try
            {
                var result = await _categoryMenuService.ListPagingAsync(pageNumber, pageSize, restaurantId, categoryMenuName);
                var response = new PaginatedResponseDto<CategoryMenuDto>(
                    result,
                    result.PageIndex,
                    result.TotalPages,
                    result.HasPreviousPage,
                    result.HasNextPage
                );
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Đã xảy ra lỗi trong quá trình xử lý: {ex.Message}");
            }
        }



        [HttpPut("Update")]
        [SwaggerOperation(Summary = "Cập nhật Category Menu của Nhà Hàng: Restaurant")]
        public async Task<IActionResult> UpdateCategoryMenu([FromBody] CategoryMenuDto categoryMenuDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _categoryMenuService.UpdateAsync(categoryMenuDto);
            if (result)
            {
                return Ok($"Cập nhật Category Menu với ID {categoryMenuDto.CategoryMenuId} thành công.");
            }
            return NotFound($"Category Menu với ID {categoryMenuDto.CategoryMenuId} không tồn tại.");
        }

        [HttpPut("Invisible/{id}")]
        [SwaggerOperation(Summary = "Ẩn/Xóa Category Menu và những bảng liên quan của Nhà Hàng: Restaurant")]
        public async Task<IActionResult> SetInvisible(int id)
        {
            var result = await _categoryMenuService.InvisibleAsync(id);
            if (result)
            {
                return Ok("CategoryMenu và các menu liên quan đã được ẩn thành công.");
            }
            return NotFound("Không tìm thấy CategoryMenu với ID tương ứng.");
        }

    }
}
