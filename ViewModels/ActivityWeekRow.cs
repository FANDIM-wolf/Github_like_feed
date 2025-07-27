using System.Collections.ObjectModel;
using ActivityPerson.Models;

namespace ActivityPerson.ViewModels
{
    public class ActivityWeekRow
    {
        public string DayOfWeekLabel { get; set; }
        public ObservableCollection<DataDay> Days { get; set; }

        public ActivityWeekRow(string dayOfWeekLabel)
        {
            DayOfWeekLabel = dayOfWeekLabel;
            Days = new ObservableCollection<DataDay>();
        }
    }
}