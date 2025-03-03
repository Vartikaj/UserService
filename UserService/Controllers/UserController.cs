using CommonService.Utility;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using UserService.Models;
using UserService.Services;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/controller")]
    public class UserController : ControllerBase
    {
        private ILogger _logger;
        private readonly IConnection _connection;
        private readonly grpcUserService _grpcHelper;
        private readonly UserServices _userService;
        public UserController(ILogger<UserController> logger,RabbitMQConnectionHelper rabbitMQConnectionHelper, grpcUserService grpcUser, UserServices userService)
        {
            _logger = logger;
            _connection = rabbitMQConnectionHelper.GetConnection();
            _grpcHelper = grpcUser;
            _userService = userService;
        }

        [HttpPost("ProducerRequest")]
        public async Task<IActionResult> Producer([FromBody] OrderModel order)
        {
            _grpcHelper.SendMessageAsync(order, "orderQueue");
            return Ok("Order sent to Rabbit MQ");
        }
    }
}
