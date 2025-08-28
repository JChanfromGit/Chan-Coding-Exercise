using System.ComponentModel.DataAnnotations;

namespace API_Coding_Exercise.DataLibrary.Models
{
    public class UpdatePizzaToppingsDto
    {
        [Required]
        public List<int> ToppingIds { get; set; } = new List<int>();
    }
}