using CommonService.Utility;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using System.Collections.Concurrent;
using System;

namespace UserService.Services
{
    public class grpcUserService
    {
        private readonly Task<IConnection> _connectionTask;
        private static readonly ConcurrentDictionary<string, TaskCompletionSource<string>> _responseHandlers = new();

        public grpcUserService(RabbitMQConnectionHelper rabbitMq)
        {
            _connectionTask = rabbitMq.GetConnectionAsync();
        }

        public async Task<string> SendMessageAsync<T> (string requestQueue, string responseQueue, T message)
        {
            var connection = await _connectionTask;
            var channel = await connection.CreateChannelAsync();

            var correlationId = Guid.NewGuid().ToString();
            var props = new BasicProperties
            {
                CorrelationId = correlationId,
                ReplyTo = responseQueue
            };

            string jsonMessage = JsonSerializer.Serialize(message);
            byte[] body = Encoding.UTF8.GetBytes(jsonMessage);

            await channel.QueueDeclareAsync(queue: requestQueue, durable: false, exclusive: false, autoDelete: false);
            await channel.QueueDeclareAsync(queue: responseQueue, durable: false, exclusive: false, autoDelete: false);

            // result.QueueName, queue.Length == 0, durable, exclusive, autoDelete, arguments

            await StartConsumerAsync(channel, responseQueue); // Only starts once per app

            var tcs = new TaskCompletionSource<string>();
            _responseHandlers[correlationId] = tcs;

            await channel.BasicPublishAsync(exchange: "", routingKey: requestQueue, body: body);

            Console.WriteLine($"[UserService] Sent Request: {jsonMessage} with CorrelationId: {correlationId} with ReplyTo : {responseQueue}");

            return await tcs.Task;
        }

        private static bool _consumerStarted = false;
        private static readonly object _lock = new();

        private async Task StartConsumerAsync(IChannel channel, string responseQueue)
        {
            lock (_lock)
            {
                if (_consumerStarted) return;
                _consumerStarted = true;
            }

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var replyToQueue = ea.BasicProperties.ReplyTo;
                var correlationId = ea.BasicProperties?.CorrelationId;

                if (correlationId != null && _responseHandlers.TryRemove(correlationId, out var tcs))
                {
                    tcs.TrySetResult(message);
                }

                await Task.Yield();
            };

            await channel.BasicConsumeAsync(queue: responseQueue, autoAck: true, consumer: consumer);
        }

    }
}
