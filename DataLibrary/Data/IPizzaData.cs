using API_Coding_Exercise.DataLibrary.Models;
using System.Threading.Tasks;

namespace API_Coding_Exercise.DataLibrary.Data
{
    public interface IPizzaData
    {
        Task<List<PizzaModel>> GetAllPizzasAsync();
        Task<PizzaModel?> GetPizzaByIdAsync(int id);
        Task<PizzaModel?> GetPizzaByNameAsync(string name);
        Task<PizzaModel> CreatePizzaAsync(PizzaModel pizza);
        Task<PizzaModel?> UpdatePizzaAsync(int id, PizzaModel pizza);
        Task<PizzaModel?> UpdatePizzaToppingsAsync(int id, List<int> toppingIds);
        Task<bool> DeletePizzaAsync(int id);
        Task<bool> PizzaExistsByNameAsync(string name, int? excludeId = null);
    }
}