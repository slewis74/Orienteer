namespace SlabRt.Data.Navigation
{
    public interface INavigationStackStorage
    {
        void StoreRoutes(string[] routes);
        string[] RetrieveRoutes();
    }
}