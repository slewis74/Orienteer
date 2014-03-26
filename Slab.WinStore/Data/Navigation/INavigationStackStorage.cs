namespace Slab.WinStore.Data.Navigation
{
    public interface INavigationStackStorage
    {
        string DefaultRoute { get; }
        bool AlwaysStartFromDefaultRoute { get; }

        void LaunchingDeepLink(string route);

        void StoreRoutes(string[] routes);
        string[] RetrieveRoutes();
    }
}