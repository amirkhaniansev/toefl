using System.Windows;
using Toefl.ToeflDesktopUI.ViewModels;

namespace Toefl.ToeflDesktopUI.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = new MainViewModel();
        }
    }
}