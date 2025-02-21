using CommonService.Utility;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using UserService.Interfaces;
using UserService.Models;
using UserService.Services;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/controller")]
    public class UserController : ControllerBase
    {
        private ILogger _logger;
        private IUser _user;
        private readonly IConnection _connection;
        private readonly grpcUserService _grpcHelper;
        private UserController(ILogger<UserController> logger, IUser user, RabbitMQConnectionHelper rabbitMQConnectionHelper, grpcUserService grpcUser)
        {
            _logger = logger;
            _user = user;
            _connection = rabbitMQConnectionHelper.GetConnection();
            _grpcHelper = grpcUser;
        }

        [HttpPost("ProducerRequest")]
        public async Task<IActionResult> Producer([FromBody] OrderModel order)
        {
            _grpcHelper.SendMessageAsync(order, "orderQueue");
            return Ok("Order sent to Rabbit MQ");
        }
    }
}
