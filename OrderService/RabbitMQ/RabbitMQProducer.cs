using RabbitMQ.Client;
using System.Text;

public interface IMessageProducer
{
    void PublishToQueue(string queueName, string message);
}

public class RabbitMQProducer : IMessageProducer
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMQProducer(IConnectionProvider connectionProvider)
    {
        _connection = connectionProvider.GetConnection();
        _channel = _connection.CreateModel();
    }

    public void PublishToQueue(string queueName, string message)
    {
        _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
    }
}
