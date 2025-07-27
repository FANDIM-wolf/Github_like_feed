using ActivityPerson.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace ActivityPerson.Views
{
    public partial class TaskDialogWindow : Window
    {
        public TaskDialogWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            // Назначаем обработчики событий
            var saveButton = this.FindControl<Button>("SaveButton");
            var cancelButton = this.FindControl<Button>("CancelButton");
            var textBox = this.FindControl<TextBox>("TaskDescriptionTextBox");

            saveButton.Click += SaveButton_Click;
            cancelButton.Click += CancelButton_Click;

            // Обновляем состояние кнопки при изменении текста
            textBox.TextChanged += (s, e) =>
            {
                var viewModel = DataContext as TaskDialogViewModel;
                if (viewModel != null)
                {
                    viewModel.TaskDescription = textBox.Text;
                    viewModel.OnPropertyChanged(nameof(viewModel.CanSave)); // Исправленный вызов
                }
            };
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as TaskDialogViewModel;
            viewModel?.SaveCommand.Execute(null);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as TaskDialogViewModel;
            viewModel?.CancelCommand.Execute(null);
        }
    }
}