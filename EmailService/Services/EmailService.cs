using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace EmailService.Services
{
    public class EmailService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public EmailService()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void ListenForEmailRequests()
        {
            _channel.QueueDeclare(queue: "orderCompletedQueue", durable: false, exclusive: false, autoDelete: false, arguments: null);
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var orderCompletedMessage = JsonSerializer.Deserialize<OrderCompletedMessage>(message);

                // Logic to send email
                // we can use caching to store the email template and use it here

                Console.WriteLine($"Sending email for Order {orderCompletedMessage.OrderId} to Customer.");
            };
            _channel.BasicConsume(queue: "orderCompletedQueue", autoAck: true, consumer: consumer);
        }
    }

    public class OrderCompletedMessage
    {
        public int OrderId { get; set; }
        public string OrderName { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
