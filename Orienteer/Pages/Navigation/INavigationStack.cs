namespace Orienteer.Pages.Navigation
{
    public interface INavigationStack
    {
        string DefaultRoute { get; }
        bool AlwaysStartFromDefaultRoute { get; }

        void LaunchingDeepLink(string route);

        void StoreRoutes(string[] routes);
        string[] RetrieveRoutes();
    }
}