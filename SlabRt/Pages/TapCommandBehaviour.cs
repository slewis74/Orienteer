using System;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Win8nl.Behaviors;

namespace SlabRt.Pages
{
    public class TapCommandBehaviour : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof (ICommand), typeof (TapCommandBehaviour), new PropertyMetadata(default(ICommand)));

        public ICommand Command
        {
            get { return (ICommand) GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof (object), typeof (TapCommandBehaviour), new PropertyMetadata(default(object)));

        public object CommandParameter
        {
            get { return (object) GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        protected override void OnAttached()
        {
            AssociatedObject.Tapped += AssociatedObjectOnTapped;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Tapped -= AssociatedObjectOnTapped;
            base.OnDetaching();
        }

        private void AssociatedObjectOnTapped(object sender, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            if (Command.CanExecute(CommandParameter))
            {
                Command.Execute(CommandParameter);
            }
        }

    }
}