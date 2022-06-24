using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirlineService.Service
{
    public static class AirlineProducer
    {
        public static void QueueProducer(string QueueName, object data)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672") //here 5672 is a port pass while create docker image
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(QueueName, durable: true, exclusive: false, autoDelete: true, arguments: null);
            var body = Encoding.UTF8.GetBytes(data.ToString());
            
            channel.BasicPublish("", QueueName, null, body);
        }
    }
}
