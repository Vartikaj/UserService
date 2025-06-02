using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonService.Utility
{
    public class RabbitMQConnectionHelper
    {
        private readonly ConnectionFactory _connectionFactory;
        // private IConnection _connection;

        public RabbitMQConnectionHelper()
        {
            _connectionFactory = new ConnectionFactory() { 
                HostName = "localhost",
                UserName = "vartika",
                Password = "1234",
            };
        }

        public async Task<IConnection> GetConnectionAsync()
        {
            
            //return await _connectionFactory.CreateConnectionAsync();
            return await _connectionFactory.CreateConnectionAsync();
        }
    }
}
