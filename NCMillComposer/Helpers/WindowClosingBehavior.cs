using Microsoft.Xaml.Behaviors;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;

namespace NCMillComposer.Helpers
{
    public class WindowClosingBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(WindowClosingBehavior),
                new PropertyMetadata(null));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Closing += OnWindowClosing;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Closing -= OnWindowClosing;
            base.OnDetaching();
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            Command?.Execute(null);
        }
    }
}
