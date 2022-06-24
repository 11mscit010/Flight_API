using FlightService.Entity;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightService.Service
{
    public static class FlightConsumer
    {
        private static AppDbContext context;
        private static IFlightRepository flightRepository;
        
        public static void GetHelperConfiguration(AppDbContext _context)
        {
            context = _context;
            flightRepository = new FlightRepository(context);
        }

        public static void QueueConsumer(string QueueName)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672") //here 5672 is a port pass into create docker image
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.QueueDeclare(QueueName, durable: true, exclusive: false, arguments: null);
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var msg = Encoding.UTF8.GetString(body);
                if (QueueName == "Airline-Q1" && !string.IsNullOrEmpty(msg))
                {
                    var data = JsonConvert.DeserializeObject<List<KeyValuePair<string, string>>>(msg);
                    if(data != null && data.Count > 0)
                    {
                        var airlineId = Convert.ToInt32(data[0].Value);
                        var isBlock = Convert.ToBoolean(data[1].Value);
                        flightRepository.BlockUnblockFlight(airlineId,isBlock);
                    }                    
                }
            };
            channel.BasicConsume(QueueName, true, consumer);
        }
    }
}
