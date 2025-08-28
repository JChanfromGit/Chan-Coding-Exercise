using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_Coding_Exercise.DataLibrary.Models
{
    public class PizzaModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Pizza name is required")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Pizza name must be between 1 and 50 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [Range(0.01, 999.99, ErrorMessage = "Base price must be between ₱0.01 and ₱999.99")]
        public decimal BasePrice { get; set; }

        [Required(ErrorMessage = "Pizza size is required")]
        public string Size { get; set; } = "Medium";

        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public List<PizzaTopping> PizzaToppings { get; set; } = new();

        [NotMapped]
        public List<ToppingModel> Toppings { get; set; } = new();

        [NotMapped]
        public decimal TotalPrice => BasePrice + Toppings.Sum(t => t.Price);

        [NotMapped]
        public List<int> ToppingIds
        {
            get => PizzaToppings.Select(pt => pt.ToppingId).ToList();
            set
            {
                PizzaToppings = value.Select(id => new PizzaTopping { ToppingId = id }).ToList();
            }
        }
    }
}