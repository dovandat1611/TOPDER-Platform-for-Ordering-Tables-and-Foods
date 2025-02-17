﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TOPDER.Service.Dtos.Restaurant
{
    public class CreateRestaurantRequest
    {
        public int Uid { get; set; }

        [Required(ErrorMessage = "CategoryId is required.")]
        public int? CategoryRestaurantId { get; set; }

        [Required(ErrorMessage = "NameOwner is required.")]
        [StringLength(100, ErrorMessage = "NameOwner cannot be longer than 100 characters.")]
        public string NameOwner { get; set; } = null!;

        [Required(ErrorMessage = "NameRes is required.")]
        [StringLength(150, ErrorMessage = "NameRes cannot be longer than 150 characters.")]
        public string NameRes { get; set; } = null!;
        public string? Logo { get; set; }

        [Required(ErrorMessage = "File is required.")]
        public IFormFile? File { get; set; }

        [Required(ErrorMessage = "OpenTime is required.")]
        public TimeSpan OpenTime { get; set; }

        [Required(ErrorMessage = "CloseTime is required.")]
        public TimeSpan CloseTime { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(200, ErrorMessage = "Address cannot be longer than 200 characters.")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "Phone is required.")]
        [Phone(ErrorMessage = "Phone must be a valid phone number.")]
        public string Phone { get; set; } = null!;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email must be a valid email address.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.00, (double)decimal.MaxValue, ErrorMessage = "Price must be greater or equal zero.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Maximum capacity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Maximum capacity must be at least 1.")]
        public int MaxCapacity { get; set; }

        public string? ProvinceCity { get; set; }
        public string? District { get; set; }
        public string? Commune { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Business license is required.")]
        public string? Subdescription { get; set; }
    }
}
