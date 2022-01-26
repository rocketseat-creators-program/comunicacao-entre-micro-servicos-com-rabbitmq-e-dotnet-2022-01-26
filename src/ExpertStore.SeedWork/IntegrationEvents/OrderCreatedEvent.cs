using ExpertStore.SeedWork.Interfaces;

namespace ExpertStore.SeedWork.IntegrationEvents
{
    public class OrderCreatedEvent : IIntegrationEvent
    {
        public OrderCreatedEvent(
            DateTime date, 
            Guid orderId, 
            int productId, 
            int quantity
        ) {
            Event = new OrderCreatedEventPayload
            {
                Date = date,
                OrderId = orderId,
                ProductId = productId,
                Quantity = quantity,
            };
        }

        public object Event { get; set; }
    }

    public class OrderCreatedEventPayload
    {
        public Guid OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime Date { get; set; }
    }
}


