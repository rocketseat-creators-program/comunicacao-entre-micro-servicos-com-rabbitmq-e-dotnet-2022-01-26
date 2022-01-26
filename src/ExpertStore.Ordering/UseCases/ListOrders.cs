using System.Reflection.Metadata;
using ExpertStore.Ordering.Domain;
using ExpertStore.Ordering.Repositories;
using ExpertStore.SeedWork.Interfaces;

namespace ExpertStore.Ordering.UseCases
{
    public class ListOrders: IUseCase<List<ListOrdersOutputItem>>
    {
        private readonly IOrderRepository _repository;

        public ListOrders(IOrderRepository repository)
        {
            _repository = repository;   
        }

        public async Task<List<ListOrdersOutputItem>> Handle()
        {
            var orders = await _repository.GetList();
            return orders.Select(ListOrdersOutputItem.FromOrder).ToList();
        }
    }

    public class ListOrdersOutputItem
    {
        public ListOrdersOutputItem(Guid id, int productId, String status, int quantity, DateTime date)
        {
            Id = id;
            ProductId = productId;
            Status = status;
            Quantity = quantity;
            Date = date;
        }

        public Guid Id { get; }
        public int ProductId { get; }
        public String Status { get; }
        public int Quantity { get; }
        public DateTime Date { get; }

        public static ListOrdersOutputItem FromOrder(Order order)
            => new ListOrdersOutputItem(
                order.Id, 
                order.ProductId, 
                order.Status.ToString(), 
                order.Quantity, 
                order.Date
            );
    }
}
