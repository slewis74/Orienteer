namespace Slab.PresentationBus
{
    public interface IHandlePresentationRequest<in T> : IHandlePresentationEvent<T>
        where T : IPresentationRequest
    {}
}