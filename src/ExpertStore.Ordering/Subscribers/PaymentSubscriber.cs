using ExpertStore.Ordering.UseCases;
using ExpertStore.SeedWork.IntegrationEvents;
using ExpertStore.SeedWork.RabbitProducer;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ExpertStore.Ordering.Subscribers;

public class PaymentSubscriber : BackgroundService
{
    private readonly IModel _channel;
    private readonly ILogger _logger;
    private readonly IUpdateOrderPaymentResult _updateOrderPaymentResult;
    private readonly IRabbitConnection _rabbitConnection;

    private readonly string Queue;
    private readonly string Exchange;

    public PaymentSubscriber(
        ILogger<PaymentSubscriber> logger,
        IRabbitConnection rabbitConnection,
        IConfiguration config,
        IServiceScopeFactory factory
    )
    {
        _logger = logger;
        _updateOrderPaymentResult = factory.CreateScope().ServiceProvider.GetRequiredService<IUpdateOrderPaymentResult>();
        _rabbitConnection = rabbitConnection;

        Queue = config.GetSection("RabbitMQ").GetValue<string>("PaymentSubscriberQueue");
        Exchange = config.GetSection("RabbitMQ").GetValue<string>("Exchange");

        _channel = _rabbitConnection.Connection.CreateModel();
        _channel.ExchangeDeclare(
            exchange: Exchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false
        );

        _channel.QueueDeclare(Queue, false, false, false, null);
        _channel.QueueBind(Queue, Exchange, "payment.*");
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

            _logger.LogInformation($"Ordering payment subscriber received an event: {contentString}");

            var output = await _updateOrderPaymentResult.Handle(new UpdateOrderPaymentResultInput() {
                OrderId = message.OrderId,
                Approved = message.Approved,
            });

            _channel.BasicAck(eventArgs.DeliveryTag, false);
        };

        _channel.BasicConsume(Queue, false, consumer);

        return Task.CompletedTask;
    }
}

public static class PaymentSubscriberExtensions
{
    public static IServiceCollection AddPaymentSubscriber(this IServiceCollection services)
    {
        services.AddHostedService<PaymentSubscriber>();
        return services;
    }
}
