namespace Slab.PresentationBus
{
    public class PresentationRequest : IPresentationRequest
    {
        public bool IsHandled { get; set; }
        public bool MustBeHandled { get; protected set; }
    }

    public class PresentationRequest<T> : PresentationRequest
    {
        public PresentationRequest(T args)
        {
            Args = args;
        }

        public T Args { get; set; }
    }
}