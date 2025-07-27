using ActivityPerson.Data;
using ActivityPerson.Models;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ActivityPerson.ViewModels
{
    public class TaskDialogViewModel : INotifyPropertyChanged
    {
        private string _taskDescription;
        private bool _canSave;
        private readonly MainWindowViewModel _mainViewModel;

        public string TaskDescription
        {
            get => _taskDescription;
            set
            {
                _taskDescription = value;
                OnPropertyChanged();
                CanSave = !string.IsNullOrWhiteSpace(value);
            }
        }

        public bool CanSave
        {
            get => _canSave;
            set
            {
                _canSave = value;
                OnPropertyChanged();
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public bool? DialogResult { get; private set; }

        public TaskDialogViewModel(MainWindowViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            SaveCommand = new RelayCommand(Save, () => CanSave);
            CancelCommand = new RelayCommand(Cancel);
            CanSave = false;
        }

        private async void Save()
        {
            if (string.IsNullOrWhiteSpace(TaskDescription)) return;

            try
            {
                using (var context = new AppDbContext())
                {
                    var today = DateTime.Today;

                    // Получаем или создаем день
                    var dataDay = await context.DataDays
                        .Include(d => d.Tasks)
                        .FirstOrDefaultAsync(d => d.Date == today)
                        ?? new DataDay { Date = today, ActivityLevel = 0 };

                    // Добавляем задачу
                    dataDay.Tasks.Add(new Models.Task
                    {
                        Description = TaskDescription,
                        DataDay = dataDay
                    });

                    // Обновляем уровень активности
                    dataDay.ActivityLevel = Math.Min(4, dataDay.Tasks.Count);

                    // Сохраняем изменения
                    if (dataDay.Id == 0) context.DataDays.Add(dataDay);
                    await context.SaveChangesAsync();

                    // Обновляем UI в главной VM
                    _mainViewModel.UpdateDay(dataDay);
                    _mainViewModel.ConnectionStatus = "Задача успешно добавлена!";
                }
            }
            catch (Exception ex)
            {
                _mainViewModel.ConnectionStatus = $"Ошибка: {ex.Message}";
            }
            finally
            {
                CloseWindow();
            }
        }


        private void Cancel()
        {
            DialogResult = false;
            CloseWindow();
        }

        private void CloseWindow()
        {
            var window = GetWindow();
            window?.Close();
        }

        private Window GetWindow()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.Windows.FirstOrDefault(w => w.DataContext == this);
            }
            return null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private class RelayCommand : ICommand
        {
            private readonly Action _execute;
            private readonly Func<bool> _canExecute;

            public RelayCommand(Action execute, Func<bool> canExecute = null)
            {
                _execute = execute;
                _canExecute = canExecute;
            }

            public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;
            public void Execute(object parameter) => _execute?.Invoke();
            public event EventHandler CanExecuteChanged;
        }
    }
}