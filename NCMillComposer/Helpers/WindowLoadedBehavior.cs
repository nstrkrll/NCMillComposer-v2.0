using Microsoft.Xaml.Behaviors;
using System.Windows.Input;
using System.Windows;

namespace NCMillComposer.Helpers
{
    public class WindowLoadedBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(WindowLoadedBehavior),
                new PropertyMetadata(null));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += OnWindowLoaded;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Loaded -= OnWindowLoaded;
            base.OnDetaching();
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            Command?.Execute(null);
        }
    }
}
