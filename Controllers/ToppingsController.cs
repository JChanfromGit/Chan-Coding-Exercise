using API_Coding_Exercise.DataLibrary.Data;
using API_Coding_Exercise.DataLibrary.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_Coding_Exercise.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ToppingsController : ControllerBase
    {
        private readonly IToppingData _toppingData;
        private readonly ILogger<ToppingsController> _logger;

        public ToppingsController(IToppingData toppingData, ILogger<ToppingsController> logger)
        {
            _toppingData = toppingData;
            _logger = logger;
        }

        
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ToppingModel>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<List<ToppingModel>>>> GetAllToppings()
        {
            try
            {
                var toppings = await _toppingData.GetAllToppingsAsync();
                return Ok(ApiResponse<List<ToppingModel>>.SuccessResponse(toppings, "Toppings retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all toppings");
                return StatusCode(500, ApiResponse<List<ToppingModel>>.ErrorResponse("An error occurred while retrieving toppings"));
            }
        }

        
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ToppingModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ToppingModel>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ToppingModel>>> GetTopping(int id)
        {
            try
            {
                var topping = await _toppingData.GetToppingByIdAsync(id);
                if (topping == null)
                {
                    return NotFound(ApiResponse<ToppingModel>.ErrorResponse("Topping not found"));
                }

                return Ok(ApiResponse<ToppingModel>.SuccessResponse(topping, "Topping retrieved successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving topping with ID: {Id}", id);
                return StatusCode(500, ApiResponse<ToppingModel>.ErrorResponse("An error occurred while retrieving the topping"));
            }
        }

        
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ToppingModel>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<ToppingModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<ToppingModel>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<ToppingModel>>> CreateTopping([FromBody] CreateToppingDto createToppingDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToList();
                    return BadRequest(ApiResponse<ToppingModel>.ErrorResponse("Validation failed", errors));
                }

                
                var existingTopping = await _toppingData.ToppingExistsByNameAsync(createToppingDto.Name);
                if (existingTopping)
                {
                    return Conflict(ApiResponse<ToppingModel>.ErrorResponse($"A topping with the name '{createToppingDto.Name}' already exists"));
                }

                var topping = new ToppingModel
                {
                    Name = createToppingDto.Name,
                    Description = createToppingDto.Description,
                    Price = createToppingDto.Price,
                    IsAvailable = createToppingDto.IsAvailable
                };

                var createdTopping = await _toppingData.CreateToppingAsync(topping);
                return CreatedAtAction(nameof(GetTopping), new { id = createdTopping.Id },
                    ApiResponse<ToppingModel>.SuccessResponse(createdTopping, "Topping created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating topping");
                return StatusCode(500, ApiResponse<ToppingModel>.ErrorResponse("An error occurred while creating the topping"));
            }
        }

        
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ToppingModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ToppingModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<ToppingModel>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<ToppingModel>), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<ToppingModel>>> UpdateTopping(int id, [FromBody] UpdateToppingDto updateToppingDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.SelectMany(x => x.Value.Errors).Select(x => x.ErrorMessage).ToList();
                    return BadRequest(ApiResponse<ToppingModel>.ErrorResponse("Validation failed", errors));
                }

                var existingTopping = await _toppingData.GetToppingByIdAsync(id);
                if (existingTopping == null)
                {
                    return NotFound(ApiResponse<ToppingModel>.ErrorResponse("Topping not found"));
                }

                
                if (!string.IsNullOrEmpty(updateToppingDto.Name) && updateToppingDto.Name != existingTopping.Name)
                {
                    var nameExists = await _toppingData.ToppingExistsByNameAsync(updateToppingDto.Name, id);
                    if (nameExists)
                    {
                        return Conflict(ApiResponse<ToppingModel>.ErrorResponse($"A topping with the name '{updateToppingDto.Name}' already exists"));
                    }
                }

                
                if (!string.IsNullOrEmpty(updateToppingDto.Name))
                    existingTopping.Name = updateToppingDto.Name;
                if (updateToppingDto.Description != null)
                    existingTopping.Description = updateToppingDto.Description;
                if (updateToppingDto.Price.HasValue)
                    existingTopping.Price = updateToppingDto.Price.Value;
                if (updateToppingDto.IsAvailable.HasValue)
                    existingTopping.IsAvailable = updateToppingDto.IsAvailable.Value;

                var updatedTopping = await _toppingData.UpdateToppingAsync(id, existingTopping);
                return Ok(ApiResponse<ToppingModel>.SuccessResponse(updatedTopping, "Topping updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating topping with ID: {Id}", id);
                return StatusCode(500, ApiResponse<ToppingModel>.ErrorResponse("An error occurred while updating the topping"));
            }
        }

        
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteTopping(int id)
        {
            try
            {
                var deleted = await _toppingData.DeleteToppingAsync(id);
                if (!deleted)
                {
                    return NotFound(ApiResponse<object>.ErrorResponse("Topping not found"));
                }

                return Ok(ApiResponse<object>.SuccessResponse(null, "Topping deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting topping with ID: {Id}", id);
                return StatusCode(500, ApiResponse<object>.ErrorResponse("An error occurred while deleting the topping"));
            }
        }
    }
}
