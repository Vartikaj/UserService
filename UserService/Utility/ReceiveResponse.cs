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

        public async Task<string> ListenForResponse(string responseQueue)
        {
            var connection = await _connectionTask;
            var channel = await connection.CreateChannelAsync();
            var tcs = new TaskCompletionSource<string>();

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
                Console.WriteLine($"[UserService] Received Response: {message}");
                tcs.TrySetResult(message);
                await Task.Yield(); // prevent warning
            };

            channel.BasicConsumeAsync(
                queue:responseQueue,
                autoAck: true,
                consumer: consumer
                );

            return await tcs.Task;
        }
    }
}
