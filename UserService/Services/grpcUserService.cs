using CommonService.Utility;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace UserService.Services
{
    public class grpcUserService
    {
        private readonly IConnection _connection;
        public grpcUserService(RabbitMQConnectionHelper rabbitMq)
        {
            _connection = rabbitMq.GetConnection();
        }

        public async Task SendMessageAsync<T> (T message, string queueName)
        {
            using var channel = await _connection.CreateChannelAsync();

            //Declare a queue
            await channel.QueueDeclareAsync(queue: "hello", durable:false, exclusive:false, autoDelete : false, arguments : null);
            string jsonMessage = JsonSerializer.Serialize(message);
            byte[] body = Encoding.UTF8.GetBytes(jsonMessage);

            //Publish message
            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: queueName,
                body: body);
        }

    }
}
