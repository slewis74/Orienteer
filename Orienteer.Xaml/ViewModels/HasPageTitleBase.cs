using System.Threading;
using Orienteer.Data;

namespace Orienteer.Xaml.ViewModels
{
    public abstract class HasPageTitleBase : BindableBase
    {
        protected HasPageTitleBase()
        {}

        protected HasPageTitleBase(SynchronizationContext synchronizationContext) : base(synchronizationContext)
        {}

        public abstract string PageTitle { get; }
    }
}