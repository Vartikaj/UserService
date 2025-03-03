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


            await _grpcHelper.SendMessageAsync(requestQueue, responseQueue, "Hello from User!");

            var listener = new Utility.ReceiveResponse(_rabbitMQ);
            listener.ListenForResponse(responseQueue);
            // await _grpcHelper.SendMessageAsync(order, "orderQueue");
            return Ok("Order sent to Rabbit MQ");
        }
    }
}
