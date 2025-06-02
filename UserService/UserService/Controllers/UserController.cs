using CommonService.Utility;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using UserService.Interfaces;
using UserService.Models;
using UserService.Services;
using UserService.Utility;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/controller")]
    public class UserController : ControllerBase
    {
        private ILogger _logger;
        private IUser _user;
        private readonly Task<IConnection> _connectionTask;
        private readonly grpcUserService _grpcHelper;
        private readonly UserServices _userServices;
        private readonly RabbitMQConnectionHelper _rabbitMQ;
        public UserController(ILogger<UserController> logger, IUser user, RabbitMQConnectionHelper rabbitMQConnectionHelper, grpcUserService grpcUser, UserServices userServices)
        {
            _logger = logger;
            _user = user;
            _connectionTask = rabbitMQConnectionHelper.GetConnectionAsync();
            _grpcHelper = grpcUser;
            _userServices = userServices;
            _rabbitMQ = rabbitMQConnectionHelper;
        }

        [HttpPost("ProducerRequest")]
        public async Task<IActionResult> Producer([FromBody] OrderModel order)
        {
            string requestQueue = "requestQueue";
            string responseQueue = "responseQueue";

            var rabbitClient = new grpcUserService(_rabbitMQ);
            string response = await rabbitClient.SendMessageAsync(requestQueue, responseQueue, order);

            return Ok($"Response from Master: {response}");
        }
    }
}
