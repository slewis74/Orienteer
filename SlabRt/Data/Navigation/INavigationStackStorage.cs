namespace SlabRt.Data.Navigation
{
    public interface INavigationStackStorage
    {
        string DefaultRoute { get; }
        bool AlwaysStartFromDefaultRoute { get; }

        void StoreRoutes(string[] routes);
        string[] RetrieveRoutes();
    }
}