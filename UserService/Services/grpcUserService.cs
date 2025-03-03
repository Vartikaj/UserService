using CommonService.Utility;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace UserService.Services
{
    public class grpcUserService
    {
        private readonly Task<IConnection> _connectionTask;
        public grpcUserService(RabbitMQConnectionHelper rabbitMq)
        {
            _connectionTask = rabbitMq.GetConnectionAsync();
        }

        public async Task SendMessageAsync<T> (string requestQueue, string responseQueue, T message)
        {
            var connection = await _connectionTask;
            var channel = await  connection.CreateChannelAsync();

            //Declare a queue
            await channel.QueueDeclareAsync(
                queue: requestQueue, 
                durable:false, 
                exclusive:false, 
                autoDelete : false, 
                arguments : null
               );

            var props = await channel.BasicProperties();
            props.ReplyTo = responseQueue;
            

            string jsonMessage = JsonSerializer.Serialize(message);
            byte[] body = Encoding.UTF8.GetBytes(jsonMessage);

            //Publish message
            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: requestQueue,
                body: body);

            Console.WriteLine($"[Master] Sent Request: {message}");
        }

    }
}
