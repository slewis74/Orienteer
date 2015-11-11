using Orienteer.Data;

namespace Orienteer.Xaml.ViewModels
{
    public abstract class HasPageTitle : Bindable
    {
        public abstract string PageTitle { get; }
    }
}