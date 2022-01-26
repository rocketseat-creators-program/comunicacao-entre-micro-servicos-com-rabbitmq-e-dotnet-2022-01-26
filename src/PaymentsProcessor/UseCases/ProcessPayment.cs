using ExpertStore.SeedWork.IntegrationEvents;
using ExpertStore.SeedWork.Interfaces;

namespace PaymentsProcessor.UseCases;

public class ProcessPayment : IProcessPayment
{
    private readonly IEventBus _eventBus;
    public ProcessPayment(IEventBus eventBus) 
        => _eventBus = eventBus;

    public Task<ProcessPaymentOutput> Handle(ProcessPaymentInput input)
    {
        // the processment logic
        Thread.Sleep(10 * 1000);
        var processed = (input.ProductId % 7 == 0);

        if (processed)
            _eventBus.Publish(new PaymentApprovedEvent(
                input.Date, 
                input.OrderId, 
                input.ProductId, 
                input.Quantity));
        else
            _eventBus.Publish(new PaymentRefusedEvent(
                input.Date,
                input.OrderId,
                input.ProductId,
                input.Quantity, 
                "Credid Card Refused"));

        return Task.FromResult(new ProcessPaymentOutput() { OrderId = input.OrderId, IsApproved = processed });
    }
}

public interface IProcessPayment : IUseCase<ProcessPaymentInput, ProcessPaymentOutput> { }

public class ProcessPaymentInput
{
    public Guid OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime Date { get; set; }
}

public class ProcessPaymentOutput
{
    public Guid OrderId { get; set; }
    public bool IsApproved { get; set; }
}