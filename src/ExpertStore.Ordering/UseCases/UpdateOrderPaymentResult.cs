using ExpertStore.Ordering.Domain;
using ExpertStore.SeedWork.Interfaces;

namespace ExpertStore.Ordering.UseCases;

public class UpdateOrderPaymentResult : IUpdateOrderPaymentResult
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<UpdateOrderPaymentResult> _logger;

    public UpdateOrderPaymentResult(IOrderRepository orderRepository, ILogger<UpdateOrderPaymentResult> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<UpdateOrderPaymentResultOutput> Handle(UpdateOrderPaymentResultInput input)
    {
        var order = await _orderRepository.Get(input.OrderId);
        if (order == null)
            throw new Exception("Order not found");
        var newStatus = input.Approved ? OrderStatus.Approved : OrderStatus.PaymentError;
        order.UpdatePaymentStatus(newStatus);
        await _orderRepository.Update(order);
        _logger.LogInformation($"Order {order.Id} updated to {order.Status.ToString()}");
        return new UpdateOrderPaymentResultOutput();
    }
}

public interface IUpdateOrderPaymentResult : IUseCase<UpdateOrderPaymentResultInput, UpdateOrderPaymentResultOutput> {}

public class UpdateOrderPaymentResultInput
{
    public Guid OrderId { get; set;}
    public bool Approved { get; set; }
}

public class UpdateOrderPaymentResultOutput { }