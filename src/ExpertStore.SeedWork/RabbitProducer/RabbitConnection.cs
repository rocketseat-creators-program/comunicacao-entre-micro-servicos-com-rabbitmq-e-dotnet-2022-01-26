using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace ExpertStore.SeedWork.RabbitProducer;

public interface IRabbitConnection
{
    public IConnection Connection { get; }
}

public class RabbitConnection : IRabbitConnection
{
    public RabbitConnection(IConfiguration config)
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = config.GetSection("RabbitMQ")["HostName"],
            UserName = config.GetSection("RabbitMQ")["UserName"],
            VirtualHost = config.GetSection("RabbitMQ")["VirtualHost"],
            Password = config.GetSection("RabbitMQ")["Password"]
        };
        Connection = connectionFactory.CreateConnection(config.GetSection("RabbitMQ")["ConnectionName"]);
    }

    public IConnection Connection { get; set; }
}
