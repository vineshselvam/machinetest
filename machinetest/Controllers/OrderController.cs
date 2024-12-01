using machinetest.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace machinetest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderRepository _repository;

        public OrderController(OrderRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("recent-order")]
        public async Task<IActionResult> GetMostRecentOrder([FromBody] JsonElement request)
        {
            try
            {
                string email = request.GetProperty("user").GetString();
                int customerId = request.GetProperty("customerId").GetInt32();

                if (string.IsNullOrEmpty(email) || customerId <= 0)
                {
                    return BadRequest("Invalid input.");
                }

                var result = await _repository.GetMostRecentOrderAsync(email, customerId);

                if (result == null || result.Order == null)
                {
                    return NotFound("No recent order found for the customer.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
