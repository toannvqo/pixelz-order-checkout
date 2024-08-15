using RabbitMQ.Client;

public interface IConnectionProvider
{
    IConnection GetConnection();
}

public class RabbitMQConnectionProvider : IConnectionProvider
{
    private readonly IConnection _connection;

    public RabbitMQConnectionProvider()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
    }

    public IConnection GetConnection()
    {
        return _connection;
    }
}
