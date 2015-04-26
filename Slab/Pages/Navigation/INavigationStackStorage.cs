namespace Slab.Pages.Navigation
{
    public interface INavigationStackStorage
    {
        void StoreRoutes(string[] routes);
        string[] RetrieveRoutes(string defaultRoute);
    }
}