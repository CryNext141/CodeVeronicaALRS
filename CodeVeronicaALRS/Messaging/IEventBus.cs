namespace CodeVeronicaALRS.Messaging
{
    public interface IEventBus
    {
        void Publish<T>(T @event, string routingKey);
    }
}
