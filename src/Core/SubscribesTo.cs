namespace Core
{
    public interface SubscribesTo<TEvent>
    {
        void Handle(TEvent @event);
    }
}