namespace Slab.PresentationBus
{
    public interface IPresentationBus
    {
        void Subscribe<T>(IHandlePresentationEvent<T> handler) where T : IPresentationEvent;
        void Subscribe(IHandlePresentationEvents instance);

        void UnSubscribe<T>(IHandlePresentationEvent<T> handler) where T : IPresentationEvent;
        void UnSubscribe(IHandlePresentationEvents instance);

        void Publish<T>(T presentationEvent) where T : IPresentationEvent;
    }
}