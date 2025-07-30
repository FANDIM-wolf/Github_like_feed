using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ActivityPerson.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            // Установка DataContext, если не задан в XAML
            if (DataContext == null)
            {
                DataContext = new ViewModels.MainWindowViewModel();
            }
        }
    }
}