using Microsoft.AspNetCore.Mvc;

namespace MasterServiceDemo.Utility
{
    public class RabbitMQBackgroundService : BackgroundService
    {
        private readonly ConsumerService _consumer;

        public RabbitMQBackgroundService(ConsumerService consumer)
        {
            _consumer = consumer;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.ConsumeQueueAsync("orderQueue");
            return Task.CompletedTask;
        }
    }
}
