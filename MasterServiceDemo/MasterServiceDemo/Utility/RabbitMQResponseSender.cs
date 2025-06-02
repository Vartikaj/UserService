using CommonService.Utility;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Channels;

namespace MasterServiceDemo.Utility
{
    public class RabbitMQResponseSender
    {
        private readonly Task<IConnection> _connectionTask;

        public RabbitMQResponseSender(RabbitMQConnectionHelper rabbitMQConnectionHelper)
        {
            _connectionTask = rabbitMQConnectionHelper.GetConnectionAsync();
        }

        public async Task SendResponse(string responseQueue, string responseMessage)
        {
            var connection = await _connectionTask;
            var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue : responseQueue,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
               );

            var body = Encoding.UTF8.GetBytes(responseMessage);
            await channel.BasicPublishAsync(exchange: "",
                                            routingKey: responseQueue,
                                            body: body);

            Console.WriteLine($"[MasterService] Sent Response: {responseMessage}");
        }

    }
}
