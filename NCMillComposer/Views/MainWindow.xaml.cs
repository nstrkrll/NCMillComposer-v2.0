using NCMillComposer.ViewModels;
using System.Windows;

namespace NCMillComposer.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}