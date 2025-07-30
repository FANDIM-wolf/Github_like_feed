using ActivityPerson.Data;
using ActivityPerson.Models;
using ActivityPerson.Views;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ActivityPerson.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private string _connectionStatus;
        private ObservableCollection<string> _months;
        private ObservableCollection<ActivityWeekRow> _activityWeekRows;

        public string ConnectionStatus
        {
            get => _connectionStatus;
            set
            {
                _connectionStatus = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Months
        {
            get => _months;
            set
            {
                _months = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ActivityWeekRow> ActivityWeekRows
        {
            get => _activityWeekRows;
            set
            {
                _activityWeekRows = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            CheckConnection();
            InitializeData();
        }

        private async void CheckConnection()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    var canConnect = await context.Database.CanConnectAsync();
                    if (canConnect)
                    {
                        ConnectionStatus = "Подключение к базе данных успешно установлено!";
                    }
                    else
                    {
                        ConnectionStatus = "Не удалось подключиться к базе данных.";
                    }
                }
            }
            catch (Exception ex)
            {
                ConnectionStatus = $"Ошибка подключения: {ex.Message}";
            }
        }

        private void InitializeData()
        {
            // Инициализация месяцев
            Months = new ObservableCollection<string>
            {
                "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
            };

            // Инициализация строк недели
            ActivityWeekRows = new ObservableCollection<ActivityWeekRow>
            {
                new ActivityWeekRow("Mon"),
                new ActivityWeekRow("Tue"),
                new ActivityWeekRow("Wed"),
                new ActivityWeekRow("Thu"),
                new ActivityWeekRow("Fri"),
                new ActivityWeekRow("Sat"),
                new ActivityWeekRow("Sun")
            };

            // Загрузка данных из БД вместо генерации тестовых данных
            LoadActivityDataFromDatabase();
        }

        public void UpdateDay(DataDay updatedDay)
        {
            try
            {
                // Находим день в коллекции
                var dayInCollection = ActivityWeekRows
                    .SelectMany(row => row.Days)
                    .FirstOrDefault(d => d.Date == updatedDay.Date);

                if (dayInCollection != null)
                {
                    // Обновляем только необходимые свойства
                    dayInCollection.Tasks = updatedDay.Tasks;
                    dayInCollection.ActivityLevel = updatedDay.ActivityLevel;

                    // Уведомляем об изменении
                    OnPropertyChanged(nameof(dayInCollection.Color));
                    OnPropertyChanged(nameof(dayInCollection.TaskCount));
                }
            }
            catch (Exception ex)
            {
                ConnectionStatus = $"Ошибка обновления: {ex.Message}";
            }
        }

        private void LoadActivityDataFromDatabase()
        {
            using (var context = new AppDbContext())
            {
                var currentYear = DateTime.Now.Year;
                var existingDays = context.DataDays
                    .Include(d => d.Tasks)
                    .Where(d => d.Date.Year == currentYear)
                    .ToDictionary(d => d.Date);

                // Заполняем коллекции
                foreach (var weekRow in ActivityWeekRows)
                {
                    weekRow.Days.Clear();
                }

                var startDate = new DateTime(currentYear, 1, 1);
                var endDate = new DateTime(currentYear, 12, 31);

                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    var dayOfWeekIndex = (int)date.DayOfWeek;
                    var displayIndex = dayOfWeekIndex == 0 ? 6 : dayOfWeekIndex - 1;

                    if (displayIndex < ActivityWeekRows.Count)
                    {
                        // Используем данные из БД или создаем новый объект
                        var dataDay = existingDays.TryGetValue(date, out var existing)
                            ? existing
                            : new DataDay { Date = date, ActivityLevel = 0 };

                        ActivityWeekRows[displayIndex].Days.Add(dataDay);
                    }
                }
            }
        }
        /*
         public async void AddTaskButton_Click()
         {
             try
             {
                 var dialog = new TaskDialogWindow
                 {
                     DataContext = new TaskDialogViewModel(this)
                 };

                 Window owner = GetMainWindow();
                 await dialog.ShowDialog(owner);

                 // Результат обрабатывается внутри TaskDialogViewModel
             }
             catch (Exception ex)
             {
                 ConnectionStatus = $"Ошибка: {ex.Message}";
             }
         }

         private Window GetMainWindow()
         {
             if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
             {
                 return desktop.Windows.FirstOrDefault() ?? desktop.MainWindow;
             }
             return null;
         }

         private ICommand _viewTasksCommand;
         public ICommand ViewTasksCommand => _viewTasksCommand ??= new RelayCommand(ExecuteViewTasks);

         public async void ExecuteViewTasks()
         {
             try
             {
                 var window = new TaskListViewWindow
                 {
                     DataContext = new TaskListViewModel()
                 };

                 if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                 {
                     await window.ShowDialog(desktop.MainWindow);
                 }
             }
             catch (Exception ex)
             {
                 Debug.WriteLine($"Ошибка: {ex}");
                 ConnectionStatus = $"Ошибка открытия: {ex.Message}";
             }
         }

        */

        private ICommand _addTaskCommand;
        private ICommand _viewTasksCommand;

        public ICommand AddTaskCommand => _addTaskCommand ??= new RelayCommand(ExecuteAddTask);
        public ICommand ViewTasksCommand => _viewTasksCommand ??= new RelayCommand(ExecuteViewTasks);

        private void ExecuteAddTask()
        {
            try
            {
                var dialog = new TaskDialogWindow
                {
                    DataContext = new TaskDialogViewModel(this)
                };

                Window owner = GetMainWindow();
                dialog.ShowDialog(owner);
            }
            catch (Exception ex)
            {
                ConnectionStatus = $"Ошибка: {ex.Message}";
            }
        }
        private Window GetMainWindow()
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                return desktop.Windows.FirstOrDefault() ?? desktop.MainWindow;
            }
            return null;
        }
        private void ExecuteViewTasks()
        {
            try
            {
                var window = new TaskListViewWindow
                {
                    DataContext = new TaskListViewModel()
                };

                if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    window.ShowDialog(desktop.MainWindow);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка: {ex}");
                ConnectionStatus = $"Ошибка открытия: {ex.Message}";
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;

        public void Execute(object parameter) => _execute();
    }
}