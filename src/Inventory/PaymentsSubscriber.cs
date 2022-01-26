using ExpertStore.SeedWork.IntegrationEvents;
using ExpertStore.SeedWork.RabbitProducer;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Inventory;


public class PaymentsSubscriber : BackgroundService
{
    private readonly IModel _channel;
    private readonly ILogger _logger;
    private readonly IRabbitConnection _rabbitConnection;

    private string Queue;
    private string Exchange;

    public PaymentsSubscriber(
        ILogger<PaymentsSubscriber> logger,
        IRabbitConnection rabbitConnection,
        IConfiguration config
    )
    {
        _logger = logger;
        _rabbitConnection = rabbitConnection;

        Queue = config.GetSection("RabbitMQ").GetValue<string>("Queue");
        Exchange = config.GetSection("RabbitMQ").GetValue<string>("Exchange");

        _channel = _rabbitConnection.Connection.CreateModel();
        _channel.ExchangeDeclare(
            exchange: Exchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false
        );

        _channel.QueueDeclare(Queue, false, false, false, null);
        _channel.QueueBind(Queue, Exchange, "payment.approved");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (sender, eventArgs) => {
            var contentArray = eventArgs.Body.ToArray();
            var contentString = Encoding.UTF8.GetString(contentArray);
            var message = JsonConvert.DeserializeObject<PaymentProcessedPayload>(contentString);
            if (message == null)
                throw new NullReferenceException("Message received is null");

            _logger.LogInformation($"Process inventory removing {message.Quantity} units of the product id {message.ProductId}");

            _channel.BasicAck(eventArgs.DeliveryTag, false);
        };

        _channel.BasicConsume(Queue, false, consumer);

        return Task.CompletedTask;
    }
}

public static class PaymentsSubscriberExtensions
{
    public static IServiceCollection AddPaymentsSubscriber(this IServiceCollection services)
    {
        services.AddHostedService<PaymentsSubscriber>();
        return services;
    }
}