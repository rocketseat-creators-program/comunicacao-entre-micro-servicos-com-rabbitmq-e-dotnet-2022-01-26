using ExpertStore.SeedWork.Interfaces;

namespace ExpertStore.SeedWork.IntegrationEvents;

public class PaymentApprovedEvent : PaymentProcessedEvent
{
    public PaymentApprovedEvent(DateTime date, Guid orderId, int productId, int quantity)
        : base(date, orderId, productId, quantity, true, "") { }
}

public class PaymentRefusedEvent : PaymentProcessedEvent
{
    public PaymentRefusedEvent(DateTime date, Guid orderId, int productId, int quantity, string note)
        : base(date, orderId, productId, quantity, false, note) { }
}

public class PaymentProcessedEvent : IIntegrationEvent
{
    public PaymentProcessedEvent(
        DateTime date,
        Guid orderId,
        int productId,
        int quantity,
        bool approved,
        string note = ""
    )
    {
        Event = new PaymentProcessedPayload
        {
            Date = date,
            OrderId = orderId,
            ProductId = productId,
            Quantity = quantity,
            Approved = approved,
            Note = note
        };
    }

    public object Event { get; set; }
}

public class PaymentProcessedPayload
{
    public Guid OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime Date { get; set; }
    public bool Approved { get; set; }
    public string Note { get; set; }
}
