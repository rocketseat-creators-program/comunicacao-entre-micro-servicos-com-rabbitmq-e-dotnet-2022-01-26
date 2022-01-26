namespace ExpertStore.Ordering.Domain
{
    public enum OrderStatus
    {
        Processing = 0,
        Approved = 1,
        PaymentError = 2,
        Error = 3
    }

    public class Order
    {
        public Order(int productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
            Id = Guid.NewGuid();
            Status = OrderStatus.Processing;
            Date = DateTime.Now;
        }

        public void UpdatePaymentStatus(OrderStatus newStatus)
            => Status = newStatus;

        public Guid Id { get; }
        public int ProductId { get; }
        public OrderStatus Status  { get; private set; }
        public int Quantity { get; }
        public DateTime Date { get; }
    }
}
