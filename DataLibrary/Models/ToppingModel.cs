using System.ComponentModel.DataAnnotations;

namespace API_Coding_Exercise.DataLibrary.Models
{
    public class ToppingModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Topping name is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Topping name must be between 1 and 50 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? Description { get; set; }

        [Range(0.01, 999.99, ErrorMessage = "Price must be between ₱0.01 and ₱999.99")]
        public decimal Price { get; set; }

        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public List<PizzaTopping> PizzaToppings { get; set; } = new();
        public List<PizzaModel> Pizzas { get; set; } = new();
    }
}