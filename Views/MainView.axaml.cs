// Views/MainView.cs
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using ActivityPerson.ViewModels;

namespace ActivityPerson.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            // Устанавливаем DataContext
            DataContext = new MainWindowViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            // Назначаем обработчик события
            var addButton = this.FindControl<Button>("AddTaskButton");
            if (addButton != null)
            {
                addButton.Click += OnAddTaskButtonClicked;
            }
        }

        private void OnAddTaskButtonClicked(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
            {
                viewModel.AddTaskButton_Click();
            }
        }
    }
}