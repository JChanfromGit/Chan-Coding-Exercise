using Microsoft.EntityFrameworkCore;
using API_Coding_Exercise.DataLibrary.Models;

namespace API_Coding_Exercise.DataLibrary.Data
{
    public class PizzaData : IPizzaData
    {
        private readonly PizzaDbContext _context;
        private readonly IToppingData _toppingData;

        public PizzaData(PizzaDbContext context, IToppingData toppingData)
        {
            _context = context;
            _toppingData = toppingData;
        }

        public async Task<List<PizzaModel>> GetAllPizzasAsync()
        {
            var pizzas = await _context.Pizzas
                .Include(p => p.PizzaToppings)
                .ThenInclude(pt => pt.Topping)
                .OrderBy(p => p.Name)
                .ToListAsync();

            
            foreach (var pizza in pizzas)
            {
                pizza.Toppings = pizza.PizzaToppings.Select(pt => pt.Topping).ToList();
            }

            return pizzas;
        }

        public async Task<PizzaModel?> GetPizzaByIdAsync(int id)
        {
            var pizza = await _context.Pizzas
                .Include(p => p.PizzaToppings)
                .ThenInclude(pt => pt.Topping)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pizza != null)
            {
                pizza.Toppings = pizza.PizzaToppings.Select(pt => pt.Topping).ToList();
            }

            return pizza;
        }

        public async Task<PizzaModel?> GetPizzaByNameAsync(string name)
        {
            var normalizedName = NormalizeName(name);
            var pizza = await _context.Pizzas
                .Include(p => p.PizzaToppings)
                .ThenInclude(pt => pt.Topping)
                .FirstOrDefaultAsync(p => p.Name.ToLower() == normalizedName.ToLower());

            if (pizza != null)
            {
                pizza.Toppings = pizza.PizzaToppings.Select(pt => pt.Topping).ToList();
            }

            return pizza;
        }

        public async Task<PizzaModel> CreatePizzaAsync(PizzaModel pizza)
        {
            pizza.Name = NormalizeName(pizza.Name);
            pizza.CreatedAt = DateTime.UtcNow;
            pizza.UpdatedAt = DateTime.UtcNow;

            
            if (pizza.ToppingIds.Any())
            {
                var existingToppings = await _toppingData.GetToppingsByIdsAsync(pizza.ToppingIds);
                pizza.PizzaToppings = existingToppings.Select(t => new PizzaTopping { ToppingId = t.Id }).ToList();
            }

            _context.Pizzas.Add(pizza);
            await _context.SaveChangesAsync();

            
            return await GetPizzaByIdAsync(pizza.Id) ?? pizza;
        }

        public async Task<PizzaModel?> UpdatePizzaAsync(int id, PizzaModel updatedPizza)
        {
            var existingPizza = await GetPizzaByIdAsync(id);
            if (existingPizza == null)
                return null;

            existingPizza.Name = NormalizeName(updatedPizza.Name);
            existingPizza.Description = updatedPizza.Description;
            existingPizza.BasePrice = updatedPizza.BasePrice;
            existingPizza.Size = updatedPizza.Size;
            existingPizza.IsAvailable = updatedPizza.IsAvailable;
            existingPizza.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return await GetPizzaByIdAsync(id);
        }

        public async Task<PizzaModel?> UpdatePizzaToppingsAsync(int id, List<int> toppingIds)
        {
            var pizza = await _context.Pizzas
                .Include(p => p.PizzaToppings)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pizza == null)
                return null;

            
            var existingToppings = await _toppingData.GetToppingsByIdsAsync(toppingIds);
            if (existingToppings.Count != toppingIds.Count)
                return null;

            
            _context.PizzaToppings.RemoveRange(pizza.PizzaToppings);

            
            pizza.PizzaToppings = toppingIds.Select(toppingId => new PizzaTopping
            {
                PizzaId = id,
                ToppingId = toppingId
            }).ToList();

            pizza.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetPizzaByIdAsync(id);
        }

        public async Task<bool> DeletePizzaAsync(int id)
        {
            var pizza = await _context.Pizzas
                .Include(p => p.PizzaToppings)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (pizza == null)
                return false;

            _context.Pizzas.Remove(pizza);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PizzaExistsByNameAsync(string name, int? excludeId = null)
        {
            var normalizedName = NormalizeName(name);

            var query = _context.Pizzas.Where(p => p.Name.ToLower() == normalizedName.ToLower());

            if (excludeId.HasValue)
                query = query.Where(p => p.Id != excludeId);

            return await query.AnyAsync();
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