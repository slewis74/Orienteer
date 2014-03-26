using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Slab.WinStore.Controls
{
    public class PageHeader : ContentControl
    {
        public static readonly DependencyProperty CurrentPageTitleProperty =
            DependencyProperty.Register("CurrentPageTitle", typeof (string), typeof (PageHeader), new PropertyMetadata(default(string)));

        public string CurrentPageTitle
        {
            get { return (string) GetValue(CurrentPageTitleProperty); }
            set { SetValue(CurrentPageTitleProperty, value); }
        }

        public static readonly DependencyProperty BackProperty =
            DependencyProperty.Register("Back", typeof (ICommand), typeof (PageHeader), new PropertyMetadata(default(ICommand)));

        public ICommand Back
        {
            get { return (ICommand) GetValue(BackProperty); }
            set { SetValue(BackProperty, value); }
        }
    }
}