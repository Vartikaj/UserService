using CommonService.Utility;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace UserService.Utility
{
    public class ReceiveResponse
    {
        private readonly Task<IConnection> _connectionTask;

        public ReceiveResponse(RabbitMQConnectionHelper rabbitMQConnectionHelper)
        {
            _connectionTask = rabbitMQConnectionHelper.GetConnectionAsync();
        }

        public async void ListenForResponse(string responseQueue)
        {
            var connection = await _connectionTask;
            var channel = await connection.CreateChannelAsync();

            channel.QueueDeclareAsync(queue: responseQueue,
                durable: false,
                autoDelete: false,
                exclusive: false,
                arguments: null
                );

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(message);
            };

            channel.BasicConsumeAsync(
                queue:responseQueue,
                autoAck: true,
                consumer: consumer
                );

            Console.WriteLine($"[Master] Sent Request: {message}");
        }
    }
}
