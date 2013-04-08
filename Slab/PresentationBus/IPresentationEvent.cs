namespace Slab.PresentationBus
{
    public interface IPresentationEvent
    {
        bool IsHandled { get; set; }
        bool MustBeHandled { get; }
    }

    public interface IPresentationEvent<out T> : IPresentationEvent
    {
        T Data { get; }
    }
}