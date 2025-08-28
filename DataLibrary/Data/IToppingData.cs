using API_Coding_Exercise.DataLibrary.Models;
using System.Threading.Tasks;

namespace API_Coding_Exercise.DataLibrary.Data
{
    public interface IToppingData
    {
        Task<List<ToppingModel>> GetAllToppingsAsync();
        Task<ToppingModel?> GetToppingByIdAsync(int id);
        Task<ToppingModel?> GetToppingByNameAsync(string name);
        Task<ToppingModel> CreateToppingAsync(ToppingModel topping);
        Task<ToppingModel?> UpdateToppingAsync(int id, ToppingModel topping);
        Task<bool> DeleteToppingAsync(int id);
        Task<bool> ToppingExistsByNameAsync(string name, int? excludeId = null);
        Task<List<ToppingModel>> GetToppingsByIdsAsync(List<int> ids);
    }
}