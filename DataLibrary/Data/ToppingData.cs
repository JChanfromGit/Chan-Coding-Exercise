using Microsoft.EntityFrameworkCore;
using API_Coding_Exercise.DataLibrary.Models;

namespace API_Coding_Exercise.DataLibrary.Data
{
    public class ToppingData : IToppingData
    {
        private readonly PizzaDbContext _context;

        public ToppingData(PizzaDbContext context)
        {
            _context = context;
        }

        public async Task<List<ToppingModel>> GetAllToppingsAsync()
        {
            return await _context.Toppings
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<ToppingModel?> GetToppingByIdAsync(int id)
        {
            return await _context.Toppings.FindAsync(id);
        }

        public async Task<ToppingModel?> GetToppingByNameAsync(string name)
        {
            var normalizedName = NormalizeName(name);
            return await _context.Toppings
                .FirstOrDefaultAsync(t => t.Name.ToLower() == normalizedName.ToLower());
        }

        public async Task<ToppingModel> CreateToppingAsync(ToppingModel topping)
        {
            topping.Name = NormalizeName(topping.Name);
            topping.CreatedAt = DateTime.UtcNow;
            topping.UpdatedAt = DateTime.UtcNow;

            _context.Toppings.Add(topping);
            await _context.SaveChangesAsync();
            return topping;
        }

        public async Task<ToppingModel?> UpdateToppingAsync(int id, ToppingModel updatedTopping)
        {
            var existingTopping = await GetToppingByIdAsync(id);
            if (existingTopping == null)
                return null;

            existingTopping.Name = NormalizeName(updatedTopping.Name);
            existingTopping.Description = updatedTopping.Description;
            existingTopping.Price = updatedTopping.Price;
            existingTopping.IsAvailable = updatedTopping.IsAvailable;
            existingTopping.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingTopping;
        }

        public async Task<bool> DeleteToppingAsync(int id)
        {
            var topping = await _context.Toppings
                .Include(t => t.PizzaToppings)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (topping == null)
                return false;

            _context.Toppings.Remove(topping);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToppingExistsByNameAsync(string name, int? excludeId = null)
        {
            var normalizedName = NormalizeName(name);

            var query = _context.Toppings.Where(t => t.Name.ToLower() == normalizedName.ToLower());

            if (excludeId.HasValue)
                query = query.Where(t => t.Id != excludeId);

            return await query.AnyAsync(); ;
        }

        public async Task<List<ToppingModel>> GetToppingsByIdsAsync(List<int> ids)
        {
            return await _context.Toppings
                .Where(t => ids.Contains(t.Id))
                .ToListAsync();
        }

        private static string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;

            
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo
                .ToTitleCase(name.Trim().ToLower());
        }
    }
}