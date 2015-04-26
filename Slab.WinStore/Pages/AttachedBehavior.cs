using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace Slab.WinStore.Pages
{
    public abstract class AttachedBehavior : DependencyObject, IBehavior
    {
        public DependencyObject AssociatedObject { get; private set; }

        public FrameworkElement AssociatedFrameworkElement { get { return (FrameworkElement) AssociatedObject; } }

        public void Attach(DependencyObject associatedObject)
        {
            AssociatedObject = associatedObject;
            OnAttached();
        }
        protected virtual void OnAttached()
        {}

        public void Detach()
        {
            OnDetaching();
            AssociatedObject = null;
        }
        protected virtual void OnDetaching()
        { }
    }
}