using API_Coding_Exercise.DataLibrary.Data;
using API_Coding_Exercise.DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_Coding_Exercise.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class PizzasController : ControllerBase
    {
        private readonly IPizzaData _pizzaData;
        private readonly IToppingData _toppingData;
        private readonly ILogger<PizzasController> _logger;

        public PizzasController(IPizzaData pizzaData, IToppingData toppingData, ILogger<PizzasController> logger)
        {
            _pizzaData = pizzaData;
            _toppingData = toppingData;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<PizzaModel>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<PizzaModel>>>> GetAllPizzas()
        {
            try
            {
                var pizzas = await _pizzaData.GetAllPizzasAsync();
                return Ok(ApiResponse<List<PizzaModel>>.SuccessResponse(pizzas, "Pizzas retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all pizzas");
                return StatusCode(500, ApiResponse<List<PizzaModel>>.ErrorResponse("An error occurred while retrieving pizzas"));
            }
        }

        
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PizzaModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PizzaModel>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PizzaModel>>> GetPizza(int id)
        {
            try
            {
                var pizza = await _pizzaData.GetPizzaByIdAsync(id);
                if (pizza == null)
                {
                    return NotFound(ApiResponse<PizzaModel>.ErrorResponse("Pizza not found"));
                }

                return Ok(ApiResponse<PizzaModel>.SuccessResponse(pizza, "Pizza retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pizza with ID: {Id}", id);
                return StatusCode(500, ApiResponse<PizzaModel>.ErrorResponse("An error occurred while retrieving the pizza"));
            }
        }

        
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PizzaModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<PizzaModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<PizzaModel>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<PizzaModel>>> CreatePizza([FromBody] CreatePizzaDto createPizzaDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToList();
                    return BadRequest(ApiResponse<PizzaModel>.ErrorResponse("Validation failed", errors));
                }

                
                var existingPizza = await _pizzaData.PizzaExistsByNameAsync(createPizzaDto.Name);
                if (existingPizza)
                {
                    return Conflict(ApiResponse<PizzaModel>.ErrorResponse($"A pizza with the name '{createPizzaDto.Name}' already exists"));
                }

                
                if (createPizzaDto.ToppingIds.Any())
                {
                    var existingToppings = await _toppingData.GetToppingsByIdsAsync(createPizzaDto.ToppingIds);
                    var invalidToppingIds = createPizzaDto.ToppingIds.Except(existingToppings.Select(t => t.Id)).ToList();

                    if (invalidToppingIds.Any())
                    {
                        return BadRequest(ApiResponse<PizzaModel>.ErrorResponse(
                            $"Invalid topping IDs: {string.Join(", ", invalidToppingIds)}"));
                    }
                }

                var pizza = new PizzaModel
                {
                    Name = createPizzaDto.Name,
                    Description = createPizzaDto.Description,
                    BasePrice = createPizzaDto.BasePrice,
                    Size = createPizzaDto.Size,
                    ToppingIds = createPizzaDto.ToppingIds,
                    IsAvailable = createPizzaDto.IsAvailable
                };

                var createdPizza = await _pizzaData.CreatePizzaAsync(pizza);
                return CreatedAtAction(nameof(GetPizza), new { id = createdPizza.Id },
                    ApiResponse<PizzaModel>.SuccessResponse(createdPizza, "Pizza created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating pizza");
                return StatusCode(500, ApiResponse<PizzaModel>.ErrorResponse("An error occurred while creating the pizza"));
            }
        }

        
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PizzaModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PizzaModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<PizzaModel>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<PizzaModel>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<PizzaModel>>> UpdatePizza(int id, [FromBody] UpdatePizzaDto updatePizzaDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToList();
                    return BadRequest(ApiResponse<PizzaModel>.ErrorResponse("Validation failed", errors));
                }

                var existingPizza = await _pizzaData.GetPizzaByIdAsync(id);
                if (existingPizza == null)
                {
                    return NotFound(ApiResponse<PizzaModel>.ErrorResponse("Pizza not found"));
                }

                
                if (!string.IsNullOrEmpty(updatePizzaDto.Name) && updatePizzaDto.Name != existingPizza.Name)
                {
                    var nameExists = await _pizzaData.PizzaExistsByNameAsync(updatePizzaDto.Name, id);
                    if (nameExists)
                    {
                        return Conflict(ApiResponse<PizzaModel>.ErrorResponse($"A pizza with the name '{updatePizzaDto.Name}' already exists"));
                    }
                }

                
                if (!string.IsNullOrEmpty(updatePizzaDto.Name))
                    existingPizza.Name = updatePizzaDto.Name;
                if (updatePizzaDto.Description != null)
                    existingPizza.Description = updatePizzaDto.Description;
                if (updatePizzaDto.BasePrice.HasValue)
                    existingPizza.BasePrice = updatePizzaDto.BasePrice.Value;
                if (!string.IsNullOrEmpty(updatePizzaDto.Size))
                    existingPizza.Size = updatePizzaDto.Size;
                if (updatePizzaDto.IsAvailable.HasValue)
                    existingPizza.IsAvailable = updatePizzaDto.IsAvailable.Value;

                var updatedPizza = await _pizzaData.UpdatePizzaAsync(id, existingPizza);
                return Ok(ApiResponse<PizzaModel>.SuccessResponse(updatedPizza, "Pizza updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating pizza with ID: {Id}", id);
                return StatusCode(500, ApiResponse<PizzaModel>.ErrorResponse("An error occurred while updating the pizza"));
            }
        }

        
        [HttpPut("{id}/toppings")]
        [ProducesResponseType(typeof(ApiResponse<PizzaModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PizzaModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<PizzaModel>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PizzaModel>>> UpdatePizzaToppings(int id, [FromBody] UpdatePizzaToppingsDto updateToppingsDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToList();
                    return BadRequest(ApiResponse<PizzaModel>.ErrorResponse("Validation failed", errors));
                }

                
                if (updateToppingsDto.ToppingIds.Any())
                {
                    var existingToppings = await _toppingData.GetToppingsByIdsAsync(updateToppingsDto.ToppingIds);
                    var invalidToppingIds = updateToppingsDto.ToppingIds.Except(existingToppings.Select(t => t.Id)).ToList();

                    if (invalidToppingIds.Any())
                    {
                        return BadRequest(ApiResponse<PizzaModel>.ErrorResponse(
                            $"Invalid topping IDs: {string.Join(", ", invalidToppingIds)}"));
                    }
                }

                var updatedPizza = await _pizzaData.UpdatePizzaToppingsAsync(id, updateToppingsDto.ToppingIds);
                if (updatedPizza == null)
                {
                    return NotFound(ApiResponse<PizzaModel>.ErrorResponse("Pizza not found"));
                }

                return Ok(ApiResponse<PizzaModel>.SuccessResponse(updatedPizza, "Pizza toppings updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating pizza toppings for ID: {Id}", id);
                return StatusCode(500, ApiResponse<PizzaModel>.ErrorResponse("An error occurred while updating pizza toppings"));
            }
        }

        
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeletePizza(int id)
        {
            try
            {
                var deleted = await _pizzaData.DeletePizzaAsync(id);
                if (!deleted)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Pizza not found"));
                }

                return Ok(ApiResponse<object>.SuccessResponse(null, "Pizza deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting pizza with ID: {Id}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while deleting the pizza"));
            }
        }
    }
}