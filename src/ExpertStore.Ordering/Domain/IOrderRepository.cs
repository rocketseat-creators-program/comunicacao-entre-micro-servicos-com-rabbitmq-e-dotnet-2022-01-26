namespace ExpertStore.Ordering.Domain
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetList();
        Task<Order?> Get(Guid id);
        Task Update(Order order);
        Task Save(Order order);
    }
}
