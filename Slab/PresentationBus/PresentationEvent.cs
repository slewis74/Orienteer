namespace Slab.PresentationBus
{
    public class PresentationEvent : IPresentationEvent
    {
        public bool IsHandled { get; set; }
        public bool MustBeHandled { get; protected set; }
    }

    public class PresentationEvent<T> : PresentationEvent, IPresentationEvent<T>
    {
        public PresentationEvent(T data)
        {
            Data = data;
        }

        public T Data { get; set; }
    }
}