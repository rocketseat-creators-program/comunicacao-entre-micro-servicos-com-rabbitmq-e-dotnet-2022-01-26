using ExpertStore.SeedWork.IntegrationEvents;
using ExpertStore.SeedWork.RabbitProducer;
using Newtonsoft.Json;
using PaymentsProcessor.UseCases;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace PaymentsProcessor;
public class PaymentProcessorSubscriber : BackgroundService
{
    private readonly IModel _channel;
    private readonly ILogger _logger;
    private readonly IProcessPayment _processPaymentUseCase;
    private readonly IRabbitConnection _rabbitConnection;

    private string Queue;
    private string Exchange;

    public PaymentProcessorSubscriber(
        ILogger<PaymentProcessorSubscriber> logger,
        IRabbitConnection rabbitConnection,
        IConfiguration config,
        IServiceScopeFactory factory
    )
    {
        _logger = logger;
        _processPaymentUseCase = factory.CreateScope().ServiceProvider.GetRequiredService<IProcessPayment>();
        _rabbitConnection = rabbitConnection;

        Queue = config.GetSection("RabbitMQ").GetValue<string>("PaymentProcessorSubscriberQueue");
        Exchange = config.GetSection("RabbitMQ").GetValue<string>("Exchange");

        _channel = _rabbitConnection.Connection.CreateModel();
        _channel.ExchangeDeclare(
            exchange: Exchange,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false
        );

        _channel.QueueDeclare(Queue, false, false, false, null);
        _channel.QueueBind(Queue, Exchange, "order.created");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (sender, eventArgs) => {
            var contentArray = eventArgs.Body.ToArray();
            var contentString = Encoding.UTF8.GetString(contentArray);
            var message = JsonConvert.DeserializeObject<OrderCreatedEventPayload>(contentString);
            if (message == null)
                throw new NullReferenceException("Message received is null");

            _logger.LogInformation($"Payment processor received an event: {contentString}");

            var output = await _processPaymentUseCase.Handle(new ProcessPaymentInput(){ 
                Date = message.Date, 
                OrderId = message.OrderId, 
                ProductId = message.ProductId, 
                Quantity = message.Quantity 
            });

            _channel.BasicAck(eventArgs.DeliveryTag, false);

            _logger.LogInformation($"Payment processor: {output.OrderId} -> { (output.IsApproved ? "Approved" : "Refused") }");
        };

        _channel.BasicConsume(Queue, false, consumer);

        return Task.CompletedTask;
    }
}

public static class PaymentProcessorSubscriberExtensions
{
    public static IServiceCollection AddPaymentProcessorSubscriber(this IServiceCollection services)
    {
        services.AddHostedService<PaymentProcessorSubscriber>();
        return services;
    }
}
