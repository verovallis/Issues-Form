﻿using System.ComponentModel.DataAnnotations;

namespace Issues_Form.Models
{
    public class FormDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = "";
        [Required, MaxLength(100)]
        public string Email { get; set; } = "";
        [MaxLength(100)]
        public string? CCEmail { get; set; }
        [Required, MaxLength(100)]
        public string PhoneNumber { get; set; } = "";
        [Required, MaxLength(100)]
        public string Subject { get; set; } = "";
        [Required, MaxLength(100)]
        public string Category { get; set; } = "";
        [Required, MaxLength(100)]
        public string Building { get; set; } = "";
        [Required, MaxLength(100)]
        public string Company { get; set; } = "";
        [Required]
        public string Description { get; set; } = "";
        public IFormFile? Attach { get; set; }
        public string Status { get; set; } = "Not Started";
        public string? AdminComment { get; set; } = "";
    }
    public class Category_Param
    {
        public int Id { get; set; }
        public string? Category_Issues { get; set; }
    }
}

// Building_Param.cs
namespace Issues_Form.Models
{
    public class Building_Param
    {
        public int Id { get; set; }
        public string? Building { get; set; }
    }
}

// Company_Param.cs
namespace Issues_Form.Models
{
    public class Company_Param
    {
        public int Id { get; set; }
        public string? Company_Name { get; set; }
    }
}