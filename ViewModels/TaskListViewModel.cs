using ActivityPerson.Data;
using ActivityPerson.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ActivityPerson.ViewModels
{
    public class TaskListViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<DayTasks> _daysWithTasks = new ObservableCollection<DayTasks>();
        private string _statusMessage;

        public ObservableCollection<DayTasks> DaysWithTasks
        {
            get => _daysWithTasks;
            set
            {
                _daysWithTasks = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public TaskListViewModel()
        {
            LoadTasks();
        }

        private async void LoadTasks()
        {
            try
            {
                using (var context = new AppDbContext())
                {
                    // Загружаем дни с задачами
                    var days = await context.DataDays
                        .Include(d => d.Tasks)
                        .Where(d => d.Tasks.Any())
                        .OrderByDescending(d => d.Date)
                        .ToListAsync();

                    DaysWithTasks.Clear();

                    // Группируем по дням
                    foreach (var day in days)
                    {
                        DaysWithTasks.Add(new DayTasks
                        {
                            Date = day.Date,
                            Tasks = day.Tasks.ToList(),
                            ActivityLevel = day.ActivityLevel
                        });
                    }

                    StatusMessage = days.Any()
                        ? $"Найдено {days.Count} дней с задачами"
                        : "Задачи не найдены";
                }
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка загрузки: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DayTasks
    {
        public DateTime Date { get; set; }
        public List<Models.Task> Tasks { get; set; }
        public int ActivityLevel { get; set; }

        public string DisplayDate => Date.ToString("dd.MM.yyyy");
        public string TaskCountText => $"{Tasks.Count} задач(и)";
    }
}