namespace ExpertStore.SeedWork.Interfaces;

public interface IEventBus
{
    void Publish(IIntegrationEvent @event);
}
