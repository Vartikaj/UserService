using CommonService.Utility;
using MasterServiceDemo.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json;

namespace MasterServiceDemo.Utility
{
    public class ConsumerService
    {
        private readonly Task<IConnection> _connectionTask;
        private readonly RabbitMQConnectionHelper _rabbit;

        public ConsumerService(RabbitMQConnectionHelper rabbitMQConnectionHelper)
        {
            _connectionTask = rabbitMQConnectionHelper.GetConnectionAsync();
            _rabbit = rabbitMQConnectionHelper;
        }

        public async Task ConsumeQueueAsync(string queueName)
        {
            var connection = await _connectionTask;
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: queueName, durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                passive: false,
                cancellationToken: CancellationToken.None);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, eventArgs) =>
            {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var replyToQueue = eventArgs.BasicProperties.ReplyTo;
                var correlationId = eventArgs.BasicProperties.CorrelationId;

                var order = JsonSerializer.Deserialize<OrderModel>(message);

                Console.WriteLine($"[MasterService] Received Order: {message}");

                // Simulate some DB processing or business logic
                var responseMessage = $"Processed: {order.ProductName}";

                await SendResponse(replyToQueue, responseMessage, correlationId);
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);
        }

        private async Task SendResponse(string responseQueue, string message, string correlationId)
        {
            var connection = await _connectionTask;
            var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: responseQueue, durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                passive: false,
                cancellationToken: CancellationToken.None);

            var props = new BasicProperties
            {
                CorrelationId = correlationId
            };

            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(exchange: "", routingKey: responseQueue, body: body);

            Console.WriteLine($"[MasterService] Sent Response to {responseQueue} with CorrelationId: {correlationId}");
        }

        public async Task StartConsumer()
        {
            var consumer = new ConsumerService(_rabbit);
            await consumer.ConsumeQueueAsync("requestQueue");
        }
    }
}
