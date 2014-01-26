using System;
using System.Linq;
using System.Windows.Input;
using Slab.Data;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Win8nl.Behaviors;

namespace SlabRt.Pages
{
    public class DragDropBehaviour : Behavior<FrameworkElement>
    {
        private TranslateTransform _translate;
        private Point _originalPosition;
        private LocationCommandMapping _isOverLocation;
        private object _itemBeingDragged;
        private FrameworkElement _floatingElement;

        protected override void OnAttached()
        {
            AssociatedObject.ManipulationMode = ManipulationModes.All;
            AssociatedObject.ManipulationStarting += OnManipulationStarting;
            AssociatedObject.ManipulationStarted += OnManipulationStarted;
            AssociatedObject.ManipulationCompleted += OnManipulationCompleted;
            AssociatedObject.ManipulationDelta += OnManipulationDelta;

            AssociatedObject.ManipulationInertiaStarting += AssociatedObjectOnManipulationInertiaStarting;
            
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.ManipulationStarting -= OnManipulationStarting;
            AssociatedObject.ManipulationStarted -= OnManipulationStarted;
            AssociatedObject.ManipulationCompleted -= OnManipulationCompleted;
            AssociatedObject.ManipulationDelta -= OnManipulationDelta;
            AssociatedObject.ManipulationInertiaStarting -= AssociatedObjectOnManipulationInertiaStarting;
            RemoveFloatingControl();
            base.OnDetaching();
        }

        public string FloatingControlTypeName { get; set; }

        public static readonly DependencyProperty LocationCommandMappingsProperty =
            DependencyProperty.Register("LocationCommandMappings", typeof(AsyncObservableCollection<LocationCommandMapping>), typeof(DragDropBehaviour), new PropertyMetadata(default(AsyncObservableCollection<LocationCommandMapping>)));

        public AsyncObservableCollection<LocationCommandMapping> LocationCommandMappings
        {
            get { return (AsyncObservableCollection<LocationCommandMapping>)GetValue(LocationCommandMappingsProperty); }
            set { SetValue(LocationCommandMappingsProperty, value); }
        }

        private Page _parentPage;
        private Page ParentPage
        {
            get { return _parentPage ?? (_parentPage = GetPage());}
        }

        private void OnManipulationStarting(object sender, ManipulationStartingRoutedEventArgs e)
        {
        }

        private void OnManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            var uiElement = (FrameworkElement)sender;
            _itemBeingDragged = uiElement.DataContext;
            _originalPosition = uiElement.TransformToVisual(ParentPage).TransformPoint(new Point(0, 0));

            _translate = new TranslateTransform
            {
                X = _originalPosition.X + e.Position.X,
                Y = _originalPosition.Y + e.Position.Y
            };

            var type = Type.GetType(FloatingControlTypeName);
            _floatingElement = (FrameworkElement)Activator.CreateInstance(type);
            
            // if we don't constrain the alignment the element can stretch, which will skew the size for centring when the Loaded event occurs.
            _floatingElement.HorizontalAlignment = HorizontalAlignment.Left;
            _floatingElement.VerticalAlignment = VerticalAlignment.Top;
            
            _floatingElement.DataContext = AssociatedObject.DataContext;
            _floatingElement.RenderTransform = _translate;
            _loadedHasBeenHandled = false;
            _floatingElement.Loaded += FloatingElementLoaded;

            var grid = (Grid)ParentPage.Content;
            grid.Children.Add(_floatingElement);
        }

        private bool _loadedHasBeenHandled;
        void FloatingElementLoaded(object sender, RoutedEventArgs e)
        {
            if (_loadedHasBeenHandled)
                return;
            _loadedHasBeenHandled = true;
            // Once the floating element is visible we know how big it actually is, so offset the translation to make the
            // drag point the centre of the element.
            var element = (FrameworkElement) sender;

            _translate.X -= element.ActualWidth / 2.0;
            _translate.Y -= element.ActualHeight / 2.0;
        }

        private Page GetPage()
        {
            var topMostPage = null as Page;
            var parent = VisualTreeHelper.GetParent(AssociatedObject);
            var nextParent = VisualTreeHelper.GetParent(parent);
            while (nextParent != null)
            {
                parent = nextParent;
                if (parent is Page)
                {
                    topMostPage = parent as Page;
                }
                nextParent = VisualTreeHelper.GetParent(parent);
            }
            return topMostPage;
        }

        private void OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            RemoveFloatingControl();

            if (_isOverLocation != null)
            {
                if (_isOverLocation.Command.CanExecute(_itemBeingDragged))
                {
                    _isOverLocation.Command.Execute(_itemBeingDragged);
                }
            }
        }

        private void AssociatedObjectOnManipulationInertiaStarting(object sender, ManipulationInertiaStartingRoutedEventArgs manipulationInertiaStartingRoutedEventArgs)
        {
            RemoveFloatingControl();
        }

        private void RemoveFloatingControl()
        {
            _translate = null;
            if (_floatingElement != null)
            {
                _floatingElement.Loaded -= FloatingElementLoaded;
                var panel = (Panel)_floatingElement.Parent;
                panel.Children.Remove(_floatingElement);
                _floatingElement = null;
            }
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (_translate == null)
                return;

            _translate.X += e.Delta.Translation.X;
            _translate.Y += e.Delta.Translation.Y;

            var currentPosition = (_floatingElement).TransformToVisual(ParentPage).TransformPoint(new Point(0, 0));

            var currentXPosition = currentPosition.X;
            var currentYPosition = currentPosition.Y;

            _isOverLocation = LocationCommandMappings
                .FirstOrDefault(lcm =>
                    lcm.Location != null && 
                    (lcm.Location.Position.X <= currentXPosition && currentXPosition <= (lcm.Location.Position.X + lcm.Location.Size.Width)) &&
                    (lcm.Location.Position.Y <= currentYPosition && currentYPosition <= (lcm.Location.Position.Y + lcm.Location.Size.Height)));

            e.Handled = true;
        }
    }

    public class LocationCommandMapping
    {
        public Location Location { get; set; }
        public ICommand Command { get; set; }
    }
}