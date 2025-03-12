using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows;

namespace NCMillComposer.Helpers
{
    public class SizeObserverBehavior : Behavior<Canvas>
    {
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(SizeObserverBehavior),
                new PropertyMetadata(0.0));

        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register("Height", typeof(double), typeof(SizeObserverBehavior),
                new PropertyMetadata(0.0));

        public double Width
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public double Height
        {
            get => (double)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SizeChanged += Canvas_SizeChanged;
            AssociatedObject.Loaded += Canvas_Loaded;
            UpdateSize();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.SizeChanged -= Canvas_SizeChanged;
            AssociatedObject.Loaded -= Canvas_Loaded;
            base.OnDetaching();
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateSize();
        }

        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSize();
        }

        private void UpdateSize()
        {
            if (AssociatedObject != null)
            {
                Width = AssociatedObject.ActualWidth;
                Height = AssociatedObject.ActualHeight;
            }
        }
    }
}
