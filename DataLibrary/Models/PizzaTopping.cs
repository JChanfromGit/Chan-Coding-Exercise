namespace API_Coding_Exercise.DataLibrary.Models
{
    public class PizzaTopping
    {
        public int PizzaId { get; set; }
        public PizzaModel Pizza { get; set; } = null!;

        public int ToppingId { get; set; }
        public ToppingModel Topping { get; set; } = null!;
    }
}