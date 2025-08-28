using System.ComponentModel.DataAnnotations;

namespace API_Coding_Exercise.DataLibrary.Models
{
    public class CreatePizzaDto
    {
        [Required(ErrorMessage = "Pizza name is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Pizza name must be between 1 and 50 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Range(0.01, 999.99, ErrorMessage = "Base price must be between ₱0.01 and ₱999.99")]
        public decimal BasePrice { get; set; }

        [Required(ErrorMessage = "Pizza size is required")]
        [RegularExpression("^(Small|Medium|Large)$", ErrorMessage = "Size must be Small, Medium, or Large")]
        public string Size { get; set; } = "Medium";

        public List<int> ToppingIds { get; set; } = new List<int>();

        public bool IsAvailable { get; set; } = true;
    }
}