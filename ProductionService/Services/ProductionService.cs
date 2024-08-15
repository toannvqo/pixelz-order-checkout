using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ProductionService.Services
{
    public class ProductionService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public ProductionService()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void ListenForProductionRequests()
        {
            _channel.QueueDeclare(queue: "orderCompletedQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var orderId = int.Parse(message);
                // Logic to get order details by Id and update the status


                Console.WriteLine($"Processing Order {orderId}.");
            };
            _channel.BasicConsume(queue: "orderCompletedQueue", autoAck: true, consumer: consumer);
        }
    }
}
