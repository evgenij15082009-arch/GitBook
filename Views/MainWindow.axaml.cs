using Avalonia.Controls;
using GitBook.ViewModels;

namespace GitBook.Views
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